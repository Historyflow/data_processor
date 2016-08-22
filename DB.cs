using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MySql.Data;
using Npgsql;
using MySql.Data.MySqlClient;
//using TStringIntItem = System.Collections.Generic.KeyValuePair<string, int>;

namespace DBproc
{   
    public class MySQLDB
    {

        public static string myConnectionString = "server=localhost;uid=root;pwd=;database=my_wiki;";
        //public static string realwikiConnectionString = "server=localhost;uid=root;pwd=;database=my_wiki3;";
        //public static string DBPrefix = "mw3";
        //Работа
        public static string realwikiConnectionString = "server=localhost;uid=root;pwd=;database=ewiki;";
        public static string DBPrefix = "e_";

        public static List<WikiPage> ReadPages(string[] page_titles, bool multiple = false, int limit = 0)
        {
            var lst = new List<WikiPage>();
            MySql.Data.MySqlClient.MySqlConnection conn;
            try
            {
                conn = new MySql.Data.MySqlClient.MySqlConnection(MySQLDB.realwikiConnectionString); // myConnectionString
                conn.Open();
                StringBuilder query = new StringBuilder();
                query.AppendFormat(" SELECT page_id, page_title, page_is_redirect, page_len, page_content_model, page_lang, page_latest, rev_text_id, old_text "
                    //                + ", hmid "
            + " FROM {0}page AS p "
            + " LEFT JOIN {0}revision AS r ON p.page_latest = r.rev_id "
            + " LEFT JOIN {0}text AS t ON r.rev_text_id = t.old_id "
            + " WHERE page_namespace = 0 "//AND page_is_redirect = 0 "
            + "   AND (FALSE "
            , MySQLDB.DBPrefix);
                foreach (string s in page_titles)
                {
                    if (multiple)
                        query.AppendFormat(" OR REPLACE(LOWER(page_title),'_',' ') REGEXP '[[:<:]]{0}[[:>:]]' ", s.ToLower());
                    else
                        query.AppendFormat(" OR REPLACE(LOWER(page_title),'_',' ') = '{0}' ", s.ToLower());
                }
                query.AppendFormat(" )");
                if (limit > 0)
                    query.AppendFormat(" LIMIT {0}", limit);
                //MessageBox.Show("MySQL Connection Open ! ");
                MySqlCommand myCommand = new MySqlCommand(query.ToString(), conn);
                MySqlDataReader reader = myCommand.ExecuteReader();
                while (reader.Read())
                    lst.Add(new WikiPage(reader));
                conn.Close();
            }
            catch (Exception ex)
            {
                return null;
            }
            return lst;
           }    
    }

    public class PGDB
    {
        public static string NpgsqlConnectionString = "Server=localhost;User Id=postgres;Password=123;Database=hm;";
        
        public static int BDInsertElement(HMElement el)
        {
            // поместить Element в БД
            // только инсерт, возвр ел.ид или -1, возм если ел.ид уже есть - делает апдейт (потом)
            int count = -1;
            if (el.element_type == null)
                return -3;
            NpgsqlConnection conn = new NpgsqlConnection(NpgsqlConnectionString);
            try
            {
                conn.Open();
                StringBuilder query = new StringBuilder();
                query.AppendFormat(" INSERT INTO public.hm_elements( "
                    + " name, label, description, info, weight, start_date, "
                    + " end_date, element_type_id, text, trusted, edit_date, user_id) "
                    + " VALUES ( "
                    + " '{0}', '{1}', '{2}', '{3}', {4}, {5}, "
                    + " {6}, {7}, '{8}', {9},  now(), {10}) "
                    + " RETURNING id ",
                    el.name, el.label, el.description, el.info_string, el.weight, PGDateString(el.start_date),
                    PGDateString(el.end_date), el.element_type.id, "el.text", el.trusted, 5);
                // el.text будет помещаться с обработкой позднее
                // эту же обработку надо и для других сложных полей в инфо, например
                NpgsqlCommand command = new NpgsqlCommand(query.ToString(), conn);
                count = (int)command.ExecuteScalar();
                //Console.Write("{0}\n", count);
                conn.Close();
            }
            catch (Exception ex)
            {
                return -1;
            }
            return count;
        }

        public static string PGDateString(DateTime d)
        {
            if (d == null) return "null";
            return "'" + d.ToString("u") + "'";
        }
        public static int BDInsertInfo(int el_id, HMInfoField eif)
        {
            // поместить HMInfoField в БД
            int count = -1;
            //if (elf. element_type == null)
            if (eif.field_id == 0)
                return -3;
            NpgsqlConnection conn = new NpgsqlConnection(NpgsqlConnectionString);
            try
            {
                conn.Open();
                StringBuilder query = new StringBuilder();
                query.AppendFormat(" INSERT INTO public.hm_element_info( "
                    + " element_id, field_id, date_value, int_value, float_value, "
                    + " string_value, target_element_id, start_date, end_date, "
                    + " trusted, edit_date, user_id, multivalue_col) "
                    + " VALUES ( "
                    + " {0}, {1}, {2}, {3}, {4}, "
                    + " '{5}', {6}, {7}, {8}, "
                    + " {9},  now(), {10}, {11}) "
                    + " RETURNING id ",
                    el_id, eif.field_id, PGDateString(eif.date_value), eif.int_value, eif.float_value, 
                    eif.string_value, (eif.target_element_id > 0 ? eif.target_element_id.ToString() : "null"), PGDateString(eif.start_date), PGDateString(eif.end_date), 
                    eif.trusted, 5, eif.multivalue_col);
                NpgsqlCommand command = new NpgsqlCommand(query.ToString(), conn);
                count = (int)command.ExecuteScalar();
                //Console.Write("{0}\n", count);
                conn.Close();
            }
            catch (Exception ex)
            {
                return -1;
            }
            return count;
        }
    }


    public class WikiPage
    {
        public int page_id;
        public string page_title;
        public int page_is_redirect;
        public int page_len;
        public string page_content_model;
        public string page_lang;
        public int page_latest;
        public int rev_text_id;
        public string page_text = "";
        public int hmid;
        // alter table page add column hmid integer;

        public WikiPage(string page_title, bool multiple = false)
        {
            MySql.Data.MySqlClient.MySqlConnection conn;
            try
            {
                conn = new MySql.Data.MySqlClient.MySqlConnection(MySQLDB.realwikiConnectionString); // myConnectionString
                conn.Open();
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendFormat(" SELECT page_id, page_title, page_is_redirect, page_len, page_content_model, page_lang, page_latest, rev_text_id, old_text "
                        //                + ", hmid "
                                     + " FROM {0}page AS p "
                                     + " LEFT JOIN {0}revision AS r ON p.page_latest = r.rev_id "
                                     + " LEFT JOIN {0}text AS t ON r.rev_text_id = t.old_id "
                                     + " WHERE page_namespace = 0 "//AND page_is_redirect = 0 "
                                     + "   AND (FALSE "
                                     + "   AND page_title = {1}) "
                                     , MySQLDB.DBPrefix, page_title);
                    MySqlCommand myCommand = new MySqlCommand(query.ToString(), conn);
                    MySqlDataReader reader = myCommand.ExecuteReader();
                    if (reader.Read())
                    {
                        Read(reader, false);
                    }
                    reader.Close();
                }

                //MessageBox.Show("MySQL Connection Open ! ");
                conn.Close();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error : " + ex.Message);
            }
        }

        public void Read(MySqlDataReader rdr, bool TakeBlob = false)
        {
            page_id = rdr.GetInt32(rdr.GetOrdinal("page_id"));
            page_title = rdr[rdr.GetOrdinal("page_title")].ToString();
            page_is_redirect = rdr.GetInt32(rdr.GetOrdinal("page_is_redirect"));
            page_len = rdr.GetInt32(rdr.GetOrdinal("page_len"));
            page_content_model = rdr[rdr.GetOrdinal("page_content_model")].ToString();
            page_lang = rdr[rdr.GetOrdinal("page_lang")].ToString();
            page_latest = rdr.GetInt32(rdr.GetOrdinal("page_latest"));
            rev_text_id = rdr.GetInt32(rdr.GetOrdinal("rev_text_id"));
            if (TakeBlob)
            {
                int blobcol = rdr.GetOrdinal("old_text");
                if (!rdr.IsDBNull(blobcol))
                {
                    long buflen = rdr.GetBytes(rdr.GetOrdinal("old_text"), 0, null, 0, 0);
                    Byte[] buf = new Byte[buflen];
                    int buflen2 = (int)rdr.GetBytes(rdr.GetOrdinal("old_text"), 0, buf, 0, (int)buflen);
                    page_text = Encoding.UTF8.GetString(buf, 0, buflen2);
                }
            }
        }

        public WikiPage(MySqlDataReader rdr, bool TakeBlob = false)
        {
            Read(rdr, TakeBlob);
        }
    }

    public class StringIntItem {
        public string Name;
        public int Val;

        public StringIntItem(string name, int val) {
            Name = name;
            Val = val;
        }
        public StringIntItem(string name) {
            Name = name;
            Val = 0;
        }
    }
}

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
            + ", hmid "
            + " FROM {0}page AS p "
            + " LEFT JOIN {0}revision AS r ON p.page_latest = r.rev_id "
            + " LEFT JOIN {0}text AS t ON r.rev_text_id = t.old_id "
            + " WHERE page_namespace = 0 "//AND page_is_redirect = 0 "
            + "   AND (FALSE "
            , MySQLDB.DBPrefix);
                foreach (string s in page_titles) {
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
                    lst.Add(new WikiPage(reader, true));
                conn.Close();
            } catch (Exception ex) {
                return null;
            }
            return lst;
        }

        public static List<WikiPage> ReadLinks(int frompageid, string[] page_titles, bool multiple = false, int limit = 0)
        {
            var lst = new List<WikiPage>();
            MySql.Data.MySqlClient.MySqlConnection conn;
            try
            {
                conn = new MySql.Data.MySqlClient.MySqlConnection(MySQLDB.realwikiConnectionString); // myConnectionString
                conn.Open();
                StringBuilder query = new StringBuilder();
                query.AppendFormat(" SELECT pl.pl_from, pl.pl_from_namespace, pl.pl_namespace, pl.pl_title, "
                                 + " p.page_id, p.page_title, p.page_is_redirect, p.page_len, p.page_content_model, p.page_lang, p.page_latest, "
                                 + " rev_text_id, old_text "
                                 + ", hmid "
                                 + " FROM {0}pagelinks AS pl "
                    //                                     + " LEFT JOIN {0}page AS p2 ON pl2.pl_title = p2.page_title "
                    //                                     + " LEFT JOIN {0}pagelinks AS pl ON "
                    //                        + "   ( p2.page_is_redirect=0 AND pl.pl_from = pl2.pl_from AND pl.pl_title = pl2.pl_title AND pl.pl_namespace = 0) "
                    //                        + "   OR ( p2.page_is_redirect=1 AND pl.pl_from = p2.page_id ) "
                                 + " LEFT JOIN {0}page AS p ON pl.pl_title = p.page_title AND pl.pl_namespace = p.page_namespace "
                                 + " LEFT JOIN {0}revision AS r ON p.page_latest = r.rev_id "
                                 + " LEFT JOIN {0}text AS t ON r.rev_text_id = t.old_id "
                                 + " WHERE pl.pl_from = {1} AND pl.pl_namespace = 0 AND (FALSE ", MySQLDB.DBPrefix, frompageid.ToString());
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
                    lst.Add(new WikiPage(reader, true));
                conn.Close();
            }
            catch (Exception ex)
            {
                return null;
            }
            return lst;
        }

        public static WikiPage ReadRedirect(int frompageid)
        {
            MySql.Data.MySqlClient.MySqlConnection conn;
            try
            {
                conn = new MySql.Data.MySqlClient.MySqlConnection(MySQLDB.realwikiConnectionString); // myConnectionString
                conn.Open();
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendFormat(" SELECT pl.pl_from, pl.pl_from_namespace, pl.pl_namespace, pl.pl_title, "
                                     + " p.page_id, p.page_title, 1 as page_is_redirect, p.page_len, p.page_content_model, p.page_lang, p.page_latest, "
                                     + " rev_text_id, old_text "
                                     + ", p.hmid "
                                     + " FROM {0}pagelinks AS pl "
                                     + " LEFT JOIN {0}page AS p ON pl.pl_title = p.page_title AND pl.pl_namespace = p.page_namespace "
                                     + " LEFT JOIN {0}revision AS r ON p.page_latest = r.rev_id "
                                     + " LEFT JOIN {0}text AS t ON r.rev_text_id = t.old_id "
                                     + " WHERE pl.pl_from = {1} ", MySQLDB.DBPrefix, frompageid.ToString());
                    query.AppendFormat(" LIMIT 1");
                    MySqlCommand myCommand = new MySqlCommand(query.ToString(), conn);
                    MySqlDataReader reader = myCommand.ExecuteReader();
                    if (!reader.Read())
                        throw new Exception("Bad Redirect: " + frompageid.ToString());
                    return new WikiPage(reader, true);
                }

                //MessageBox.Show("MySQL Connection Open ! ");
                conn.Close();
            } catch (Exception ex) {
                return null;
                //MessageBox.Show("Error : " + ex.Message);
            }
            return null;
        }

        public static int UpdateHMID(int pageid, int hmid, bool recursive = false, string pagetitle = "")
        {
            //int pageid = el.id;
            MySql.Data.MySqlClient.MySqlConnection conn;
            try
            {
                conn = new MySql.Data.MySqlClient.MySqlConnection(MySQLDB.realwikiConnectionString); // myConnectionString
                conn.Open();
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendFormat(" UPDATE {0}page SET hmid = {2} WHERE page_id = {1}",
                        MySQLDB.DBPrefix, pageid.ToString(), hmid.ToString());
                    MySqlCommand myCommand = new MySqlCommand(query.ToString(), conn);
                    int res = myCommand.ExecuteNonQuery();
                    if (res <= 0)
                        throw new Exception("Не получилось обновить: " + pageid.ToString());

                    if (recursive) {
                        query = new StringBuilder();
                        query.AppendFormat(" UPDATE {0}page "
                                + " join {0}pagelinks on pl_from_namespace = page_namespace and pl_from = page_id "
                                + " SET hmid = {2} "
                                + " where pl_namespace = 0 and pl_title = {1} and page_is_redirect=1",
                                MySQLDB.DBPrefix, PGDB.PGString(pagetitle), hmid.ToString());
                        myCommand = new MySqlCommand(query.ToString(), conn);
                        int res2 = myCommand.ExecuteNonQuery();
                        if (res <= 0)
                            throw new Exception("Не получилось обновить пересылки: " + pageid.ToString());
                    }

                    return res;
                }

                //MessageBox.Show("MySQL Connection Open ! ");
                conn.Close();
            }
            catch (Exception ex)
            {
                return -2;
                //MessageBox.Show("Error : " + ex.Message);
            }
        }

    }

    public class PGDB
    {
        public static string NpgsqlConnectionString = "Server=localhost;User Id=postgres;Password=123;Database=hm;";
        
        public static string PGString(string s){
            if (s == null)
                return "null";
            else
                return "'" + s.Replace("'''", "").Replace("''", "").Replace("'", "''") + "'";
        }

        public static int BDSetElementType(int elid, int eltypeid)
        {
            // поставить тип элемента
            int res = 0;
            if (eltypeid == 0) return -3;
            NpgsqlConnection conn = new NpgsqlConnection(NpgsqlConnectionString);
            try
            {
                conn.Open();
                StringBuilder query = new StringBuilder();
                query.AppendFormat(" INSERT INTO public.hm_element_type_links( "
                    + " element_id, element_type_id ) "
                    + " VALUES ({0}, {1}) "
                    + " RETURNING id ",
                    elid, eltypeid);
                NpgsqlCommand command = new NpgsqlCommand(query.ToString(), conn);
                res = (int)command.ExecuteScalar();
                conn.Close();
            }
            catch (Exception ex)
            {
                return -1;
            }
            return res;
        }

        public static int BDInsertElement(HMElement el)
        {
            // поместить Element в БД
            // только инсерт, возвр ел.ид или -1, возм если ел.ид уже есть - делает апдейт (потом)
            int newhmid = -1;
            if (el.element_type == null)
                return -3;
            NpgsqlConnection conn = new NpgsqlConnection(NpgsqlConnectionString);
            try
            {
                conn.Open();
                StringBuilder query = new StringBuilder();
                query.AppendFormat(" INSERT INTO public.hm_elements( "
                    + " WikiID, "
                    + " name, label, description, info, weight, start_date, "
                    + " end_date, "
                    //+ " element_type_id, "
                    + " text, trusted, edit_date, user_id) "
                    + " VALUES ( "
                    + " {10}, " // + " {11}, "
                    + " {0}, {1}, {2}, {3}, {4}, {5}, "
                    + " {6}, {7}, {8}, now(), {9}) "
                    + " RETURNING id ",
                    PGString(el.name), PGString(el.label), PGString(el.description), "null", el.weight, PGDateString(el.start_date),
                    PGDateString(el.end_date), //el.element_type.id, 
                    PGString(el.text), el.trusted, 5
                    , el.WikiId);
                // el.text будет помещаться с обработкой позднее
                // эту же обработку надо и для других сложных полей в инфо, например
                NpgsqlCommand command = new NpgsqlCommand(query.ToString(), conn);
                newhmid = (int)command.ExecuteScalar();
                //Console.Write("{0}\n", count);
                conn.Close();
                BDSetElementType(newhmid, el.element_type.id);
            }
            catch (Exception ex)
            {
                return -1;
            }
            var res = MySQLDB.UpdateHMID(el.WikiId, newhmid, true, el.name); // рекурсивно обновить hmid в БД вики
            if (res <= 0)
                return res - 10;
            return newhmid;
        }

        public static string PGDateString(DateTime d)
        {
            if (d == null || d == DateTime.MinValue) return "null";
            return "'" + d.ToString("u") + "'";
        }
        public static int BDInsertInfo(int el_id, HMInfoField eif)
        {
            // поместить HMInfoField в БД
            int newid = -1;
            //if (elf. element_type == null)
            if (eif == null) // пока такого вообще не может быть
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
                    + " {5}, {6}, {7}, {8}, "
                    + " {9},  now(), {10}, {11}) "
                    + " RETURNING id ",
                    el_id, eif.field_type.id, PGDateString(eif.date_value), eif.int_value, eif.float_value, 
                    PGString(eif.string_value), (eif.target_element_id > 0 ? eif.target_element_id.ToString() : "null"), PGDateString(eif.start_date), PGDateString(eif.end_date), 
                    eif.trusted, 5, eif.multivalue_col);
                NpgsqlCommand command = new NpgsqlCommand(query.ToString(), conn);
                newid = (int)command.ExecuteScalar();
                //Console.Write("{0}\n", count);
                conn.Close();
            }
            catch (Exception ex)
            {
                return -1;
            }
            return newid;
        }


        public static int BDInsertElementLink(HMElementLink link)
        {
            // поместить линк в БД
            // только инсерт, возвр ел.ид или -1, возм если ел.ид уже есть - делает апдейт (потом)
            int count = -1;
            if (link == null || link.a == null || link.b_id == 0)
                return -3;
            NpgsqlConnection conn = new NpgsqlConnection(NpgsqlConnectionString);
            try
            {
                conn.Open();
                StringBuilder query = new StringBuilder();
                query.AppendFormat("INSERT INTO public.hm_element_links( "
                    + " a_element_id, b_element_id, semantic_link_type_id, "
                    + " weight, link_start_date, link_end_date) "
                    + " VALUES ( "
                    + " {0}, {1}, {2}, "
                    + " {3}, {4}, {5} ) "
                    + " RETURNING id ",
                    link.a.id, link.b_id, link.link_type.id, 
                    link.weight, PGDateString(link.start_date), PGDateString(link.end_date)
                );
                NpgsqlCommand command = new NpgsqlCommand(query.ToString(), conn);
                count = (int)command.ExecuteScalar();
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

        public WikiPage(string page_title, bool TakeBlob = false)
        {
            MySql.Data.MySqlClient.MySqlConnection conn;
            try
            {
                conn = new MySql.Data.MySqlClient.MySqlConnection(MySQLDB.realwikiConnectionString); // myConnectionString
                conn.Open();
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendFormat(" SELECT page_id, page_title, page_is_redirect, page_len, page_content_model, page_lang, page_latest, rev_text_id, old_text "
                                     + ", hmid "
                                     + " FROM {0}page AS p "
                                     + " LEFT JOIN {0}revision AS r ON p.page_latest = r.rev_id "
                                     + " LEFT JOIN {0}text AS t ON r.rev_text_id = t.old_id "
                                     + " WHERE page_namespace = 0 "//AND page_is_redirect = 0 "
                                     + "   AND (FALSE "
                                     + "   OR page_title = {1}) "
                                     , MySQLDB.DBPrefix, PGDB.PGString(page_title).Replace(' ', '_'));
                    MySqlCommand myCommand = new MySqlCommand(query.ToString(), conn);
                    MySqlDataReader reader = myCommand.ExecuteReader();
                    if (reader.Read())
                    {
                        Read(reader, TakeBlob);
                    }
                    reader.Close();
                    if (hmid > 0) return; // если hmid - дальше спускаться не нужно
                    if (page_is_redirect > 0) {
                        var wp2 = MySQLDB.ReadRedirect(page_id);
                        if (wp2.hmid > 0) { // тут надо апдейтнуть и дальше не обрабатывать
                            hmid = wp2.hmid;  // (причем на 1 уровень пересылок - вики не поддерживает больше) 
                            MySQLDB.UpdateHMID(page_id, hmid);
                            return;
                        }
                        CopyWikiPage(wp2); // подменяем редиректом
                    }
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
            page_text = "";
            int hmidcol = rdr.GetOrdinal("hmid");
            hmid = (rdr.IsDBNull(hmidcol)? 0: rdr.GetInt32(hmidcol));

            if (TakeBlob && hmid == 0) // если hmid - дальше спускаться не нужно
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

        public void CopyWikiPage(WikiPage wp2)
        {
            page_id = wp2.page_id;
            page_title = wp2.page_title;
            page_is_redirect = wp2.page_is_redirect;
            page_len = wp2.page_len;
            page_content_model = wp2.page_content_model;
            page_lang = wp2.page_lang;
            page_latest = wp2.page_latest;
            rev_text_id = wp2.rev_text_id;
            page_text = wp2.page_text;
            hmid = wp2.hmid;
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

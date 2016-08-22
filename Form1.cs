using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using MySql.Data;
using Npgsql;
using MySql.Data.MySqlClient;

namespace DBproc
{
    public partial class Form1 : Form
    {

       // string myConnectionString = "server=localhost;uid=root;pwd=;database=dbwiki;";

        public Form1()
        {
            InitializeComponent();
        }


        public int PgProcess(MySqlDataReader rdr, int depth, int tops, string[] words)
        {
            string innerstrs = "";
            MySql.Data.MySqlClient.MySqlConnection conn, conn2;
            conn2 = new MySql.Data.MySqlClient.MySqlConnection(MySQLDB.realwikiConnectionString); // myConnectionString                    
            string page_title = rdr[rdr.GetOrdinal("page_title")].ToString();
            string[] row0 = { };

            if (rdr.GetInt16(rdr.GetOrdinal("page_is_redirect")) > 0)
            { 
                try
                {
                    conn2.Open();
                    StringBuilder query = new StringBuilder();
                    query.AppendFormat(" SELECT pl.pl_from, pl.pl_from_namespace, pl.pl_namespace, pl.pl_title, "
                                     + " p.page_id, p.page_title, 1 as page_is_redirect, p.page_len, p.page_content_model, p.page_lang, p.page_latest, "
                                     + " rev_text_id, old_text "
                                     + " FROM {0}pagelinks AS pl "
                                     + " LEFT JOIN {0}page AS p ON pl.pl_title = p.page_title AND pl.pl_namespace = p.page_namespace "
                                     + " LEFT JOIN {0}revision AS r ON p.page_latest = r.rev_id "
                                     + " LEFT JOIN {0}text AS t ON r.rev_text_id = t.old_id "
                                     + " WHERE pl.pl_from = {1} ", MySQLDB.DBPrefix, rdr[rdr.GetOrdinal("page_id")].ToString());
                    query.AppendFormat(" LIMIT 1"); 
                    MySqlCommand myCommand = new MySqlCommand(query.ToString(), conn2);
                    MySqlDataReader reader = myCommand.ExecuteReader();
                    if (!reader.Read())
                        throw new Exception("Bad Redirect: " + rdr[rdr.GetOrdinal("page_id")].ToString());
                    rdr = reader; // подмена                    
                }
                catch (Exception ex)
                {
                    dgw.Rows.Add(new string[] { "", page_title, "НЕ РАСКРЫЛСЯ РЕДИРЕКТ: " + ex.Message });
                    return 4; 
                }
            }

            int blobcol = rdr.GetOrdinal("old_text");
            string page_text = "";
            if (!rdr.IsDBNull(blobcol)) // если статьи нет - обрабатывать нет смысла
            {
                long buflen = rdr.GetBytes(rdr.GetOrdinal("old_text"), 0, null, 0, 0);
                Byte[] buf = new Byte[buflen];
                int buflen2 = (int)rdr.GetBytes(rdr.GetOrdinal("old_text"), 0, buf, 0, (int)buflen);
                page_text = Encoding.UTF8.GetString(buf, 0, buflen2);

                page_title = rdr[rdr.GetOrdinal("page_title")].ToString();

                //page_text.Contains("#REDIRECT") || page_text.Contains("#перенаправление"))

                var el = ParserM.AlphaParser(page_text, page_title, "Вооружённый конфликт");
                if (el != null) {
                    dgw.Rows.Add(new string[] { "", page_title, "не " + "Вооружённый конфликт" });
//                    row0 = { "", orig_page_title, "не " + "Вооружённый конфликт" };
  //                  dgw.Rows.Add(row0);
                    return 3;
                }

                if (depth > 0)
                {
                    try
                    {
                        conn = new MySql.Data.MySqlClient.MySqlConnection(MySQLDB.realwikiConnectionString); // myConnectionString                    
                        conn.Open();

                        StringBuilder query = new StringBuilder();
                        query.AppendFormat(" SELECT pl.pl_from, pl.pl_from_namespace, pl.pl_namespace, pl.pl_title, "
                                         + " p.page_id, p.page_title, p.page_is_redirect, p.page_len, p.page_content_model, p.page_lang, p.page_latest, "
                                         + " rev_text_id, old_text "
                                         + " FROM {0}pagelinks AS pl "
                            //                                     + " LEFT JOIN {0}page AS p2 ON pl2.pl_title = p2.page_title "
                            //                                     + " LEFT JOIN {0}pagelinks AS pl ON "
                            //                        + "   ( p2.page_is_redirect=0 AND pl.pl_from = pl2.pl_from AND pl.pl_title = pl2.pl_title AND pl.pl_namespace = 0) "
                            //                        + "   OR ( p2.page_is_redirect=1 AND pl.pl_from = p2.page_id ) "
                                         + " LEFT JOIN {0}page AS p ON pl.pl_title = p.page_title AND pl.pl_namespace = p.page_namespace "
                                         + " LEFT JOIN {0}revision AS r ON p.page_latest = r.rev_id "
                                         + " LEFT JOIN {0}text AS t ON r.rev_text_id = t.old_id "
                                         + " WHERE pl.pl_from = {1} AND pl.pl_namespace = 0 AND (FALSE ", MySQLDB.DBPrefix, rdr[rdr.GetOrdinal("page_id")].ToString());
                        foreach (string s in words)
                        {
                            query.AppendFormat(" OR REPLACE(LOWER(p.page_title),'_',' ') REGEXP '[[:<:]]{0}[[:>:]]' ", s);
                        }
                        query.AppendFormat(" ) LIMIT {0}", tops);
                        MySqlCommand myCommand = new MySqlCommand(query.ToString(), conn);
                        MySqlDataReader reader = myCommand.ExecuteReader();
                        while (reader.Read())
                        {
                            tbOut.Text += page_title + " -> "
                                + reader[reader.GetOrdinal("page_title")].ToString() + Environment.NewLine;
                            innerstrs += reader[reader.GetOrdinal("page_title")].ToString() + Environment.NewLine;
                            PgProcess(reader, depth - 1, tops, words);
                        }
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        innerstrs += "Ошибка: " + ex.Message;
                        //return -1;
                    }
                }
                //                return 2;

                dgw.Rows.Add(new string [] {
                                rdr[rdr.GetOrdinal("page_id")].ToString(),
                                page_title,
                                rdr[rdr.GetOrdinal("page_is_redirect")].ToString(),
                                rdr[rdr.GetOrdinal("page_len")].ToString(),
                                rdr[rdr.GetOrdinal("page_content_model")].ToString(),
                                rdr[rdr.GetOrdinal("page_lang")].ToString(),
                                rdr[rdr.GetOrdinal("page_latest")].ToString(),
                                rdr[rdr.GetOrdinal("rev_text_id")].ToString(),
                                page_text,
                                innerstrs
                            });
            }
            else {
                dgw.Rows.Add(new string [] { "", page_title, "статья пустая" });
            }

            if (conn2.State == ConnectionState.Open)
                conn2.Close();
            return 1;
        }    
        private void button1_Click(object sender, EventArgs e)
        {
            HMData.ff = this; // для вывода
            int i2 = 0;
            try {
                i2 = int.Parse(tbSize.Text);
            }
            catch(Exception ex) {}

            string[] selstrs1 = { "война" };
            string[] selstrs2 = { "сражение", "битва" };
            MySql.Data.MySqlClient.MySqlConnection conn;
            try
            {
                conn = new MySql.Data.MySqlClient.MySqlConnection(MySQLDB.realwikiConnectionString); // myConnectionString
                conn.Open();
                StringBuilder query = new StringBuilder();
                query.AppendFormat(" SELECT page_id, page_title, page_is_redirect, page_len, page_content_model, page_lang, page_latest, rev_text_id, old_text "
                                 + " FROM {0}page AS p "
                                 + " LEFT JOIN {0}revision AS r ON p.page_latest = r.rev_id "
                                 + " LEFT JOIN {0}text AS t ON r.rev_text_id = t.old_id "
                                 + " WHERE page_namespace = 0 "//AND page_is_redirect = 0 "
                                 + "   AND (FALSE "
                    //         + " AND  (page_title REGEXP 'война' OR page_title REGEXP 'Война')"
                                 , MySQLDB.DBPrefix);
                foreach (string s in selstrs1)
                {
                    query.AppendFormat(" OR REPLACE(LOWER(page_title),'_',' ') REGEXP '[[:<:]]{0}[[:>:]]' ", s);
                }
                query.AppendFormat(" ) LIMIT {0}", i2);
                MySqlCommand myCommand = new MySqlCommand(query.ToString(), conn);
                MySqlDataReader reader = myCommand.ExecuteReader();
                while (reader.Read())
                {
//                        tbOut.Text += reader[reader.GetOrdinal("page_title")].ToString();
                    if (PgProcess(reader, 1, 10, selstrs2) <= 0)
                    {
                        throw new Exception("Broken Page on: " + reader[reader.GetOrdinal("page_title")].ToString());
                    }
                }
//                reader.Close();
                MessageBox.Show("MySQL Connection Open ! ");
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                NpgsqlConnection conn = new NpgsqlConnection(PGDB.NpgsqlConnectionString);
                conn.Open();
                MessageBox.Show ("PG Connection Open ! ");
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open connection ! " + ex.Message);
            }
            
        /*NpgsqlCommand command = new NpgsqlCommand("SELECT COUNT(*) FROM cities", conn);
        Int64 count = (Int64)command.ExecuteScalar();
        Console.Write("{0}\n", count);
            conn.Close();*/
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XmlReceiptReader
{
    class SqlDriver
    {
        public void CreateDatabase()
        {
            try
            {
                SQLiteConnection sqlite_conn;
                SQLiteCommand sqlite_cmd;

                if (!File.Exists(Program.DB_PATH))
                {
                    sqlite_conn = new SQLiteConnection("Data Source=" + Program.DB_PATH + ";Version=3;New=True;Compress=True;");
                    sqlite_conn.Open();
                    sqlite_cmd = sqlite_conn.CreateCommand();

                    sqlite_cmd.CommandText = SQL.CreateDatabaseScript();
                    sqlite_cmd.ExecuteNonQuery();
                }
                else {
                    sqlite_conn = new SQLiteConnection("Data Source=" + Program.DB_PATH + ";Version=3;New=False;Compress=True;");
                    sqlite_conn.Open();
                    sqlite_cmd = sqlite_conn.CreateCommand();

                    sqlite_cmd.CommandText = SQL.CLearDBScript();
                    sqlite_cmd.ExecuteNonQuery();
                }
                sqlite_conn.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + Environment.NewLine + e.StackTrace, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string[] Get(string name)
        {
            SQLiteConnection sqlite_conn;
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;

            sqlite_conn = new SQLiteConnection("Data Source=" + Program.DB_PATH + ";Version=3;New=False;Compress=True;");
            sqlite_conn.Open();
            sqlite_cmd = sqlite_conn.CreateCommand();

            sqlite_cmd.CommandText = "SELECT * FROM environment WHERE name='" + name + "' LIMIT 1";
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            string[] results = new string[2];
            while (sqlite_datareader.Read())
            {
                results[0] = sqlite_datareader["value"].ToString();
                results[1] = sqlite_datareader["enabled"].ToString();
            }
            sqlite_conn.Close();
            return results;
        }

        public void Update(string name, string value, string enabled)
        {
            SQLiteConnection sqlite_conn;
            SQLiteCommand sqlite_cmd;

            sqlite_conn = new SQLiteConnection("Data Source=" + Program.DB_PATH + ";Version=3;New=False;Compress=True;");
            sqlite_conn.Open();
            sqlite_cmd = sqlite_conn.CreateCommand();

            sqlite_cmd.CommandText = "UPDATE environment SET value='" + value + "', enabled='" + enabled + "' WHERE name='" + name + "'";
            sqlite_cmd.ExecuteNonQuery();
            sqlite_conn.Close();
        }

        public int GetRowCount(string table, string condition = "1")
        {
            SQLiteConnection sqlite_conn;
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;

            sqlite_conn = new SQLiteConnection("Data Source=" + Program.DB_PATH + ";Version=3;New=False;Compress=True;");
            sqlite_conn.Open();
            sqlite_cmd = sqlite_conn.CreateCommand();

            sqlite_cmd.CommandText = "SELECT COUNT(*) FROM " + table + " WHERE " + condition;
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            string count = "0";
            while (sqlite_datareader.Read())
            {
                count = sqlite_datareader[0].ToString();
            }
            sqlite_conn.Close();
            int intCount = Convert.ToInt32(count);

            return intCount;
        }

        public int GetRowCountForItems(string receiptid)
        {
            SQLiteConnection sqlite_conn;
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;

            sqlite_conn = new SQLiteConnection("Data Source=" + Program.DB_PATH + ";Version=3;New=False;Compress=True;");
            sqlite_conn.Open();
            sqlite_cmd = sqlite_conn.CreateCommand();

            sqlite_cmd.CommandText = "SELECT COUNT(*) FROM items WHERE receiptid='" + receiptid + "'";
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            string count = "0";
            while (sqlite_datareader.Read())
            {
                count = sqlite_datareader[0].ToString();
            }
            sqlite_conn.Close();
            int intCount = Convert.ToInt32(count);

            return intCount;
        }

        public string InsertReceipt(Dictionary<string, string> data)
        {
            SQLiteConnection sqlite_conn;
            SQLiteCommand sqlite_cmd;

            DateTime date = DateTime.Now;
            string dateformated = date.ToString("dd.MM.yyyy HH:mm:ss");

            sqlite_conn = new SQLiteConnection("Data Source=" + Program.DB_PATH + ";Version=3;New=False;Compress=True;");
            sqlite_conn.Open();
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = "INSERT INTO receipts (uid, date, amount, receiptnumber, intreceiptnumber, date, mod, paycard, paycash, paystr, payvmp, xmldata, receipttype, invoicenumber, taxbasebasic, taxbasereduced, basicvatamount, reducedvatamount, taxfreeamount) VALUES ('" + data["uid"] + "', '" + data["date"] + "', '" + data["amount"] + "', '" + data["receiptnumber"] + "', '" + data["intreceiptnumber"] + "', '" + dateformated + "', '" + data["mod"] + "', '" + data["paycard"] + "', '" + data["paycash"] + "', '" + data["paystr"] + "', '" + data["payvmp"] + "', '" + data["xmldata"] + "', '" + data["receipttype"] + "', '" + data["invoicenumber"] + "', '" + data["taxbasebasic"] + "', '" + data["taxbasereduced"] + "', '" + data["basicvatamount"] + "', '" + data["reducedvatamount"] + "', '" + data["taxfreeamount"] + "');" +
                "SELECT last_insert_rowid();";
            var id = sqlite_cmd.ExecuteScalar();
            sqlite_conn.Close();
            return id.ToString();
        }

        public void InsertReceiptItems(string[,] items, string receiptid)
        {
            SQLiteConnection sqlite_conn;
            SQLiteCommand sqlite_cmd;

            sqlite_conn = new SQLiteConnection("Data Source=" + Program.DB_PATH + ";Version=3;New=False;Compress=True;");
            sqlite_conn.Open();
            sqlite_cmd = sqlite_conn.CreateCommand();

            for (int index = 0; index < items.GetLength(0); index++)
            {
                sqlite_cmd.CommandText = "INSERT INTO items (receiptid, name, mj, unitprice, price, vatrate, quantity, refid) VALUES ('" + receiptid + "', '" + items[index, 0] + "', '" + items[index, 7] + "', '" + items[index, 4] + "', '" + items[index, 5] + "', '" + items[index, 3] + "', '" + items[index, 2] + "', '" + items[index, 6] + "')";
                sqlite_cmd.ExecuteNonQuery();
            }
            sqlite_conn.Close();
        }

        public string[,] GetAllReceipts()
        {
            SQLiteConnection sqlite_conn;
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;

            sqlite_conn = new SQLiteConnection("Data Source=" + Program.DB_PATH + ";Version=3;New=False;Compress=True;");
            sqlite_conn.Open();
            sqlite_cmd = sqlite_conn.CreateCommand();

            sqlite_cmd.CommandText = "SELECT * FROM receipts ORDER BY id DESC";
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            string[,] results = new string[this.GetRowCount("receipts"), 18];
            int index = 0;
            while (sqlite_datareader.Read())
            {
                results[index, 0] = sqlite_datareader["id"].ToString();
                results[index, 1] = sqlite_datareader["uid"].ToString();
                results[index, 2] = sqlite_datareader["amount"].ToString();
                results[index, 3] = sqlite_datareader["receiptnumber"].ToString();
                results[index, 4] = sqlite_datareader["intreceiptnumber"].ToString();
                results[index, 5] = sqlite_datareader["date"].ToString();
                results[index, 6] = sqlite_datareader["mod"].ToString();
                results[index, 7] = sqlite_datareader["paycard"].ToString();
                results[index, 8] = sqlite_datareader["paycash"].ToString();
                results[index, 9] = sqlite_datareader["paystr"].ToString();
                results[index, 10] = sqlite_datareader["payvmp"].ToString();
                results[index, 11] = sqlite_datareader["receipttype"].ToString();
                results[index, 12] = sqlite_datareader["invoicenumber"].ToString();
                results[index, 13] = sqlite_datareader["taxbasebasic"].ToString();
                results[index, 14] = sqlite_datareader["taxbasereduced"].ToString();
                results[index, 15] = sqlite_datareader["basicvatamount"].ToString();
                results[index, 16] = sqlite_datareader["reducedvatamount"].ToString();
                results[index, 17] = sqlite_datareader["taxfreeamount"].ToString();

                index++;
            }
            sqlite_conn.Close();
            return results;
        }
    
        public string[,] GetItems(string receiptid)
        {
            SQLiteConnection sqlite_conn;
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;

            sqlite_conn = new SQLiteConnection("Data Source=" + Program.DB_PATH + ";Version=3;New=False;Compress=True;");
            sqlite_conn.Open();
            sqlite_cmd = sqlite_conn.CreateCommand();

            sqlite_cmd.CommandText = "SELECT * FROM items WHERE receiptid='" + receiptid + "'";
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            string[,] results = new string[this.GetRowCountForItems(receiptid), 8];

            int index = 0;
            while (sqlite_datareader.Read())
            {
                results[index, 0] = sqlite_datareader["id"].ToString();
                results[index, 1] = sqlite_datareader["name"].ToString();
                results[index, 2] = sqlite_datareader["unitprice"].ToString();
                results[index, 3] = sqlite_datareader["price"].ToString();
                results[index, 4] = sqlite_datareader["vatrate"].ToString();
                results[index, 5] = sqlite_datareader["quantity"].ToString();
                results[index, 6] = sqlite_datareader["refid"].ToString();
                results[index, 7] = sqlite_datareader["mj"].ToString();
                index++;
            }
            sqlite_conn.Close();
            return results;
        }

        public void ExportDatabase(string Separator, string path)
        {
            SQLiteConnection sqlite_conn;
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;

            StreamWriter outputFile = new StreamWriter(path);

            sqlite_conn = new SQLiteConnection("Data Source=" + Program.DB_PATH + ";Version=3;New=False;Compress=True;");
            sqlite_conn.Open();

            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM receipts";
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            while (sqlite_datareader.Read())
            {
                string headers = String.Empty;
                for (int x = 0; x < sqlite_datareader.FieldCount; x++)
                {
                    headers += sqlite_datareader.GetName(x).ToString();

                    if (x != sqlite_datareader.FieldCount - 1)
                        headers += Separator;
                }

                outputFile.WriteLine(headers);

                string line = String.Empty;
                for (int x = 0; x < sqlite_datareader.FieldCount; x++)
                {
                    line += sqlite_datareader[x].ToString();

                    if (x != sqlite_datareader.FieldCount - 1)
                        line += Separator;
                }

                outputFile.WriteLine(line);
                outputFile.WriteLine("Položky");
                outputFile.WriteLine("---------");

                SQLiteCommand sqlite_cmd2 = sqlite_conn.CreateCommand();
                sqlite_cmd2.CommandText = "SELECT * FROM items WHERE receiptid='" + sqlite_datareader["id"].ToString() + "'";
                SQLiteDataReader sqlite_datareader2 = sqlite_cmd2.ExecuteReader();

                headers = String.Empty;
                for (int x = 0; x < sqlite_datareader2.FieldCount; x++)
                {
                    headers += sqlite_datareader2.GetName(x).ToString();

                    if (x != sqlite_datareader2.FieldCount - 1)
                        headers += Separator;
                }

                outputFile.WriteLine(headers);

                while (sqlite_datareader2.Read())
                {
                    line = String.Empty;
                    for (int x = 0; x < sqlite_datareader2.FieldCount; x++)
                    {
                        line += sqlite_datareader2[x].ToString();

                        if (x != sqlite_datareader2.FieldCount - 1)
                            line += Separator;
                    }

                    outputFile.WriteLine(line);
                }

                outputFile.WriteLine("---------");
                outputFile.WriteLine(String.Empty);
                outputFile.WriteLine(String.Empty);
            }

            sqlite_conn.Close();
            outputFile.Close();
        }
    }
}

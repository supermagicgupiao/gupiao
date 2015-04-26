using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SQLite;

namespace Stock.Controller.DBController
{
    class DBDataController
    {
        private string dbPath;
        private SQLiteConnection conn;
        public DBDataController(string s)
        {
            dbPath = s;
            conn = new SQLiteConnection();
            SQLiteConnectionStringBuilder connstr = new SQLiteConnectionStringBuilder();
            connstr.DataSource = dbPath;
            //connstr.Password = "admin";
            conn.ConnectionString = connstr.ToString();
            conn.Open();
        }
        public void DBDealList_Insert(string name,string id,string date,string type,string money,string number,string taxrate,string commission,string explain,string remark)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            string[] s = new string[] { name, id, date, type, money, number, taxrate, commission, explain, remark };
            DBTablesController.Insert(DBTablesController.table[0],ref cmd,ref s);
        }
    }
}

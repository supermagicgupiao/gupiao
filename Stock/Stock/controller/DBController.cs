using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Data.SQLite;

namespace Stock.controller
{
    class DBController
    {
        private string dbPath;
        private SQLiteConnection conn;
        DBController()
        {
            dbPath = Environment.CurrentDirectory + "/Stock.db";
        }
        DBController(string s)
        {
            dbPath = s;
        }
        public bool DBExists()
        {
            if (Directory.Exists(dbPath))
                return true;
            return false;
        }
        public bool DBConnection()
        {
            conn = new SQLiteConnection();
            SQLiteConnectionStringBuilder connstr = new SQLiteConnectionStringBuilder();
            connstr.DataSource = dbPath;
            //connstr.Password = "admin";
            conn.ConnectionString = connstr.ToString();
            conn.Open();
            return true;
        }
        public bool DBCreate()
        {
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SQLite;

namespace Stock.Controller.DBController.DBConnection
{
    class FileConnection
    {
        private static Dictionary<string, SQLiteConnection> file_conns = new Dictionary<string, SQLiteConnection>();
        public static SQLiteConnection Create(string path)
        {
            if (file_conns.ContainsKey(path)) 
            {
                return file_conns[path];
            }
            else
            {
                return CreateConnection(path);
            }
        }
        private static SQLiteConnection CreateConnection(string path)
        {
            SQLiteConnection conn = new SQLiteConnection();
            SQLiteConnectionStringBuilder connstr = new SQLiteConnectionStringBuilder();
            connstr.DataSource = path;
            //connstr.Password = "admin";
            conn.ConnectionString = connstr.ToString();
            conn.Open();
            file_conns.Add(path, conn);
            return conn;
        }
    }
}

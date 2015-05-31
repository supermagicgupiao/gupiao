using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SQLite;

namespace Stock.Controller.DBController.DBConnection
{
    class MemConnection
    {
        private static SQLiteConnection mem_conn;
        public static SQLiteConnection Create()
        {
            if (mem_conn != null)
            {
                return mem_conn;
            }
            else
            {
                return CreateConnection();
            }
        }
        private static SQLiteConnection CreateConnection()
        {
            mem_conn = new SQLiteConnection("data source=:memory:");
            mem_conn.Open();
            return mem_conn;
        }
    }
}

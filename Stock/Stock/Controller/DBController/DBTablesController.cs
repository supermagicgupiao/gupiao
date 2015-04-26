using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SQLite;

namespace Stock.Controller.DBController
{
    class DBTablesController
    {
        public static string[] table = new string[] { "DealList", };
        public static void Create(string table_name, ref SQLiteCommand cmd)
        {
            if (table_name == "DealList")
            {
                cmd.CommandText = "create table if not exists '" + table_name + "'(name varchar(8),id integer,date text,type int2,money real,number int,taxrate int,commission int,explain text,remark text)";
                cmd.ExecuteNonQuery();
            }
        }
        public static void Insert(string table_name, ref SQLiteCommand cmd, ref string[] argv)
        {
            if (table_name == "DealList")
            {
                cmd.CommandText = "insert into '" + table_name + "' values(";
                for (int i = 0; i < argv.Length - 1; i++)
                {
                    cmd.CommandText += argv[i] + ",";
                }
                cmd.CommandText += argv[argv.Length - 1] + ")";
                cmd.ExecuteNonQuery();
            }
        }
    }
}

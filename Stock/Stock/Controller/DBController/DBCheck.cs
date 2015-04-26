using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Data.SQLite;

namespace Stock.Controller.DBController
{
    class DBCheck
    {
        public string dbPath;
        private SQLiteConnection conn;
        private string[] table = DBTablesController.table;
        public DBCheck()
        {
            dbPath = Environment.CurrentDirectory + "\\Stock.db";
        }
        public DBCheck(string s)
        {
            dbPath = s;
        }
        public DB_ERROR Check()
        {
            if (DBConnection())
            {
                DB_ERROR dbe = DBCheckTable();
                if (dbe == DB_ERROR.DB_TABLE_NOT_EXISTS)
                {
                    DBCreateTable();
                    return dbe;
                }
                else if (dbe == DB_ERROR.DB_TABLE_CRACK) 
                {
                    conn.Close();
                    try{ File.Delete(dbPath);}
                    catch(Exception)
                    {
                        return dbe;
                    }
                    conn.Open();
                    DBCreateTable();
                    return DB_ERROR.DB_TABLE_CRACK_FIX;
                }
                else
                {
                    return dbe;
                }
            }
            else
            {
                return DB_ERROR.DB_CANT_CONNECT;
            }
        }
        private bool DBExists()
        {
            if (Directory.Exists(dbPath))
            {
                DBConnection();
                return true;
            }   
            return false;
        }
        private bool DBConnection()
        {
            try
            {
                conn = new SQLiteConnection();
                SQLiteConnectionStringBuilder connstr = new SQLiteConnectionStringBuilder();
                connstr.DataSource = dbPath;
                //connstr.Password = "admin";
                conn.ConnectionString = connstr.ToString();
                conn.Open();
            }
            catch (SQLiteException) 
            {
                return false;
            }
            return true;
        }
        private DB_ERROR DBCheckTable()
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            for (int i = 0; i < table.Length; i++)
            {
                try
                {
                    cmd.CommandText = "select count(*) from sqlite_master where type='table' and name='" + table[i] + "'";
                    if (0 == Convert.ToInt32(cmd.ExecuteScalar()))
                    {
                        return DB_ERROR.DB_TABLE_NOT_EXISTS;
                    }
                }
                catch (SQLiteException)
                {
                    return DB_ERROR.DB_TABLE_CRACK;
                }
            }
            return DB_ERROR.DB_OK;
        }
        private bool DBCreateTable()
        {
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(conn);
                for (int i = 0; i < table.Length; i++)
                {
                    DBTablesController.Create(table[i], ref cmd);
                }
            }
            catch (SQLiteException)
            {
                return false;
            }
            return true;
        }
    }
    enum DB_ERROR:byte
    {
        DB_OK,
        DB_CANT_CONNECT,
        DB_TABLE_NOT_EXISTS,
        DB_TABLE_CRACK,
        DB_TABLE_CRACK_FIX,
    }
}

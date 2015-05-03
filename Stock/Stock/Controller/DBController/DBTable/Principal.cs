using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace Stock.Controller.DBController.DBTable
{
    class Principal
    {
        private SQLiteConnection conn;
        public Principal(SQLiteConnection conn)
        {
            this.conn = conn;
        }
        public void Create()
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "create table 'Principal'(money real)";
            cmd.ExecuteNonQuery();
        }
        public void Insert(double money)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "insert into 'Principal' values(@money)";
            cmd.Parameters.Add(new SQLiteParameter("money", money));
            cmd.ExecuteNonQuery();
        }
        public void Update(double money)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "update 'Principal' set money=@money";
            cmd.Parameters.Add(new SQLiteParameter("money", money));
            cmd.ExecuteNonQuery();
        }
        public double Select()
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "select * from 'Principal'";
            SQLiteDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                if (reader.Read())
                {
                    return Convert.ToDouble(reader.GetValue(0));
                }
            }
            return 0;
        }
        public bool Exists()
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "select count(*) from sqlite_master where type='table' and name='Principal'";
            if (0 == Convert.ToInt32(cmd.ExecuteScalar()))
            {
                return false;
            }
            return true;
        }
    }
}

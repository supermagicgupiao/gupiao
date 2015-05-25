using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace Stock.Controller.DBController.DBTable
{
    class Principal
    {
        private SQLiteConnection conn;
        public Principal(SQLiteConnection conn)
        {
            this.conn = conn;
            Create();
        }
        //表创建
        public void Create()
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "create table if not exists 'Principal'(money real)";
            cmd.ExecuteNonQuery();
        }
        //插入数据
        public void Insert(PrincipalEntity PE)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "insert into 'Principal' values(@money)";
            cmd.Parameters.Add(new SQLiteParameter("money", PE.money));
            cmd.ExecuteNonQuery();
        }
        //更新数据
        public void Update(PrincipalEntity PE)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "update 'Principal' set money=@money";
            cmd.Parameters.Add(new SQLiteParameter("money", PE.money));
            cmd.ExecuteNonQuery();
        }
        //读取数据
        public void Select(out PrincipalEntity PE)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "select * from 'Principal'";
            SQLiteDataReader reader = cmd.ExecuteReader();
            PE = new PrincipalEntity();
            if (reader.HasRows)
            {
                if (reader.Read())
                {
                    PE.money = reader.GetValue(0).ToString();
                }
            }
        }
    }
    //Principal表结构体
    public struct PrincipalEntity
    {
        public string money;
    }
}

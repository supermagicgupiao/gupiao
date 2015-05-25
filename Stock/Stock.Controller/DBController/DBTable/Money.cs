using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace Stock.Controller.DBController.DBTable
{
    class Money
    {
        private SQLiteConnection conn;
        public Money(SQLiteConnection conn)
        {
            this.conn = conn;
            Create();
        }
        //表创建
        public void Create()
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "create table if not exists 'Money'(total real,now real)";
            cmd.ExecuteNonQuery();
        }
        //插入数据
        public void Insert(MoneyEntity ME)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "insert into 'Money' values(@total,@now)";
            cmd.Parameters.Add(new SQLiteParameter("total", ME.total));
            cmd.Parameters.Add(new SQLiteParameter("now", ME.now));
            cmd.ExecuteNonQuery();
        }
        //更新数据
        public void Update(MoneyEntity ME)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "update 'Money' set total=@total,now=@now";
            cmd.Parameters.Add(new SQLiteParameter("total", ME.total));
            cmd.Parameters.Add(new SQLiteParameter("now", ME.now));
            cmd.ExecuteNonQuery();
        }
        //更新数据
        public void Update(double m,byte flag)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            if (flag == 0)//0修改total
            {
                cmd.CommandText = "update 'Money' set total=@total";
                cmd.Parameters.Add(new SQLiteParameter("total", m));
            }
            else//1修改now
            {
                cmd.CommandText = "update 'Money' set now=@now";
                cmd.Parameters.Add(new SQLiteParameter("now", m));
            }
            cmd.ExecuteNonQuery();
        }
        //读取数据
        public void Select(out MoneyEntity ME)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "select * from 'Money'";
            SQLiteDataReader reader = cmd.ExecuteReader();
            ME = new MoneyEntity();
            if (reader.HasRows)
            {
                if (reader.Read())
                {
                    ME.total = Convert.ToDouble(reader.GetValue(0));
                    ME.now = Convert.ToDouble(reader.GetValue(1));
                }
            }
        }
    }
    public struct MoneyEntity
    {
        public double total;
        public double now;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;

namespace Stock.Controller.DBController.DBTable
{
    class StockHold
    {
        private SQLiteConnection conn;
        public StockHold(SQLiteConnection conn)
        {
            this.conn = conn;
            Create();
        }
        //创建表
        public void Create()
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "create table 'StockHold'(id varchar(7),name varchar,hold integer,money real)";
            cmd.ExecuteNonQuery();
        }
        //删除表
        public void Drop()
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "drop table 'StockHold'";
            cmd.ExecuteNonQuery();
        }
        //插入数据
        public void Insert(StockHoldEntity SHE)
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "insert into 'StockHold' values(@id,@name,@hold,@money)";
            cmd.Parameters.Add(new SQLiteParameter("id", SHE.id));
            cmd.Parameters.Add(new SQLiteParameter("name", SHE.name));
            cmd.Parameters.Add(new SQLiteParameter("hold", SHE.hold));
            cmd.Parameters.Add(new SQLiteParameter("money", SHE.money));
            cmd.ExecuteNonQuery();
        }

        //更新数据
        public void Update(StockHoldEntity SHE)
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "update 'StockHold' set hold=@hold,money=@money where id=@id";
            cmd.Parameters.Add(new SQLiteParameter("id", SHE.id));
            cmd.Parameters.Add(new SQLiteParameter("hold", SHE.hold));
            cmd.Parameters.Add(new SQLiteParameter("money", SHE.money));
            cmd.ExecuteNonQuery();
        }
        //读取数据
        public void Select(ref StockHoldEntity SHE)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "select name,hold,money from 'StockHold' where id=@id";
            cmd.Parameters.Add(new SQLiteParameter("id", SHE.id));
            SQLiteDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                if (reader.Read())
                {
                    SHE.name = reader.GetValue(0).ToString();
                    SHE.hold = reader.GetValue(1).ToString();
                    SHE.money = reader.GetValue(2).ToString();
                }
            }
        }
        //读取全部数据
        public void Select(out List<StockHoldEntity> SHEL)
        {
            SHEL = new List<StockHoldEntity>();
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "select * from 'StockHold'";
            StockHoldEntity SHE = new StockHoldEntity();
            SQLiteDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    SHE.id = reader.GetValue(0).ToString();
                    SHE.name = reader.GetValue(1).ToString();
                    SHE.hold = reader.GetValue(2).ToString();
                    SHE.money = reader.GetValue(3).ToString();
                    SHEL.Add(SHE);
                }
            }
        }
    }
    //StockHold表结构体
    public struct StockHoldEntity
    {
        public string id;
        public string name;
        public string hold;
        public string money;
    }
}

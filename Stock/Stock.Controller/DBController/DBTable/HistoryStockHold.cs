using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace Stock.Controller.DBController.DBTable
{
    class HistoryStockHold
    {
        private SQLiteConnection conn;
        public HistoryStockHold(SQLiteConnection conn)
        {
            this.conn = conn;
            Create();
        }
        //表不存在则创建
        public void Create()
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "create table if not exists 'HistoryStockHold'(id varchar(7),date datetime,number int,change int,money real)";
            cmd.ExecuteNonQuery();
        }
        //插入数据
        public void Insert(HistoryStockHoldEntity HSHE)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "insert into 'HistoryStockHold' values(@id,@date,@number,@change,@money)";
            cmd.Parameters.Add(new SQLiteParameter("id", HSHE.id));
            cmd.Parameters.Add(new SQLiteParameter("date", Convert.ToDateTime(HSHE.date.ToString("yyyy-MM-dd"))));
            cmd.Parameters.Add(new SQLiteParameter("number", HSHE.number));
            cmd.Parameters.Add(new SQLiteParameter("change", HSHE.change));
            cmd.Parameters.Add(new SQLiteParameter("money", HSHE.money));
            cmd.ExecuteNonQuery();
        }
        //获取全部历史记录
        public void Select(out List<HistoryStockHoldEntity> HSHEL)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "select * from 'HistoryStockHold'";
            SQLiteDataReader reader = cmd.ExecuteReader();
            HSHEL = Package(reader);
        }
        //根据id获取其中全部历史记录
        public void Select(string id, out List<HistoryStockHoldEntity> HSHEL)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "select * from 'HistoryStockHold' where id=@id";
            cmd.Parameters.Add(new SQLiteParameter("id", id));
            SQLiteDataReader reader = cmd.ExecuteReader();
            HSHEL = Package(reader);
        }
        //根据id和日期范围获取其中全部历史记录
        public void Select(string id, DateTime date, int days, out List<HistoryStockHoldEntity> HSHEL)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "select * from 'HistoryStockHold' where id=@id and date>=@date and date<=@days order by date asc";
            cmd.Parameters.Add(new SQLiteParameter("id", id));
            cmd.Parameters.Add(new SQLiteParameter("date", date));
            cmd.Parameters.Add(new SQLiteParameter("days", date.AddDays(days)));
            SQLiteDataReader reader = cmd.ExecuteReader();
            HSHEL = Package(reader);
        }
        //日期范围获取其中全部历史记录
        public void Select(DateTime date, int days, out List<HistoryStockHoldEntity> HSHEL)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "select * from 'HistoryStockHold' where date>=@date and date<=@days order by date asc";
            cmd.Parameters.Add(new SQLiteParameter("date", date));
            cmd.Parameters.Add(new SQLiteParameter("days", date.AddDays(days)));
            SQLiteDataReader reader = cmd.ExecuteReader();
            HSHEL = Package(reader);
        }
        //Reader数据封装成list
        private List<HistoryStockHoldEntity> Package(SQLiteDataReader reader)
        {
            List<HistoryStockHoldEntity> HSHEL = new List<HistoryStockHoldEntity>();
            HistoryStockHoldEntity HSHE;
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    HSHE.id = reader.GetValue(0).ToString();
                    HSHE.date = Convert.ToDateTime(reader.GetValue(1));
                    HSHE.number = Convert.ToInt32(reader.GetValue(2));
                    HSHE.change = Convert.ToInt32(reader.GetValue(3));
                    HSHE.money = Convert.ToInt32(reader.GetValue(4));
                    HSHEL.Add(HSHE);
                }
            }
            return HSHEL;
        }
    }
    public struct HistoryStockHoldEntity
    {
        public String id;
        public DateTime date;
        public Int32 number;
        public Int32 change;
        public Double money;
    }
}

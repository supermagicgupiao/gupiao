using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace Stock.Controller.DBController.DBTable
{
    class DealLists
    {
        private SQLiteConnection conn;
        private string user;
        public DealLists(SQLiteConnection conn, string name)
        {
            this.conn = conn;
            this.user=name;
            Create();
        }
        //表不存在则创建
        public void Create()
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "create table if not exists '" + user + "_DealList'(deal int unique,name varchar(8),id varchar(7),date datetime,type int2,money real,number int,taxrate real,commission real,explain text,remark text)";
            cmd.ExecuteNonQuery();
        }
        //删除表
        public void Drop()
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "drop table if exists '" + user + "_DealList'";
            cmd.ExecuteNonQuery();
        }
        //插入数据
        public void Insert(DealListEntity DLE)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "insert into '" + user + "_DealList' values(@deal,@name,@id,@date,@type,@money,@number,@taxrate,@commission,@explain,@remark)";
            cmd.Parameters.Add(new SQLiteParameter("deal", DLE.deal));
            cmd.Parameters.Add(new SQLiteParameter("name", DLE.name));
            cmd.Parameters.Add(new SQLiteParameter("id", DLE.id));
            cmd.Parameters.Add(new SQLiteParameter("date", DLE.date));
            cmd.Parameters.Add(new SQLiteParameter("type", DLE.type));
            cmd.Parameters.Add(new SQLiteParameter("money", DLE.money));
            cmd.Parameters.Add(new SQLiteParameter("number", DLE.number));
            cmd.Parameters.Add(new SQLiteParameter("taxrate", DLE.taxrate));
            cmd.Parameters.Add(new SQLiteParameter("commission", DLE.commission));
            cmd.Parameters.Add(new SQLiteParameter("explain", DLE.explain));
            cmd.Parameters.Add(new SQLiteParameter("remark", DLE.remark));
            cmd.ExecuteNonQuery();
        }
        //修改数据
        public void Update(DealListEntity DLE)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "update '" + user + "_DealList' set name=@name,id=@id,date=@date,type=@type,money=@money,number=@number,taxrate=@taxrate,commission=@commission,explain=@explain,remark=@remark where deal=@deal";
            cmd.Parameters.Add(new SQLiteParameter("deal", DLE.deal));
            cmd.Parameters.Add(new SQLiteParameter("name", DLE.name));
            cmd.Parameters.Add(new SQLiteParameter("id", DLE.id));
            cmd.Parameters.Add(new SQLiteParameter("date", DLE.date));
            cmd.Parameters.Add(new SQLiteParameter("type", DLE.type));
            cmd.Parameters.Add(new SQLiteParameter("money", DLE.money));
            cmd.Parameters.Add(new SQLiteParameter("number", DLE.number));
            cmd.Parameters.Add(new SQLiteParameter("taxrate", DLE.taxrate));
            cmd.Parameters.Add(new SQLiteParameter("commission", DLE.commission));
            cmd.Parameters.Add(new SQLiteParameter("explain", DLE.explain));
            cmd.Parameters.Add(new SQLiteParameter("remark", DLE.remark));
            cmd.ExecuteNonQuery();
        }
        //删除数据
        public void Delate(Int32 deal)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "delete from '" + user + "_DealList' where deal=@deal";
            cmd.Parameters.Add(new SQLiteParameter("deal", deal));
            cmd.ExecuteNonQuery();
        }
        //根据交易号获取其中交易记录
        public void Select(int deal, out DealListEntity DLE)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "select * from '" + user + "_DealList' where deal=@deal";
            cmd.Parameters.Add(new SQLiteParameter("deal", deal));
            SQLiteDataReader reader = cmd.ExecuteReader();
            List<DealListEntity> DLEL = Package(reader);
            if (DLEL.Count != 0)
                DLE = DLEL.First();
            else
                DLE = new DealListEntity();
        }
        //根据id获取其中全部交易记录
        public void Select(string id, out List<DealListEntity> DLEL)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "select * from '" + user + "_DealList' where id=@id";
            cmd.Parameters.Add(new SQLiteParameter("id", id));
            SQLiteDataReader reader = cmd.ExecuteReader();
            DLEL = Package(reader);
        }
        //获取全部交易记录
        public void Select(out List<DealListEntity> DLEL)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "select * from '" + user + "_DealList'";
            SQLiteDataReader reader = cmd.ExecuteReader();
            DLEL = Package(reader);
        }
        //Reader数据封装成list
        private List<DealListEntity> Package(SQLiteDataReader reader)
        {
            List<DealListEntity> DLEL;
            DLEL = new List<DealListEntity>();
            DealListEntity DLE;
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    DLE.deal = Convert.ToInt32(reader.GetValue(0));
                    DLE.name = reader.GetValue(1).ToString();
                    DLE.id = reader.GetValue(2).ToString();
                    DLE.date = Convert.ToDateTime(reader.GetValue(3));
                    DLE.type = reader.GetValue(4).ToString();
                    DLE.money = Convert.ToDouble(reader.GetValue(5));
                    DLE.number = Convert.ToInt32(reader.GetValue(6));
                    DLE.taxrate =  Convert.ToDouble(reader.GetValue(7));
                    DLE.commission =  Convert.ToDouble(reader.GetValue(8));
                    DLE.explain = reader.GetValue(9).ToString();
                    DLE.remark = reader.GetValue(10).ToString();
                    DLEL.Add(DLE);
                }
            }
            return DLEL;
        }
    }
    //DealList表结构体
    public struct DealListEntity
    {
        public Int32 deal;
        public String name;
        public String id;
        public DateTime date;
        public String type;
        public Double money;
        public Int32 number;
        public Double taxrate;
        public Double commission;
        public String explain;
        public String remark;
    }
}

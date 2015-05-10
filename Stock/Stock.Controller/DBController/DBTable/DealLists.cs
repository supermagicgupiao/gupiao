using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace Stock.Controller.DBController.DBTable
{
    class DealLists
    {
        private SQLiteConnection conn;
        public DealLists(SQLiteConnection conn)
        {
            this.conn = conn;
            Create();
        }
        //表不存在则创建
        public void Create()
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "create table if not exists 'DealList'(name varchar(8),id varchar(7),date text,type int2,money real,number int,taxrate real,commission real,explain text,remark text)";
            cmd.ExecuteNonQuery();
        }
        //删除表
        public void Drop()
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "drop table 'DealList'";
            cmd.ExecuteNonQuery();
        }
        //插入数据
        public void Insert(DealListEntity DLE)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "insert into 'DealList' values(@name,@id,@date,@type,@money,@number,@taxrate,@commission,@explain,@remark)";
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
        //根据id获取其中全部交易记录
        public void Select(string id, out List<DealListEntity> DLEL)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "select * from 'DealList' where id=@id";
            cmd.Parameters.Add(new SQLiteParameter("id", id));
            SQLiteDataReader reader = cmd.ExecuteReader();
            DLEL = Package(reader);
        }
        //获取全部交易记录
        public void Select(out List<DealListEntity> DLEL)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "select * from 'DealList'";
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
                    DLE.name = reader.GetValue(0).ToString();
                    DLE.id = reader.GetValue(1).ToString();
                    DLE.date = reader.GetValue(2).ToString();
                    DLE.type = reader.GetValue(3).ToString();
                    DLE.money = reader.GetValue(4).ToString();
                    DLE.number = reader.GetValue(5).ToString();
                    DLE.taxrate = reader.GetValue(6).ToString();
                    DLE.commission = reader.GetValue(7).ToString();
                    DLE.explain = reader.GetValue(8).ToString();
                    DLE.remark = reader.GetValue(9).ToString();
                    DLEL.Add(DLE);
                }
            }
            return DLEL;
        }
    }
    //DealList表结构体
    public struct DealListEntity
    {
        public string name;
        public string id;
        public string date;
        public string type;
        public string money;
        public string number;
        public string taxrate;
        public string commission;
        public string explain;
        public string remark;
    }
}

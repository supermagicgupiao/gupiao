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
            if (!Exists())
                Create();
        }
        public void Create()
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "create table 'DealList'(name varchar(8),id varchar(7),date text,type int2,money real,number int,taxrate real,commission real,explain text,remark text)";
            cmd.ExecuteNonQuery();
        }
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
        public void Select(ref DealListEntity DLE)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "select * from 'DealList' where id=@id";
            cmd.Parameters.Add(new SQLiteParameter("id", DLE.id));
            SQLiteDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                if(reader.Read())
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
                }
            }
        }
        public void Selects(out List<DealListEntity> DLEL)
        {
            DLEL = new List<DealListEntity>();
            SQLiteCommand cmd = new SQLiteCommand(conn);
            DealListEntity DLE;
            cmd.CommandText = "select * from 'DealList'";
            SQLiteDataReader reader = cmd.ExecuteReader();
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
        }
        private bool Exists()
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "select count(*) from sqlite_master where type='table' and name='DealList'";
            if (0 == Convert.ToInt32(cmd.ExecuteScalar()))
            {
                return false;
            }
            return true;
        }
    }
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

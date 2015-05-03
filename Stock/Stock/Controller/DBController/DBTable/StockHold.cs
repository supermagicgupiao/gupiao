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
            if (Exists())
                Drop();
            Create();
        }
        public void Create()
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "create table 'StockHold'(id varchar(7),hold integer,money real)";
            cmd.ExecuteNonQuery();
        }
        public void Insert(StockHoldEntity SHE)
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "insert into 'StockHold' values(@id,@hold,@money)";
            cmd.Parameters.Add(new SQLiteParameter("id", SHE.id));
            cmd.Parameters.Add(new SQLiteParameter("hold", SHE.hold));
            cmd.Parameters.Add(new SQLiteParameter("money", SHE.money));
            cmd.ExecuteNonQuery();
        }
        public void Change(StockHoldEntity SHE)
        {
            StockHoldEntity SHE_;
            SHE_.id = SHE.id;
            SHE_.hold = "";
            SHE_.money = "";
            Select(ref SHE_);
            SHE.hold = (Convert.ToInt32(SHE.hold) + Convert.ToInt32(SHE_.hold)).ToString();
            SHE.money = (Convert.ToDouble(SHE.money) + Convert.ToDouble(SHE_.money)).ToString();
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "update 'StockHold' set hold=@hold,money=@money where id=@id";
            cmd.Parameters.Add(new SQLiteParameter("id", SHE.id));
            cmd.Parameters.Add(new SQLiteParameter("hold", SHE.hold));
            cmd.Parameters.Add(new SQLiteParameter("money", SHE.money));
            cmd.ExecuteNonQuery();
        }
        public void Select(ref StockHoldEntity SHE)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "select hold,money from 'StockHold' where id=@id";
            cmd.Parameters.Add(new SQLiteParameter("id", SHE.id));
            SQLiteDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    SHE.hold = reader.GetValue(0).ToString();
                    SHE.money = reader.GetValue(1).ToString();
                }
            }
        }
        public void Selects(out List<StockHoldEntity> SHEL)
        {
            SHEL = new List<StockHoldEntity>();
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "select * from 'StockHold'";
            StockHoldEntity SHE;
            SQLiteDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    SHE.id = reader.GetValue(0).ToString();
                    SHE.hold = reader.GetValue(1).ToString();
                    SHE.money = reader.GetValue(2).ToString();
                    SHEL.Add(SHE);
                }
            }
        }
        public bool StockExists(string id)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "select count(*) from 'StockHold' where id=@id";
            cmd.Parameters.Add(new SQLiteParameter("id", id));
            if (0 == Convert.ToInt32(cmd.ExecuteScalar()))
            {
                return false;
            }
            return true;
        }
        private bool Exists()
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "select count(*) from sqlite_master where type='table' and name='StockHold'";
            if (0 == Convert.ToInt32(cmd.ExecuteScalar()))
            {
                return false;
            }
            return true;
        }
        public void Drop()
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "drop table 'StockHold'";
            cmd.ExecuteNonQuery();
        }
    }
    public struct StockHoldEntity
    {
        public string id;
        public string hold;
        public string money;
    }
}

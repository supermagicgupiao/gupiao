using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace Stock.Controller.DBController.DBTable
{
    class Log
    {
        private SQLiteConnection conn;
        public Log(SQLiteConnection conn)
        {
            this.conn = conn;
            Create();
        }
        //表不存在则创建
        public void Create()
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "create table if not exists 'Log'(state varchar,context text)";
            cmd.ExecuteNonQuery();
        }
        //删除表
        public void Drop()
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "drop table 'Log'";
            cmd.ExecuteNonQuery();
        }
        //插入数据
        public void Insert(LogEntity log)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "insert into 'Log' values(@state,@context)";
            cmd.Parameters.Add(new SQLiteParameter("state", log.state));
            cmd.Parameters.Add(new SQLiteParameter("context", log.context));
            cmd.ExecuteNonQuery();
        }
        //读取全部数据
        public void Selects(out List<LogEntity> LEL)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "select * from 'Log'";
            SQLiteDataReader reader = cmd.ExecuteReader();
            LogEntity LE = new LogEntity();
            LEL = new List<LogEntity>();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    LE.state = reader.GetValue(0).ToString();
                    LE.context = reader.GetValue(1).ToString();
                    LEL.Add(LE);
                }
            }
        }
    }
    //Log表结构体
    public struct LogEntity
    {
        public string state;
        public string context;
    }
}

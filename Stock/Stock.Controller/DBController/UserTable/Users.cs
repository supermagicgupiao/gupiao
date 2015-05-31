using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SQLite;

namespace Stock.Controller.DBController.UserTable
{
    class Users
    {
        private SQLiteConnection conn;
        public Users(SQLiteConnection conn)
        {
            this.conn = conn;
            Create();
        }
        //表不存在则创建
        public void Create()
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "create table if not exists 'Users'(name varchar unique,path varchar)";
            cmd.ExecuteNonQuery();
        }
        //删除表
        public void Drop()
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "drop table 'Users'";
            cmd.ExecuteNonQuery();
        }
        //插入数据
        public void Insert(UsersEntity UE)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "insert into 'Users' values(@name,@path)";
            cmd.Parameters.Add(new SQLiteParameter("name", UE.name));
            cmd.Parameters.Add(new SQLiteParameter("path", UE.path));
            cmd.ExecuteNonQuery();
        }
        //删除数据
        public void Delete(string name)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "delete from 'Users' where name=@name";
            cmd.Parameters.Add(new SQLiteParameter("name", name));
            cmd.ExecuteNonQuery();
        }
        //获取一个用户
        public UsersEntity Select(string name)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "select * from 'Users' where name=@name";
            cmd.Parameters.Add(new SQLiteParameter("name", name));
            SQLiteDataReader reader = cmd.ExecuteReader();
            UsersEntity UE = new UsersEntity();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    UE.name = reader.GetValue(0).ToString();
                    UE.path = reader.GetValue(1).ToString();
                }
            }
            return UE;
        }
        //获取全部用户
        public void Select(out List<UsersEntity> UEL)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "select * from 'Users'";
            SQLiteDataReader reader = cmd.ExecuteReader();
            UEL = new List<UsersEntity>();
            UsersEntity UE;
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    UE.name = reader.GetValue(0).ToString();
                    UE.path = reader.GetValue(1).ToString();
                    UEL.Add(UE);
                }
            }
        }
    }
    //Users表结构体
    public struct UsersEntity
    {
        public String name;
        public String path;
    }
}

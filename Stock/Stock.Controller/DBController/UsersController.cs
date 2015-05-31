using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SQLite;
using System.IO;

using Stock.Controller.DBController.DBConnection;
using Stock.Controller.DBController.UserTable;

namespace Stock.Controller.DBController
{
    public class UsersController
    {
        private Dictionary<string, DBDataController> userdict = new Dictionary<string, DBDataController>();
        private string dbPath;
        //文件数据库连接
        private SQLiteConnection file_conn;
        //数据库错误
        private DB_ERROR DBE;
        //用户表
        private Users users;
        //默认数据库
        public UsersController()
        {
            dbPath = Environment.CurrentDirectory + "\\Stock.db";
            DBE = Check();
        }
        //指定数据库
        public UsersController(string path)
        {
            dbPath = path;
            DBE = Check();
        }
        //增加新的users
        public DB_ERROR AddNewUser(string name, double principal)
        {
            return AddNewUser(name, dbPath, principal);
        }
        public DB_ERROR AddNewUser(string name, string path, double principal)
        {
            if (userdict.ContainsKey(name))
                return DB_ERROR.DB_USER_TABLE_EXISTS;
            DBDataController DBC = new DBDataController(name, path);
            DB_ERROR DBE = DBC.GetLastError();
            if (DBE == DB_ERROR.DB_DATA_NOT_EXISTS)
            {
                DBC.PrincipalCreate(principal);
                UsersEntity UE;
                UE.name = name;
                UE.path = dbPath;
                users.Insert(UE);
                userdict.Add(name, DBC);
                return DB_ERROR.DB_OK;
            }
            else if (DBE == DB_ERROR.DB_OK)
            {
                return DB_ERROR.DB_USER_EXISTS;
            }
            else
            {
                return DBE;
            }
        }
        //增加到dict中
        public DB_ERROR AddUser(string name, string path)
        {
            if (userdict.ContainsKey(name))
                return DB_ERROR.DB_USER_TABLE_EXISTS;
            DBDataController DBC = new DBDataController(name, path);
            DB_ERROR DBE = DBC.GetLastError();
            if (DBE == DB_ERROR.DB_OK)
            {
                UsersEntity UE;
                UE.name = name;
                UE.path = dbPath;
                users.Insert(UE);
                userdict.Add(name, DBC);
                return DB_ERROR.DB_OK;
            }
            else
            {
                DBC.DeleteAll();
                return DBE;
            }
        }
        //删除用户
        public void DelUser(string name)
        {
            DelUser(name, dbPath);
        }
        public void DelUser(string name, string path)
        {
            users.Delete(name);
            if (userdict.ContainsKey(name))
            {
                userdict[name].DeleteAll();
                userdict.Remove(name);
            }
            else
            {
                DBDataController DBC = new DBDataController(name, dbPath);
                DBC.DeleteAll();
            }
        }


        //初始化检测
        private DB_ERROR Check()
        {
            if (DBConnection())//连接检测
            {
                DB_ERROR dbe = DBCheckTable();//数据库表检测
                if (dbe == DB_ERROR.DB_TABLE_CRACK)//表损坏
                {
                    file_conn.Close();
                    try { File.Delete(dbPath); }//删除表
                    catch (Exception)
                    {
                        return dbe;//表损坏
                    }
                    file_conn.Open();
                    if (DBCheckTable() == DB_ERROR.DB_DATA_NOT_EXISTS) //重建表
                        return DB_ERROR.DB_TABLE_CRACK_FIX;//表修复
                    else
                        return dbe;
                }
                else
                {
                    return dbe;//数据库正常或者数据不存在
                }
            }
            else
            {
                return DB_ERROR.DB_CANT_CONNECT;//数据库无法连接
            }
        }
        //数据库连接
        private bool DBConnection()
        {
            try
            {
                file_conn = FileConnection.Create(dbPath);
            }
            catch (SQLiteException)
            {
                return false;
            }
            return true;
        }
        //数据库检测
        private DB_ERROR DBCheckTable()
        {
            try
            {
                users = new Users(file_conn);
                List<UsersEntity> UEL;
                users.Select(out UEL);
                if (UEL.Count == 0)
                    return DB_ERROR.DB_DATA_NOT_EXISTS;
                foreach (UsersEntity UE in UEL)
                {
                    AddUser(UE.name, UE.path);
                }
            }
            catch (SQLiteException)
            {
                return DB_ERROR.DB_TABLE_CRACK;//表损坏
            }
            return DB_ERROR.DB_OK;
        }
        //获取最后错误
        public DB_ERROR GetLastError()
        {
            return DBE;
        }

    }
}

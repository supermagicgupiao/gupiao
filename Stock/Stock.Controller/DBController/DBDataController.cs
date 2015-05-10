using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;
using System.IO;
using Stock.Controller.DBController.DBTable;

namespace Stock.Controller.DBController
{
    public class DBDataController
    {
        //路径
        private string dbPath;
        //文件数据库连接
        private SQLiteConnection file_conn;
        //内存数据库连接
        private SQLiteConnection mem_conn;
        //数据库表类
        private DealLists deallist;
        private StockHold stockhold;
        private Principal principal;
        private Log log;


        //默认路径数据库
        public DBDataController()
        {
            dbPath = Environment.CurrentDirectory + "\\Stock.db";
        }
        //指定路径数据库
        public DBDataController(string s)
        {
            dbPath = s;
        }


        //数据库初始化的检查
        public DB_ERROR Check()
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

        //读取全部的交易记录
        public void DealListReadAll(out List<DealListEntity> DLEL)
        {
            deallist.Select(out DLEL);
        }
        //增加交易记录
        public void DealListAdd(DealListEntity DLE)
        {
            deallist.Insert(DLE);
            DealList_Insert_StockHold(DLE);
        }
        //批量增加交易记录
        public void DealListAdd(List<DealListEntity> DLEL)
        {
            foreach (DealListEntity DLE in DLEL)
            {
                deallist.Insert(DLE);
                DealList_Insert_StockHold(DLE);
            }
        }
        //读取全部持股构成
        public void StockHoldReadAll(out List<StockHoldEntity> SHEL)
        {
            stockhold.Select(out SHEL);
        }
        //创建本金
        public void PrincipalCreate(double m)
        {
            PrincipalEntity PE = new PrincipalEntity();
            PE.money = m.ToString();
            principal.Insert(PE);
        }
        //修改本金
        public void PrincipalWrite(double m)
        {
            PrincipalEntity PE = new PrincipalEntity();
            PE.money = m.ToString();
            principal.Update(PE);
        }
        //读取本金
        public double PrincipalRead()
        {
            PrincipalEntity PE;
            principal.Select(out PE);
            return Convert.ToDouble(PE.money);
        }
        //保存日志
        public void LogSave(LogEntity LE)
        {
            log.Insert(LE);
        }
        //读取日志
        public void LogRead(out List<LogEntity> LEL)
        {
            log.Selects(out LEL);
        }


        //数据库连接
        private bool DBConnection()
        {
            try
            {
                //连接文件数据库
                file_conn = new SQLiteConnection();
                SQLiteConnectionStringBuilder connstr = new SQLiteConnectionStringBuilder();
                connstr.DataSource = dbPath;
                //connstr.Password = "admin";
                file_conn.ConnectionString = connstr.ToString();
                file_conn.Open();
                //连接内存数据库
                mem_conn = new SQLiteConnection("data source=:memory:");
                mem_conn.Open();
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
                //文件数据库：交易记录表、本金表、日志表
                deallist = new DealLists(file_conn);
                principal = new Principal(file_conn);
                log = new Log(file_conn);

                //内存数据库：持股表
                stockhold = new StockHold(mem_conn);

                //交易记录创建持股表
                StockHold_Init();

                PrincipalEntity PE;
                principal.Select(out PE);//本金表检测数据是否存在
                if (PE.money == null)
                    return DB_ERROR.DB_DATA_NOT_EXISTS;//数据不存在
            }
            catch (SQLiteException)
            {
                return DB_ERROR.DB_TABLE_CRACK;//表损坏
            }
            return DB_ERROR.DB_OK;
        }
        //初始化交易记录转内存数据库持股构成
        private void StockHold_Init()
        {
            List<DealListEntity> DLEL;
            DealListReadAll(out DLEL);
            foreach(DealListEntity DLE in DLEL)
            {
                DealList_Insert_StockHold(DLE);
            }
        }
        //按类型转换
        private void DealList_Insert_StockHold(DealListEntity DLE)
        {
            StockHoldEntity SHE = new StockHoldEntity();
            SHE.id = DLE.id;
            SHE.name = DLE.name;
            if (DLE.type == "买入")
                SHE.hold = DLE.number;
            else if (DLE.type == "卖出")
                SHE.hold = "-" + DLE.number;
            else if (DLE.type == "补仓")
                SHE.hold = DLE.number;
            else if (DLE.type == "卖空")
                SHE.hold = "-" + DLE.number;
            else
                SHE.hold = DLE.number;
            SHE.money = (Convert.ToDouble(DLE.money) * Convert.ToDouble(SHE.hold)).ToString();
            StockHoldEntity SHE_=new StockHoldEntity();
            SHE_.id = SHE.id;
            stockhold.Select(ref SHE_);
            if (SHE_.money != null && SHE_.hold != null)
            {
                SHE.hold = (Convert.ToInt32(SHE.hold) + Convert.ToInt32(SHE_.hold)).ToString();
                SHE.money = (Convert.ToDouble(SHE.money) + Convert.ToDouble(SHE_.money)).ToString();
                stockhold.Update(SHE);
            }
            else
            {
                stockhold.Insert(SHE);
            }
        }

    }
    public enum DB_ERROR : byte
    {
        DB_OK,
        DB_CANT_CONNECT,
        DB_TABLE_CRACK,
        DB_TABLE_CRACK_FIX,
        DB_DATA_NOT_EXISTS,
    }
}

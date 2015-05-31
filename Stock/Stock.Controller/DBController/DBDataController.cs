using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;
using System.IO;
using Stock.Controller.DBController.DBTable;
using Stock.Controller.DBController.DBConnection;

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
        private HistoryStockHold historystockhold;
        private Log log;
        //金钱实体
        private MoneyEntity money = new MoneyEntity();

        //数据库错误
        private DB_ERROR DBE;

        //委托类
        private DBDelegateBridge delegateController;

        //用户名
        private string user;

        ////默认路径数据库
        //public DBDataController()
        //{
        //    dbPath = Environment.CurrentDirectory + "\\Stock.db";
        //    user = "default";
        //    DBE = Check();
        //    delegateController = new DBDelegateBridge(this);
        //}
        ////指定用户名
        //public DBDataController(string name)
        //{
        //    dbPath = Environment.CurrentDirectory + "\\Stock.db";
        //    user = name;
        //    DBE = Check();
        //    delegateController = new DBDelegateBridge(this);
        //}
        //指定路径
        public DBDataController(string name,string path)
        {
            dbPath = path;
            user = name;
            DBE = Check();
            delegateController = new DBDelegateBridge(this);
        }
        //获取委托控制器
        public DBDelegateBridge DelegateController()
        {
            return delegateController;
        }

        //数据库初始化的检查
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

        //委托
        //修改金钱委托
        public delegate void MoneyDelegate(MoneyEntity ME);
        private MoneyDelegate moneyDelegate;
        public void SetMoneyDelegate(MoneyDelegate moneyDelegate)
        {
            this.moneyDelegate = moneyDelegate;
        }
        //money执行委托
        private void MoneyReadSet()
        {
            if (moneyDelegate != null)
                moneyDelegate(money);
        }
        //修改持股构成委托
        public delegate void StockHoldDelegate(StockHoldEntity SHE);
        private StockHoldDelegate stockholdDelegate;
        public void SetStockHoldDelegate(StockHoldDelegate stockholdDelegate)
        {
            this.stockholdDelegate = stockholdDelegate;
        }
        //持股构成执行委托
        private void StockHoldSet(StockHoldEntity SHE)
        {
            if (stockholdDelegate != null)
                stockholdDelegate(SHE);
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
            MoneyReadSet();
        }
        //批量增加交易记录
        public void DealListAdd(List<DealListEntity> DLEL)
        {
            foreach (DealListEntity DLE in DLEL)
            {
                deallist.Insert(DLE);
                DealList_Insert_StockHold(DLE);
            }
            MoneyReadSet();
        }
        //读取全部持股构成
        public void StockHoldReadAll(out List<StockHoldEntity> SHEL)
        {
            stockhold.Select(out SHEL);
        }
        //读取指定id持股
        public void StockHoldRead(ref StockHoldEntity SHE)
        {
            stockhold.Select(ref SHE);
        }
        //创建本金
        public void PrincipalCreate(double m)
        {
            PrincipalEntity PE = new PrincipalEntity();
            PE.money = m.ToString();
            principal.Insert(PE);
            //总资产与现金创建
            money.now = m;
            money.total = m;
            //交易记录创建持股表
            StockHold_Init();
        }
        //写入本金
        private void PrincipalWrite(double m)
        {
            PrincipalEntity PE = new PrincipalEntity();
            PE.money = m.ToString();
            principal.Update(PE);
            money.principal = m;
        }
        //读取本金
        public double PrincipalRead()
        {
            PrincipalEntity PE;
            principal.Select(out PE);
            return Convert.ToDouble(PE.money);
        }
        //修改本金
        public void PrincipalChange(double m)
        {
            PrincipalWrite(PrincipalRead() + m);
            MoneyChangeNow(m);
            MoneyChangeTotal(m);
            MoneyReadSet();
        }
        //保存日志
        public void LogSave(LogEntity LE)
        {
            log.Insert(LE);
        }
        //读取日志
        public void LogRead(out List<LogEntity> LEL)
        {
            log.Select(out LEL);
        }
        //股票编号和日期范围内指定股票编号的历史收盘价
        public void HistoryStockHoldReadByRange(string id, DateTime date, int days, out List<HistoryStockHoldEntity> HSHEL)
        {
            historystockhold.Select(id, date, days, out HSHEL);
        }
        //日期范围内指定股票编号的历史收盘价
        public void HistoryStockHoldReadByRange(DateTime date, int days, out List<HistoryStockHoldEntity> HSHEL)
        {
            historystockhold.Select(date, days, out HSHEL);
        }

        //修改money
        private void MoneyChangeNow(double m)
        {
            money.now += m;
        }
        private void MoneyChangePrincipal(double m)
        {
            money.principal += m;
        }
        private void MoneyChangeTotal(double m)
        {
            money.total += m;
        }


        //获取最后错误
        public DB_ERROR GetLastError()
        {
            return DBE;
        }

        //删除用户所有的表
        public void DeleteAll()
        {
            deallist.Drop();
            stockhold.Drop();
            principal.Drop();
            historystockhold.Drop();
            log.Drop();
        }

        //关闭数据库连接
        public void Close()
        {
            file_conn.Close();
        }

        //数据库连接
        private bool DBConnection()
        {
            try
            {
                //连接文件数据库
                file_conn = FileConnection.Create(dbPath);
                //连接内存数据库
                mem_conn = MemConnection.Create();
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
                deallist = new DealLists(file_conn, user);
                principal = new Principal(file_conn, user);
                log = new Log(file_conn, user);

                //内存数据库：持股表
                stockhold = new StockHold(mem_conn, user);
                historystockhold = new HistoryStockHold(mem_conn, user);

                PrincipalEntity PE;
                principal.Select(out PE);//本金表检测数据是否存在
                if (PE.money == null)
                    return DB_ERROR.DB_DATA_NOT_EXISTS;//数据不存在
                //内存建立总资产和现金
                double m = Convert.ToDouble(PE.money);
                money.now = m;
                money.total = m;

                //交易记录创建持股表
                StockHold_Init();
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
            MoneyReadSet();
        }
        //按类型转换
        private void DealList_Insert_StockHold(DealListEntity DLE)
        {
            StockHoldEntity SHE = new StockHoldEntity();
            SHE.id = DLE.id;
            SHE.name = DLE.name;
            if (DLE.type == "买入")
            {
                SHE.hold = DLE.number;
                MoneyChangeNow(-(DLE.number * DLE.money));
            }
            else if (DLE.type == "卖出")
            {
                SHE.hold = -DLE.number;
                MoneyChangeNow(DLE.number * DLE.money);
            }
            else if (DLE.type == "卖空")
            {
                SHE.hold = DLE.number;
                MoneyChangeNow(-(DLE.number * DLE.money));
            }
            else if (DLE.type == "补仓")
            {
                SHE.hold = -DLE.number;
                MoneyChangeNow(DLE.number * DLE.money);
            }
            else
            {
                SHE.hold = DLE.number;
                MoneyChangeNow(-(DLE.number * DLE.money));
            }
            SHE.money = Convert.ToDouble(DLE.money) * Convert.ToDouble(SHE.hold);
            StockHoldEntity SHE_=new StockHoldEntity();
            SHE_.id = SHE.id;
            stockhold.Select(ref SHE_);
            HistoryStockHoldEntity HSHE = new HistoryStockHoldEntity();
            HSHE.id = SHE.id;
            HSHE.number = SHE_.hold;
            HSHE.date = DLE.date;
            HSHE.change = SHE.hold;
            HSHE.money = DLE.money;
            historystockhold.Insert(HSHE);
            if (SHE_.name != null)
            {
                SHE.hold = SHE.hold + SHE_.hold;
                SHE.money = Convert.ToDouble(SHE.money) + Convert.ToDouble(SHE_.money);
                stockhold.Update(SHE);
            }
            else
            {
                stockhold.Insert(SHE);
            }
            StockHoldSet(SHE);
        }
    }
    public struct MoneyEntity
    {
        public double principal;
        public double now;
        public double total;
    }
}

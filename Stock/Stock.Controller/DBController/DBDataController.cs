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

        //交易记录id号
        private int dealid = 0;

        //用户名
        private string user;
        public string GetUserName()
        {
            return user;
        }

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
            money.name = name;
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
        public void TriggerDelegate()
        {
            if (moneyDelegate != null)
                moneyDelegate(money);
        }

        //读取现金
        public double NowMoneyRead()
        {
            return money.now;
        }

        //读取全部的交易记录
        public void DealListReadAll(out List<DealListEntity> DLEL)
        {
            deallist.Select(out DLEL);
        }
        //读取id全部的交易记录
        public void DealListReadById(string id, out List<DealListEntity> DLEL)
        {
            deallist.Select(id, out DLEL);
        }
        //读取交易号交易记录
        public DealListEntity DealListReadByDeal(int deal)
        {
            DealListEntity DLE;
            deallist.Select(deal, out DLE);
            return DLE;
        }
        //增加交易记录
        public void DealListAdd(DealListEntity DLE)
        {
            DLE.deal = dealid;
            deallist.Insert(DLE);
            StockHoldSet(DealList_Insert_StockHold(DLE));
            MoneyReadSet();
        }
        //批量增加交易记录
        public void DealListAdd(List<DealListEntity> DLEL)
        {
            Dictionary<string, StockHoldEntity> dict = new Dictionary<string, StockHoldEntity>();
            foreach (DealListEntity DLE in DLEL)
            {
                DealListEntity D = DLE;
                D.deal = dealid;
                deallist.Insert(D);
                StockHoldEntity SHE = DealList_Insert_StockHold(D);
                if (dict.ContainsKey(SHE.id))
                    dict[SHE.id] = SHE;
                else
                    dict.Add(SHE.id, SHE);
            }
            foreach (var x in dict)
            {
                StockHoldSet(x.Value);
            }
            MoneyReadSet();
        }
        //修改交易记录
        public void DealListUpdate(DealListEntity DLE)
        {
            deallist.Update(DLE);
            DealListRestore(DLE);
        }
        //删除交易记录
        public void DealListDelete(DealListEntity DLE)
        {
            deallist.Delate(DLE.deal);
            DealListRestore(DLE);
        }
        //重置id相关
        private void DealListRestore(DealListEntity DLE)
        {
            List<DealListEntity> DLEL;
            deallist.Select(DLE.id, out DLEL);
            StockHoldEntity SHE = new StockHoldEntity();
            SHE.id = DLE.id;
            stockhold.Select(ref SHE);
            MoneyChangeNow(SHE.money);
            stockhold.Delete(DLE.id);
            historystockhold.Delete(DLE.id);
            if (DLEL.Count == 0)
            {
                SHE.hold = 0;
                SHE.id = DLE.id;
                SHE.money = 0;
                SHE.name = DLE.name;
                StockHoldSet(SHE);
            }
            foreach (DealListEntity D in DLEL)
            {
                SHE = DealList_Insert_StockHold(D);
            }
            StockHoldSet(SHE);
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
            //本金与现金创建
            money.principal = m;
            money.now = m;
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
                //内存建立本金和现金
                double m = Convert.ToDouble(PE.money);
                money.principal = m;
                money.now = m;

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
            Dictionary<string, StockHoldEntity> dict = new Dictionary<string, StockHoldEntity>();
            foreach(DealListEntity DLE in DLEL)
            {
                StockHoldEntity SHE = DealList_Insert_StockHold(DLE);
                if (dict.ContainsKey(SHE.id))
                    dict[SHE.id] = SHE;
                else
                    dict.Add(SHE.id, SHE);
            }
            foreach (var x in dict)
            {
                StockHoldSet(x.Value);
            }
            MoneyReadSet();
        }
        //按类型转换
        private StockHoldEntity DealList_Insert_StockHold(DealListEntity DLE)
        {
            if (DLE.deal >= dealid)
                dealid = DLE.deal + 1;
            StockHoldEntity SHE = new StockHoldEntity();
            SHE.id = DLE.id;
            SHE.name = DLE.name;
            double c;
            if (DLE.type == "买入")
            {
                SHE.hold = DLE.number;
                c = -(DLE.number * DLE.money);
                MoneyChangeNow(c);
            }
            else if (DLE.type == "卖出")
            {
                SHE.hold = -DLE.number;
                c = DLE.number * DLE.money;
                MoneyChangeNow(c);
            }
            else if (DLE.type == "补仓")
            {
                SHE.hold = DLE.number;
                c = -(DLE.number * DLE.money);
                MoneyChangeNow(c);
            }
            else if (DLE.type == "卖空")
            {
                SHE.hold = -DLE.number;
                c = DLE.number * DLE.money;
                MoneyChangeNow(c);
            }
            else
            {
                SHE.hold = DLE.number;
                c = -(DLE.number * DLE.money);
                MoneyChangeNow(c);
            }
            SHE.money = -c;
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
            return SHE;
            //StockHoldSet(SHE);
        }
    }
    public struct MoneyEntity
    {
        public string name;
        public double principal;
        public double now;
    }
}

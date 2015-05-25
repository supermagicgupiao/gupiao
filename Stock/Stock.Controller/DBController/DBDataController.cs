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
        //修改金钱委托
        public delegate void ChangeMoney(double money);
        private ChangeMoney pri, total, now;
        public void SetMoneyDelegate(ChangeMoney pri,ChangeMoney total,ChangeMoney now)
        {
            this.pri = pri;
            this.total = total;
            this.now = now;
        }
        private void setPrincipal(double money)
        {
            if (pri != null)
                pri(money);
        }
        private void setTotal(double money)
        {
            if (total != null)
                total(money);
        }
        private void setNow(double money)
        {
            if (now != null)
                now(money);
        }
        //money修改
        //改变total
        public void TotalChange(double m)
        {
            MoneyEntity ME;
            money.Select(out ME);
            double mon = ME.total + m;
            money.Update(mon, 0);
        }
        //改变now
        public void NowChange(double m)
        {
            MoneyEntity ME;
            money.Select(out ME);
            double mon = ME.now + m;
            money.Update(mon, 1);
        }
        //两者改变
        public void TotalNowChange(double m)
        {
            MoneyEntity ME;
            money.Select(out ME);
            ME.total += m;
            ME.now += m;
            money.Update(ME);
        }
        //创建money记录
        private void MoneyCreate(double m1,double m2)
        {
            MoneyEntity ME;
            ME.total = m1;
            ME.now = m2;
            money.Insert(ME);
        }
        //读取money记录
        public void MoneyRead(out MoneyEntity ME)
        {
            money.Select(out ME);
        }
        //money读取设置
        public void MoneyReadSet()
        {
            MoneyEntity ME;
            money.Select(out ME);
            setTotal(ME.total);
            setNow(ME.now);
        }
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
        private Money money;

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
            MoneyCreate(m, m);
            //交易记录创建持股表
            StockHold_Init();
        }
        //写入本金
        private void PrincipalWrite(double m)
        {
            PrincipalEntity PE = new PrincipalEntity();
            PE.money = m.ToString();
            principal.Update(PE);
            setPrincipal(m);
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
            TotalNowChange(m);
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
                historystockhold = new HistoryStockHold(mem_conn);
                money = new Money(mem_conn);

                PrincipalEntity PE;
                principal.Select(out PE);//本金表检测数据是否存在
                if (PE.money == null)
                    return DB_ERROR.DB_DATA_NOT_EXISTS;//数据不存在
                //内存建立总资产和现金
                double m = Convert.ToDouble(PE.money);
                MoneyCreate(m, m);

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
                NowChange(-(DLE.number * DLE.money));
            }
            else if (DLE.type == "卖出")
            {
                SHE.hold = -DLE.number;
                NowChange(DLE.number * DLE.money);
            }
            else if (DLE.type == "卖空")
            {
                SHE.hold = DLE.number;
                NowChange(-(DLE.number * DLE.money));
            }
            else if (DLE.type == "补仓")
            {
                SHE.hold = -DLE.number;
                NowChange(DLE.number * DLE.money);
            }
            else
            {
                SHE.hold = DLE.number;
                NowChange(-(DLE.number * DLE.money));
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

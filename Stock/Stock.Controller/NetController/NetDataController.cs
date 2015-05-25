using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.Drawing;
using System.Collections;

using Stock.Controller.NetController.StockAPI;


namespace Stock.Controller.NetController
{
    public class NetDataController
    {
        //同步委托 定时返回获取的股票数据
        public delegate void sync(StockInfoEntity SIE);
        //同步委托 线程返回图片数据
        public delegate void backimage(Image image);
        //委托字典 一个股票编号对应一个委托
        private Dictionary<string, sync[]> stockdict = new Dictionary<string, sync[]>();
        //定时器
        private Timer t;
        //网络连接日志
        public static string log;
        //不带参数的刷新 五秒一次
        public void StartRefresh()
        {
            t = new Timer(GetStockInfo, stockdict, 2, 5 * 1000);
        }
        //指定时间刷新
        public void StartRefresh(int time)
        {
            t = new Timer(GetStockInfo, stockdict, 2, time * 1000);
        }
        //销毁定时器
        public void StopRefresh()
        {
            t.Dispose();
        }
        //增加股票以及其委托
        public bool StockRefreshAdd(string stockid,ref sync s)
        {
            try
            {
                if (!stockdict.ContainsKey(stockid))
                {
                    sync[] sn = new sync[2];
                    sn[0] = s;
                    stockdict.Add(stockid, sn);
                }
                else
                {
                    if (stockdict[stockid][0] != null)
                        return false;
                    stockdict[stockid][0] = s;
                }
            }
            catch(Exception)
            {
                return false;
            }
            return true;
        }
        public bool StockRefreshAddNow(string stockid, ref sync s)
        {
            if (!StockRefreshAdd(stockid, ref s))
                return false;
            GetStockInfoNow();
            return true;
        }
        //增加temp股票以及其委托
        public bool StockTempRefreshAdd(string stockid, ref sync s)
        {
            try
            {
                if (stockdict.ContainsKey(stockid))
                {
                    stockdict[stockid][1] = s;
                }
                else
                {
                    sync[] sn = new sync[2];
                    sn[1] = s;
                    stockdict.Add(stockid, sn);
                }
            }
            catch (Exception)
            {
                return false;
            }
            GetStockInfoNow();
            return true;
        }
        //删除股票
        public bool StockRefreshDelete(string stockid)
        {
            try
            {
                if (stockdict[stockid][1] == null)
                    stockdict.Remove(stockid);
                else
                    stockdict[stockid][0] = null;
            }
            catch(Exception)
            {
                return false;
            }
            return true;
        }
        //删除temp股票
        public bool StockTempRefreshDelete(string stockid)
        {
            try
            {
                if (stockdict[stockid][0] == null)
                    stockdict.Remove(stockid);
                else
                    stockdict[stockid][1] = null;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        //清空股票
        public bool StockRefreshClear()
        {
            try
            {
                stockdict.Clear();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        //获取k线图
        public void KchartImageGet(string id, kchart k, backimage bimage)
        {
            Thread get = new Thread(new ParameterizedThreadStart(ThreadImageGet));
            ImageEntity IE = new ImageEntity();
            IE.id = id;
            IE.k = k;
            IE.bimage = bimage;
            get.Start(IE);
        }
        private void ThreadImageGet(object data)
        {
            ImageEntity IE = (ImageEntity)data;
            StockInfo si = new Netease();
            Image image;
            si.KchartImageGet(IE.id, IE.k, out image);
            IE.bimage(image);
        }
        //获取历史股票收盘价
        public static void HistoryMoney(string id, DateTime date, int days, out Dictionary<DateTime, double> money)
        {
            StockInfo si = new Netease();
            si.HistoryMoney(id, date, date.AddDays(days), out money);
        }
        //获取历史股票收盘价
        public static void HistoryMoney(string id, DateTime date, DateTime enddate, out Dictionary<DateTime, double> money)
        {
            StockInfo si = new Netease();
            si.HistoryMoney(id, date, enddate, out money);
        }
        //获取某天股票收盘价
        public static double HistoryMoney(string id, DateTime date)
        {
            StockInfo si = new Netease();
            Dictionary<DateTime, double> money;
            si.HistoryMoney(id, date, date, out money);
            if (money.Count > 0)
                return money.First().Value;
            else
                return 0;
        }
        //批量获取
        private void GetStockInfo(object stock)
        {
            Dictionary<string, sync[]> dict = (Dictionary<string, sync[]>)stock;
            if (dict.Count == 0)//委托字典不为空则联网获取数据
                return;
            StockInfo si = new Netease();//使用网易的api
            List<string> id = new List<string>(dict.Keys);//股票编号列表
            Dictionary<string, StockInfoEntity> ds;
            si.StockGet(ref id, out ds);//得到id到股票数据实体的字典
            foreach(var s in ds)
            {
                if(dict.ContainsKey(s.Key))
                {
                    foreach (sync sn in dict[s.Key])
                    {
                        if (sn != null)
                            sn(s.Value);//将返回的字典键值作参数获取sync方法同步股票数据实体
                    }
                }
            }
        }
        //提交获取
        public void GetStockInfoNow()
        {
            GetStockInfo(stockdict);
        }
    }
    struct ImageEntity
    {
        public string id;
        public kchart k;
        public NetDataController.backimage bimage;
    }
    //股票数据实体
    public struct StockInfoEntity
    {
        public string name;
        public string arrow;
        public string high;
        public string low;
        public string open;
        public string percent;
        public string price;
        public string time;
        public string turnover;
        public string updown;
        public string volume;
        public string yestclose;
    }
    public enum NET_ERROR : byte
    {
        NET_REQ_OK,
        NET_CANT_CONNECT,
        NET_JSON_NOT_EXISTS,
        NET_REQ_ERROR,
        NET_DATA_ERROR,
    }
    public enum kchart : byte
    {
        time,//分时
        day30,//30天
        day90,//90天
        day180,//180天
        week,//周
        month,//月
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;

namespace Stock.Controller.NetController.StockAPI
{
    abstract class StockInfo
    {
        static public List<string> log = new List<string>();//静态日志
        public abstract bool StockGet(ref List<string> id, out Dictionary<string, StockInfoEntity> dict);//抽象方法 股票不带参数返回获取
        public abstract NET_ERROR StockGetWithCheck(ref List<string> id, out Dictionary<string, StockInfoEntity> dict);// 带返回参数获取
        public abstract NET_ERROR KchartImageGet(string id, kchart k, out Image image);//返回股票k线图
        public abstract NET_ERROR HistoryMoney(string id, DateTime date, DateTime enddate, out Dictionary<DateTime, double> money);//返回股票历史收盘价
        protected void UpdateLog(string stockid, NET_ERROR e)//更新日志
        {
            DateTime dt = DateTime.Now;
            if (log.Count >= 100)//日志限制100条
            {
                log.RemoveRange(0, log.Count - 100);
            }
            if (e == NET_ERROR.NET_CANT_CONNECT)
            {
                log.Add(dt.ToLongTimeString().ToString() + stockid + ":NET_CANT_CONNECT");
            }
            else if (e == NET_ERROR.NET_JSON_NOT_EXISTS)
            {
                log.Add(dt.ToLongTimeString().ToString() + stockid + ":NET_JSON_NOT_EXISTS");
            }
            else if (e == NET_ERROR.NET_REQ_ERROR)
            {
                log.Add(dt.ToLongTimeString().ToString() + stockid + ":NET_REQ_ERROR");
            }
        }
    }
}

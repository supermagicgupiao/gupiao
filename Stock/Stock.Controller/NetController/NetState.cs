using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.IO;

using Stock.Controller.NetController.StockAPI;

namespace Stock.Controller.NetController
{
    public class NetState
    {
        //网络检测
        public static NET_ERROR Check(string stockid)
        {
            string name;
            return CheckName(stockid, out name);
        }
        //获取股票名称
        public static NET_ERROR CheckName(string stockid,out string name)
        {
            StockInfo si = new Netease();
            List<string> id = new List<string>();
            id.Add(stockid);
            Dictionary<string, StockInfoEntity> ds;
            NET_ERROR NE = si.StockGetWithCheck(ref id, out ds);
            if (NE == NET_ERROR.NET_REQ_OK && ds.Count > 0)
                name = ds.First().Value.name;
            else
                name = "";
            return NE;
        }
        //批量获取名称
        public static NET_ERROR CheckName(List<string> stockid, out Dictionary<string, string> name)
        {
            StockInfo si = new Netease();
            Dictionary<string, StockInfoEntity> ds;
            NET_ERROR NE = si.StockGetWithCheck(ref stockid, out ds);
            name = new Dictionary<string, string>();
            if (NE == NET_ERROR.NET_REQ_OK)
                foreach(var s in ds)
                {
                    name[s.Key] = s.Value.name;
                }
            return NE;
        }
        //id转带sh或sz
        public static NET_ERROR IdConvert(ref string stockid)
        {
            List<string> shid = new List<string>();
            List<string> szid = new List<string>();
            shid.Add("0" + stockid);
            szid.Add("1" + stockid);
            StockInfo si = new Netease();
            Dictionary<string, StockInfoEntity> ds;
            NET_ERROR NE = si.StockGetWithCheck(ref shid, out ds);
            if (NE == NET_ERROR.NET_REQ_OK && ds.Count > 0)
                stockid = ds.First().Key;
            else
                return NE;
            NE = si.StockGetWithCheck(ref szid, out ds);
            if (NE == NET_ERROR.NET_REQ_OK && ds.Count > 0)
                stockid = ds.First().Key;
            return NE;
        }
        //批量转id
        public static NET_ERROR IdConvert(ref List<string> stockid)
        {
            List<string> shid=new List<string>();
            List<string> szid=new List<string>();
            foreach (string id in stockid)
            {
                shid.Add("0" + id);
                szid.Add("1" + id);
            }
            stockid.Clear();
            StockInfo si = new Netease();
            Dictionary<string, StockInfoEntity> ds;
            NET_ERROR NE = si.StockGetWithCheck(ref shid, out ds);
            if (NE == NET_ERROR.NET_REQ_OK)
                foreach (var s in ds)
                {
                    stockid.Add(s.Key);
                }
            else
                return NE;
            NE = si.StockGetWithCheck(ref szid, out ds);
            if (NE == NET_ERROR.NET_REQ_OK)
                foreach (var s in ds)
                {
                    stockid.Add(s.Key);
                }
            return NE;
        }
    }
}

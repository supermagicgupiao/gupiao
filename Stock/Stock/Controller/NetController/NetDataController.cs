using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.IO;
using System.Threading;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Text.RegularExpressions;
using System.Collections; 


namespace Stock.Controller.NetController
{
    class NetDataController
    {
        public delegate void sync(string[] s);
        private Dictionary<string, sync> stockdict = new Dictionary<string, sync>();
        private Timer t;
        public static string log;
        private int time = 5;
        public void StartRefresh()
        {
            t = new Timer(GetStockInfo, stockdict, 2, time * 1000);
        }
        public void StopRefresh()
        {
            t.Dispose();
        }
        public bool StockRefreshAdd(string stockid,ref sync s)
        {
            stockdict.Add(stockid, s);
            return true;
        }
        public bool StockRefreshDelete(string stockid)
        {
            stockdict.Remove(stockid);
            return true;
        }
        public bool StockRefreshClear()
        {
            stockdict.Clear();
            return true;
        }
        private void GetStockInfo(object stockdict)
        {
            Dictionary<string, sync> dict = (Dictionary<string, sync>)stockdict;
            if (dict.Count == 0)
                return;
            string url = "http://api.money.126.net/data/feed/";
            string stock = "";
            foreach (var s in dict)
            {
                stock += s.Key + ',';
            }
            WebClient client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/4.0");
            Stream data;
            try
            {
                data = client.OpenRead(url + stock);
            }
            catch (WebException)
            {
                UpdateLog(stock, NET_ERROR.NET_CANT_CONNECT);
                return;
            }
            StreamReader reader = new StreamReader(data);
            string str = reader.ReadToEnd();
            string pattern = @"\(.*?\)";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            string json = regex.Match(str).Value.Trim('(', ')');
            JObject jo;
            try
            {
                jo = (JObject)JsonConvert.DeserializeObject(json);
            }
            catch(JsonException)
            {
                UpdateLog(stock, NET_ERROR.NET_JSON_NOT_EXISTS);
                return;
            }
            foreach (var s in dict)
            {
                if (jo.Property(s.Key) == null)
                {
                    UpdateLog(stock, NET_ERROR.NET_REQ_ERROR);
                }
                else
                {
                    string[] d = new string[11];
                    JToken j = jo[s.Key];
                    d[0] = j["arrow"].ToString();
                    d[1] = j["high"].ToString();
                    d[2] = j["low"].ToString();
                    d[3] = j["open"].ToString();
                    d[4] = j["percent"].ToString();
                    d[5] = j["price"].ToString();
                    d[6] = j["time"].ToString();
                    d[7] = j["turnover"].ToString();
                    d[8] = j["updown"].ToString();
                    d[9] = j["volume"].ToString();
                    d[10] = j["yestclose"].ToString();
                    s.Value(d);
                }
            }
        }
        private void UpdateLog(string stockid,NET_ERROR e)
        {
            if (e == NET_ERROR.NET_CANT_CONNECT)
            {
                log += stockid + ":NET_CANT_CONNECT\n";
            }
            else if(e == NET_ERROR.NET_JSON_NOT_EXISTS)
            {
                log += stockid + ":NET_JSON_NOT_EXISTS\n";
            }
            else if(e == NET_ERROR.NET_REQ_ERROR)
            {
                log += stockid + ":NET_REQ_ERROR\n";
            }
        }
    }
}

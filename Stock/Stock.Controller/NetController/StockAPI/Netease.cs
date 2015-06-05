using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Stock.Controller.NetController;

namespace Stock.Controller.NetController.StockAPI
{
    //网易API
    class Netease : StockInfo
    {
        private string url = "http://api.money.126.net/data/feed/";//api地址
        public override NET_ERROR StockGetWithCheck(ref List<string> id, out Dictionary<string, StockInfoEntity> dict)
        {
            dict = new Dictionary<string, StockInfoEntity>();
            string stock = "";
            foreach (string s in id)
            {
                stock += s + ',';
            }
            WebClient client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/4.0");
            Stream data;
            try
            {
                data = client.OpenRead(url + stock);//构建批量获取地址
            }
            catch (WebException)
            {
                UpdateLog(stock, NET_ERROR.NET_CANT_CONNECT);
                return NET_ERROR.NET_CANT_CONNECT;//网络错误 无法连接
            }
            StreamReader reader = new StreamReader(data);//读取所有结果
            string str = reader.ReadToEnd();
            string json;
            try
            {
                string pattern = @"\(.*?\)";
                Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
                json = regex.Match(str).Value.Trim('(', ')');//正则获取json
            }
            catch (Exception)
            {
                UpdateLog(stock, NET_ERROR.NET_DATA_ERROR);
                return NET_ERROR.NET_DATA_ERROR;//获取json失败
            }
            JObject jo;
            try
            {
                jo = (JObject)JsonConvert.DeserializeObject(json);//解析json
            }
            catch (JsonException)
            {
                UpdateLog(stock, NET_ERROR.NET_JSON_NOT_EXISTS);
                return NET_ERROR.NET_JSON_NOT_EXISTS;//解析失败
            }
            StockInfoEntity SIE = new StockInfoEntity();
            if (jo.Count == 0)
            {
                UpdateLog(stock, NET_ERROR.NET_REQ_ERROR);
                return NET_ERROR.NET_REQ_ERROR;
            }
            foreach (string s in id)
            {
                if (jo.Property(s) == null)
                {
                    UpdateLog(s, NET_ERROR.NET_REQ_ERROR);//股票返回存在性检测
                }
                else
                {
                    try//尝试json读取股票数据
                    {
                        JToken j = jo[s];
                        SIE.name = j["name"].ToString();
                        SIE.arrow = j["arrow"].ToString();
                        SIE.high = j["high"].ToString();
                        SIE.low = j["low"].ToString();
                        SIE.open = j["open"].ToString();
                        SIE.percent = j["percent"].ToString();
                        SIE.price = j["price"].ToString();
                        SIE.time = j["time"].ToString();
                        SIE.turnover = j["turnover"].ToString();
                        SIE.updown = j["updown"].ToString();
                        SIE.volume = j["volume"].ToString();
                        SIE.yestclose = j["yestclose"].ToString();
                    }
                    catch (JsonException)//读取失败则放弃这股票
                    {
                        UpdateLog(s, NET_ERROR.NET_REQ_ERROR);//股票读取失败
                        continue;
                    }
                    dict.Add(s, SIE);//成功获取可用数据放入dict中返回
                }
            }
            return NET_ERROR.NET_REQ_OK;
        }
        public override NET_ERROR KchartImageGet(string id, kchart k, out Image image)
        {
            HttpWebRequest req;
            image = null;
            string kurl = KchartSwitch(k);
            if (kurl == "")
                return NET_ERROR.NET_REQ_ERROR;
            Stream stm;
            try
            {
                req = HttpWebRequest.Create(kurl + id + ".png") as HttpWebRequest;//构建获取地址
                stm = req.GetResponse().GetResponseStream();
            }
            catch (WebException)
            {
                UpdateLog(id, NET_ERROR.NET_CANT_CONNECT);
                return NET_ERROR.NET_CANT_CONNECT;//网络错误 无法连接
            }
            catch
            {
                return NET_ERROR.NET_DATA_ERROR;
            }
            try
            {
                image = Image.FromStream(stm);
            }
            catch
            {
                image = null;
                return NET_ERROR.NET_DATA_ERROR;
            }
            return NET_ERROR.NET_REQ_OK;
        }
        public override NET_ERROR HistoryMoney(string id, DateTime date, DateTime enddate, out Dictionary<DateTime, double> money)
        {
            money = new Dictionary<DateTime, double>();
            HttpWebRequest req;
            string hurl = HistoryMoneyUrl(id, date, enddate);
            if (hurl == "")
                return NET_ERROR.NET_REQ_ERROR;
            Stream stm;
            try
            {
                req = HttpWebRequest.Create(hurl) as HttpWebRequest;//构建获取地址
                stm = req.GetResponse().GetResponseStream();
            }
            catch (WebException)
            {
                UpdateLog(id, NET_ERROR.NET_CANT_CONNECT);
                return NET_ERROR.NET_CANT_CONNECT;//网络错误 无法连接
            }
            try
            {
                StreamReader reader = new StreamReader(stm);
                String line;
                reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    string[] s = line.Split(',');
                    string[] d = s[0].Split('-');
                    DateTime dt = new DateTime(Convert.ToInt32(d[0]), Convert.ToInt32(d[1]), Convert.ToInt32(d[2]));
                    money.Add(dt, Convert.ToDouble(s[3]));
                }
            }
            catch
            {
                return NET_ERROR.NET_DATA_ERROR;
            }
            return NET_ERROR.NET_REQ_OK;
        }
        private string KchartSwitch(kchart k)//k线图请求url选择
        {
            switch(k)
            {
                case kchart.time:
                    return "http://img1.money.126.net/chart/hs/time/540x360/";
                case kchart.week:
                    return "http://img1.money.126.net/chart/hs/kline/week/";
                case kchart.month:
                    return "http://img1.money.126.net/chart/hs/kline/month/";
                case kchart.day30:
                    return "http://img1.money.126.net/chart/hs/kline/day/30/";
                case kchart.day90:
                    return "http://img1.money.126.net/chart/hs/kline/day/90/";
                case kchart.day180:
                    return "http://img1.money.126.net/chart/hs/kline/day/180/";
                default:
                    return "";
            }
        }
        private string HistoryMoneyUrl(string id, DateTime date, DateTime enddate)//k线图请求url选择
        {
            return "http://quotes.money.163.com/service/chddata.html?code=" + id + "&start=" + date.ToString("yyyyMMdd") + "&end=" + enddate.ToString("yyyyMMdd") + "&fields=TCLOSE";
        }
        public override bool StockGet(ref List<string> id, out Dictionary<string, StockInfoEntity> dict)
        {
            if (StockGetWithCheck(ref id, out dict) != NET_ERROR.NET_REQ_OK)//成功获取则返回true
                return false;
            return true;
        }
    }
}

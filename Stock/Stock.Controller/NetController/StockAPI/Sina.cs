using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;

namespace Stock.Controller.NetController.StockAPI
{
    class Sina : StockInfo
    {
        private string url = "http://hq.sinajs.cn/list=";//api地址
        public override NET_ERROR StockGetWithCheck(ref List<string> id, out Dictionary<string, StockInfoEntity> dict)
        {
            dict = new Dictionary<string, StockInfoEntity>();
            string stock = "";
            foreach (string s in id)
            {
                if (s[0] == '0')
                    stock += "sh" + s.Substring(1) + ',';
                else
                    stock += "sz" + s.Substring(1) + ',';
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
            string str;
            StockInfoEntity SIE = new StockInfoEntity();
            while ((str = reader.ReadLine()) != null)
            {
                string backid = str.Substring(12, 7);
                if (backid[0] == 'h')
                    backid = backid.Replace('h', '0');
                else if (backid[0] == 'z')
                    backid = backid.Replace('z', '1');
                else
                    continue;
                str = str.Substring(str.IndexOf("\"") + 1);
                string[] sArray = str.Split(',');
                if(sArray.Length < 33)
                    continue;
                SIE.name = sArray[0];
                SIE.open = sArray[1];
                SIE.yestclose = sArray[2];
                SIE.price = sArray[3];
                SIE.high = sArray[4];
                SIE.low = sArray[5];
                SIE.volume = sArray[8];
                SIE.turnover = sArray[9];
                double dvalue = Convert.ToDouble(sArray[3]) - Convert.ToDouble(sArray[2]);
                SIE.updown = dvalue.ToString();
                double per = dvalue / Convert.ToDouble(sArray[2]);
                SIE.percent = per > 0 ? per.ToString() : (-per).ToString();
                SIE.arrow = per > 0 ? "↑" : "↓";
                SIE.time = sArray[30] + " " + sArray[31];
                dict.Add(backid, SIE);
            }
            return NET_ERROR.NET_REQ_OK;
        }
        public override NET_ERROR HistoryMoney(string id, DateTime date, DateTime enddate, out Dictionary<DateTime, double> money)
        {
            Netease si = new Netease();
            return si.HistoryMoney(id, date, enddate, out money);
        }
        public override NET_ERROR KchartImageGet(string id, kchart k, out Image image)
        {
            HttpWebRequest req;
            image = null;
            string kurl = KchartSwitch(k);
            if (kurl == "")
                return NET_ERROR.NET_REQ_ERROR;
            string stock = "";
            if (id[0] == '0')
                stock += "sh" + id.Substring(1);
            else
                stock += "sz" + id.Substring(1);
            Stream stm;
            try
            {
                req = HttpWebRequest.Create(kurl + stock + ".gif") as HttpWebRequest;//构建获取地址
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
        public override bool StockGet(ref List<string> id, out Dictionary<string, StockInfoEntity> dict)
        {
            if (StockGetWithCheck(ref id, out dict) != NET_ERROR.NET_REQ_OK)//成功获取则返回true
                return false;
            return true;
        }

        private string KchartSwitch(kchart k)//k线图请求url选择
        {
            switch (k)
            {
                case kchart.time:
                    return "http://image.sinajs.cn/newchart/min/n/";
                case kchart.week:
                    return "http://image.sinajs.cn/newchart/weekly/n/";
                case kchart.month:
                    return "http://image.sinajs.cn/newchart/monthly/n/";
                case kchart.day30:
                    return "";
                case kchart.day90:
                    return "";
                case kchart.day180:
                    return "";
                default:
                    return "";
            }
        }
    }
}

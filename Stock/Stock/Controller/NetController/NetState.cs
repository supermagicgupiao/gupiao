using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Text.RegularExpressions;

namespace Stock.Controller.NetController
{
    class NetState
    {
        public static NET_ERROR Check(string stockid)
        {
            string name = "";
            return CheckName(stockid, ref name);
        }
        public static NET_ERROR CheckName(string stockid,ref string name)
        {
            string url = "http://api.money.126.net/data/feed/" + stockid;
            WebClient client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/4.0");
            Stream data;
            try
            {
                data = client.OpenRead(url);
            }
            catch (WebException)
            {
                return NET_ERROR.NET_CANT_CONNECT;
            }
            StreamReader reader = new StreamReader(data);
            string s = reader.ReadToEnd();
            string pattern = @"\(.*?\)";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            string json = regex.Match(s).Value.Trim('(',')');
            JObject jo;
            try
            {
                jo = (JObject)JsonConvert.DeserializeObject(json);
            }
            catch(JsonException)
            {
                return NET_ERROR.NET_JSON_NOT_EXISTS;
            }
            if (jo.Property(stockid) == null)
            {
                return NET_ERROR.NET_REQ_ERROR;
            }
            else
            {
                name = jo[stockid]["name"].ToString();
                return NET_ERROR.NET_REQ_OK;
            }
        }
    }
    enum NET_ERROR : byte
    {
        NET_REQ_OK,
        NET_CANT_CONNECT,
        NET_JSON_NOT_EXISTS,
        NET_REQ_ERROR,
    }
}

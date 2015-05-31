using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Controls;
using System.Windows;

using Stock.Controller.NetController;

namespace Stock.UIController
{
    class StockStateBoxController
    {
        private Canvas canvas;
        private static StockStateBoxController SSBC;
        private StockStateBoxController() { }
        public static StockStateBoxController Create(ref Canvas canvas)
        {
            if (SSBC == null)
            {
                SSBC = new StockStateBoxController();
                SSBC.canvas = canvas;
                return SSBC;
            }
            else
                return SSBC;
        }
        public static StockStateBoxController Handler()
        {
            return SSBC;
        }
        public static void setCanvas(ref Canvas canvas)
        {
            SSBC.canvas = canvas;
        }
        public bool Add(string id,string name,int hold,double money)
        {
            double height = -5;
            if (StockStateBox.pre != null)
                height = StockStateBox.pre.Margin.Top + StockStateBox.pre.Height;
            StockStateBox box = new StockStateBox();
            box.Margin = new Thickness(5, height + 10, 0, 0);
            box.stockid = id;
            canvas.Children.Add(box);

            string StockID = "";
            string netname;
            if (NetState.CheckName("0" + id, out netname) == NET_ERROR.NET_REQ_OK)
            {
                StockID = "0" + id;
                box.StockName.Text = netname.Insert(2, "\r\n");
            }
            else if (NetState.CheckName("1" + id, out netname) == NET_ERROR.NET_REQ_OK)
            {
                StockID = "1" + id;
                box.StockName.Text = netname.Insert(2, "\r\n");
            }
            else
            {
                MessageBox.Show("股票编号:" + id + "错误！");
                box.StockName.Text = name.Insert(2, "\r\n");
                return false;
            }

            box.hold.Text = hold.ToString();
            box.basemoney = money;
            NetDataController.sync s = new NetDataController.sync(box.UpdataSync);
            if (!NetSyncController.Handler().StockRefreshAdd(StockID, ref s))
            {
                NetSyncController.Handler().StockRefreshDelete(StockID);
                NetSyncController.Handler().StockRefreshAdd(StockID, ref s);
            }
            return true;
        }
        public double ExistsRemove(string id)
        {
            double height = -5;
            foreach(StockStateBox SSB in canvas.Children)
            {
                if (SSB.stockid == id)
                {
                    if (StockStateBox.pre == SSB)
                        height = SSB.Margin.Top;
                    canvas.Children.Remove(SSB);
                    return height;
                }
            }
            return height;
        }
        public bool Change(string id,string type,int number,double money)
        {
            foreach (StockStateBox SSB in canvas.Children)
            {
                if (SSB.stockid == id)
                {
                    if (type == "买入" || type == "卖空")
                    {
                        SSB.hold.Text = (Convert.ToInt32(SSB.hold.Text) + number).ToString();
                        SSB.basemoney += number * money;
                    }
                    else
                    {
                        SSB.hold.Text = (Convert.ToInt32(SSB.hold.Text) - number).ToString();
                        SSB.basemoney -= number * money;
                    }
                    NetSyncController.Handler().GetStockInfoNow();
                    return true;
                }
            }
            return false;
        }
    }
}

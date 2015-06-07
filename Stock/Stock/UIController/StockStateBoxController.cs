using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Controls;
using System.Windows;

using Stock.Controller.NetController;
using Stock.Controller.DBController;
using Stock.Controller.DBController.DBTable;

namespace Stock.UIController
{
    class StockStateBoxController
    {
        private List<StockStateBox> canvasbox;
        private Canvas canvas;
        private static StockStateBoxController SSBC;
        public static InfoDelegate ID;
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
        public void StockBoxInit()
        {
            StockStateBox.pre = null;
            canvasbox.Clear();
            canvas.Children.Clear();
            List<StockHoldEntity> SHEL;
            DBSyncController.Handler().StockHoldReadAll(out SHEL);
            foreach (StockHoldEntity SHE in SHEL)
            {
                StockStateBoxController.Handler().Add(SHE.id, SHE.name, SHE.hold, SHE.money);
            }
            NetSyncController.Handler().StartRefresh();
        }
        public void setCanvas(ref List<StockStateBox> boxlist)
        {
            this.canvasbox = boxlist;
            canvas.Children.Clear();
            if (canvasbox.Count != 0)
                StockStateBox.pre = canvasbox.Last();
            else
                StockStateBox.pre = null;
            foreach (StockStateBox item in canvasbox)
            {
                canvas.Children.Add(item);
            }
        }
        public bool Add(string id,string name,int hold,double money)
        {
            double height = -5;
            if (StockStateBox.pre != null)
                height = StockStateBox.pre.Margin.Top + StockStateBox.pre.Height;
            StockStateBox box = new StockStateBox(new StockStateBox.ChangeValues(ID.change));
            box.Margin = new Thickness(5, height + 10, 0, 0);
            box.stockid = id;

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

            canvas.Children.Add(box);
            canvasbox.Add(box);
            return true;
        }
        //public double ExistsRemove(string id)
        //{
        //    double height = -5;
        //    foreach(StockStateBox SSB in canvas.Children)
        //    {
        //        if (SSB.stockid == id)
        //        {
        //            if (StockStateBox.pre == SSB)
        //                height = SSB.Margin.Top;
        //            canvas.Children.Remove(SSB);
        //            canvasbox.Remove(SSB);
        //            return height;
        //        }
        //    }
        //    return height;
        //}
        public void GetDelegateValues(StockHoldEntity SHE)
        {
            if (!Change(SHE.id, SHE.hold, SHE.money))
            {
                Add(SHE.id, SHE.name, SHE.hold, SHE.money);
            }
        }

        public bool Change(string id, int hold, double money)
        {
            foreach (StockStateBox SSB in canvas.Children)
            {
                if (SSB.stockid == id)
                {

                    SSB.hold.Text = hold.ToString();
                    SSB.basemoney = money;
                    NetSyncController.Handler().GetStockInfoNow();
                    return true;
                }
            }
            return false;
        }
    }
}

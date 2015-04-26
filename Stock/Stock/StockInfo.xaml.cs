using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Stock.Controller.NetController;

namespace Stock
{
    /// <summary>
    /// StockInfo.xaml 的交互逻辑
    /// </summary>
    public partial class StockInfo : Window
    {
        public StockInfo()
        {
            InitializeComponent();
        }

        private void Min_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Close_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (e.GetPosition((IInputElement)sender).Y < title.Height.Value)
                {
                    this.DragMove();
                }
            }
        }
        private void thsync(TextBlock tb,string s)
        {
            tb.Text = s;
        }
        private void UpdataSync(string[] s)
        {
            Action<TextBlock, String> updateAction = new Action<TextBlock, string>(thsync);
            arrow.Dispatcher.BeginInvoke(updateAction, arrow, s[0]);
            high.Dispatcher.BeginInvoke(updateAction, high, s[1]);
            low.Dispatcher.BeginInvoke(updateAction, low, s[2]);
            open.Dispatcher.BeginInvoke(updateAction, open, s[3]);
            percent.Dispatcher.BeginInvoke(updateAction, percent, Convert.ToString(Convert.ToDouble(s[4]) * 100) + "%");
            price.Dispatcher.BeginInvoke(updateAction, price, s[5]);
            time.Dispatcher.BeginInvoke(updateAction, time, s[6]);
            turnover.Dispatcher.BeginInvoke(updateAction, turnover, s[7]);
            updown.Dispatcher.BeginInvoke(updateAction, updown, s[8]);
            volume.Dispatcher.BeginInvoke(updateAction, volume, s[9]);
            yestclose.Dispatcher.BeginInvoke(updateAction, yestclose, s[10]);
        }
        private NetDataController netdc = new NetDataController();
        public string StockID;
        public string C_StockID;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string StockName = "";
            string S_StockID = "";
            if (NetState.CheckName("0" + StockID, ref StockName) == NET_ERROR.NET_REQ_OK)
            {
                S_StockID = "sh" + StockID;
                C_StockID = "0" + StockID;
            }
            else if (NetState.CheckName("1" + StockID, ref StockName) == NET_ERROR.NET_REQ_OK)
            {
                S_StockID = "sz" + StockID;
                C_StockID = "1" + StockID;
            }
            else
            {
                MessageBox.Show("股票编号不存在或者网络异常！");
                netdc.StartRefresh();
                this.Close();
                return;
            }
            this.Left = (SystemParameters.PrimaryScreenWidth - this.ActualWidth) / 2;
            this.Top = (SystemParameters.PrimaryScreenHeight - this.ActualHeight) / 2;
            StockTitle.Text = "股票:" + StockName + "(" + S_StockID + ")";
            NetDataController.sync s = new NetDataController.sync(UpdataSync);
            netdc.StockRefreshAdd(C_StockID, ref s);
            netdc.StartRefresh();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            netdc.StopRefresh();
        }
    }
}

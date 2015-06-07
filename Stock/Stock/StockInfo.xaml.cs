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
using Stock.UIController;

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
        private void TextBoxSync(TextBlock tb,string s)
        {
            tb.Text = s;
        }
        private void UpdataSync(StockInfoEntity SIE)
        {
            Action<TextBlock, String> updateAction = new Action<TextBlock, string>(TextBoxSync);
            arrow.Dispatcher.BeginInvoke(updateAction, arrow, SIE.arrow);
            high.Dispatcher.BeginInvoke(updateAction, high, SIE.high);
            low.Dispatcher.BeginInvoke(updateAction, low, SIE.low);
            open.Dispatcher.BeginInvoke(updateAction, open, SIE.open);
            percent.Dispatcher.BeginInvoke(updateAction, percent, Adapter.DataAdapter.RealTwo(Convert.ToDouble(SIE.percent) * 100) + "%");
            price.Dispatcher.BeginInvoke(updateAction, price, SIE.price);
            time.Dispatcher.BeginInvoke(updateAction, time, SIE.time);
            turnover.Dispatcher.BeginInvoke(updateAction, turnover, Adapter.DataAdapter.RealTwo(Convert.ToDouble(SIE.turnover) / 100) + "手");
            updown.Dispatcher.BeginInvoke(updateAction, updown, Adapter.DataAdapter.RealTwo(Convert.ToDouble(SIE.updown)));
            volume.Dispatcher.BeginInvoke(updateAction, volume, Adapter.DataAdapter.RealTwo(Convert.ToDouble(SIE.volume) / 10000) + "万");
            yestclose.Dispatcher.BeginInvoke(updateAction, yestclose, SIE.yestclose);
        }
        private void ImageSync(Image image,System.Drawing.Image bimage)
        {
            if(bimage!=null)
            {
                image.Source = Adapter.ImageAdapter.ImageConvert(bimage); 
            }
        }
        private void UpdataImage(System.Drawing.Image image)
        {
            Action<Image, System.Drawing.Image> updateAction = new Action<Image, System.Drawing.Image>(ImageSync);
            k.Dispatcher.BeginInvoke(updateAction, k, image);
        }
        public string StockID;
        public string C_StockID;
        public string StockName;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string S_StockID;
            if (NetState.CheckName("0" + StockID, out StockName) == NET_ERROR.NET_REQ_OK)
            {
                S_StockID = "sh" + StockID;
                C_StockID = "0" + StockID;
            }
            else if (NetState.CheckName("1" + StockID, out StockName) == NET_ERROR.NET_REQ_OK)
            {
                S_StockID = "sz" + StockID;
                C_StockID = "1" + StockID;
            }
            else
            {
                MessageBox.Show("股票编号不存在或者网络异常！");
                this.Close();
                return;
            }
            this.Left = (SystemParameters.PrimaryScreenWidth - this.ActualWidth) / 2;
            this.Top = (SystemParameters.PrimaryScreenHeight - this.ActualHeight) / 2;
            StockTitle.Text = "股票:" + StockName + "(" + S_StockID + ")";
            NetDataController.backimage bimage = new NetDataController.backimage(UpdataImage);
            kchart k = kchart.time;
            NetSyncController.Handler().KchartImageGet(C_StockID, k, bimage);
            NetDataController.sync s = new NetDataController.sync(UpdataSync);
            NetSyncController.Handler().StockTempRefreshAdd(C_StockID, ref s);
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            NetSyncController.Handler().StockTempRefreshDelete(C_StockID);
        }

        private void GetKchart(kchart k)
        {
            this.k.Source = null;
            NetDataController.backimage bimage = new NetDataController.backimage(UpdataImage);
            NetSyncController.Handler().KchartImageGet(C_StockID, k, bimage);
        }

        private void TIME_Click(object sender, RoutedEventArgs e)
        {
            kchart k = kchart.time;
            GetKchart(k);
        }

        private void WEEK_Click(object sender, RoutedEventArgs e)
        {
            kchart k = kchart.week;
            GetKchart(k);
        }
        private void MONTH_Click(object sender, RoutedEventArgs e)
        {
            kchart k = kchart.month;
            GetKchart(k);
        }
        private void DAY30_Click(object sender, RoutedEventArgs e)
        {
            kchart k = kchart.day30;
            GetKchart(k);
        }
        private void DAY90_Click(object sender, RoutedEventArgs e)
        {
            kchart k = kchart.day90;
            GetKchart(k);
        }
        private void DAY180_Click(object sender, RoutedEventArgs e)
        {
            kchart k = kchart.day180;
            GetKchart(k);
        }
        private void AddDealList_Click(object sender, RoutedEventArgs e)
        {
            AddDealList ADL = new AddDealList();
            ADL.name.Text = StockName;
            ADL.id.Text = StockID;
            ADL.date.Text = DateTime.Now.ToString();
            ADL.money.Text = price.Text;
            ADL.taxrate.Text = "1‰";
            ADL.commission.Text = "0.3‰";
            ADL.ShowDialog();
        }

        private void ShowDealList_Click(object sender, RoutedEventArgs e)
        {
            DealList dl = new DealList(StockID, StockName);
        }
    }
}

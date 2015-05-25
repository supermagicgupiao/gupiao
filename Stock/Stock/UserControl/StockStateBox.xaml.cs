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
using System.Windows.Navigation;
using System.Windows.Shapes;

using Stock.Controller.NetController;
using Stock.UIController;

namespace Stock
{
    /// <summary>
    /// StockStateBox.xaml 的交互逻辑
    /// </summary>
    public partial class StockStateBox : UserControl
    {
        public static StockStateBox pre = null;
        public double basemoney;
        public StockStateBox()
        {
            InitializeComponent();
            pre = this;
        }
        private double totalMark = 0;
        private double priceMark = 0;
        private void pricesync(TextBox tb, string s)
        {
            tb.Text = s;
            double m = Convert.ToDouble(hold.Text) * Convert.ToDouble(s);
            money.Text = m.ToString();
            double ud = m - basemoney;
            updown.Text = Adapter.DataAdapter.RealTwo(ud);

            DBSyncController.Handler().TotalChange(ud - totalMark);
            MainWindow.price += m - priceMark;
            if (pre == this)
            {
                DBSyncController.Handler().MoneyReadSet();
            }
            totalMark = ud;
            priceMark = m;
        }

        private double upwinMark = 0;
        private double daywinMark = 0;
        private void updownsync(string s)
        {
            double temp = Convert.ToDouble(hold.Text) * Convert.ToDouble(s) * Convert.ToDouble(price.Text) / 100;
            MainWindow.daywin += temp - daywinMark;
            daywinMark = temp;
            MainWindow.upwin = Convert.ToDouble(hold.Text) * (Convert.ToDouble(s) - upwinMark) * Convert.ToDouble(price.Text) / 100;
            upwinMark = Convert.ToDouble(s);
        }
        public void UpdataSync(StockInfoEntity SIE)
        {
            Action<TextBox, String> updateAction = new Action<TextBox, string>(pricesync);
            price.Dispatcher.BeginInvoke(updateAction, price, SIE.price);

            Action<String> up = new Action<string>(updownsync);
            this.Dispatcher.BeginInvoke(up, SIE.updown);
        }
        public string stockid;
        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            StockInfo dlg = new StockInfo();
            dlg.StockID = ((StockStateBox)sender).stockid;
            dlg.Show();
        }
    }
}

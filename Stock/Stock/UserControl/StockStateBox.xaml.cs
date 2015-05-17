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

namespace Stock
{
    /// <summary>
    /// StockStateBox.xaml 的交互逻辑
    /// </summary>
    public partial class StockStateBox : UserControl
    {
        public static StockStateBox pre;
        public double basemoney;
        public StockStateBox()
        {
            InitializeComponent();
            pre = this;
        }
        private void pricesync(TextBox tb, string s)
        {
            tb.Text = s;
            double m = Convert.ToDouble(hold.Text) * Convert.ToDouble(s);
            money.Text = m.ToString();
//          updown.Text = String.Format("{0:F} %", m * 100 / basemoney);  
        }
        private void updownsync(TextBox tb, string s)
        {
            updown.Text = s + "%";
        }
        public void UpdataSync(StockInfoEntity SIE)
        {
            Action<TextBox, String> updateAction0 = new Action<TextBox, string>(pricesync);
            price.Dispatcher.BeginInvoke(updateAction0, price, SIE.price);
            Action<TextBox, String> updateAction1 = new Action<TextBox, string>(updownsync);
            updown.Dispatcher.BeginInvoke(updateAction1, updown, SIE.updown);
        }
        public event EventHandler UEvent;
        public string stockid;
        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (UEvent != null)
            {
                UEvent(this, e);
            }
        }
    }
}

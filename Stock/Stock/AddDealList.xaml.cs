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

using Stock.Controller.DBController.DBTable;

using Stock.Controller.NetController;

namespace Stock
{
    /// <summary>
    /// AddDealList.xaml 的交互逻辑
    /// </summary>
    public partial class AddDealList : Window
    {
        public AddDealList()
        {
            InitializeComponent();
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

        private void Min_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Close_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Convert.ToDouble(money.Text);
                if(number.Text=="0"||Convert.ToInt32(number.Text)%100!=0)
                    throw new Exception();
                if(taxrate.Text[taxrate.Text.Length-1]!='‰')
                    throw new Exception();
                if(commission.Text[commission.Text.Length-1]!='‰')
                    throw new Exception();
            }
            catch(Exception)
            {
                MessageBox.Show("输入的数据有误\n请检查后输入");
                return;
            }
            DealListEntity DLE;
            DLE.name = name.Text;
            DLE.id = id.Text;
            DLE.date = date.Text;
            DLE.type = type.Text;
            DLE.money = money.Text;
            DLE.number = number.Text;
            DLE.taxrate = taxrate.Text;
            DLE.commission = commission.Text;
            DLE.explain = explain.Text;
            DLE.remark = remark.Text;
            MainWindow.dbc.DealListAdd(DLE);
            MessageBox.Show("添加成功!");
            UIElementCollection uc = ((MainWindow)Owner).StockCanvas.Children;
            foreach(StockStateBox b in uc)
            {
                if (b.stockid == id.Text)
                    this.Close();
            }
            StockStateBox box = new StockStateBox();
            box.Margin = new Thickness(5, 5 + (10 + box.Height) * StockStateBox.count, 0, 0);
            box.stockid = id.Text;
            box.UEvent += new EventHandler(((MainWindow)Owner).uEvent);
            uc.Add(box);

            string stockid = id.Text;
            string StockID = "";
            string Stockname;
            if (NetState.CheckName("0" + stockid, out Stockname) == NET_ERROR.NET_REQ_OK)
            {
                StockID = "0" + stockid;
                box.StockName.Text = Stockname.Insert(2, "\r\n");
            }
            else if (NetState.CheckName("1" + stockid, out Stockname) == NET_ERROR.NET_REQ_OK)
            {
                StockID = "1" + stockid;
                box.StockName.Text = Stockname.Insert(2, "\r\n");
            }
            else
            {
                MessageBox.Show("股票编号:" + stockid + "错误！");
                box.StockName.Text = name.Text.Insert(2, "\r\n");
            }
            box.hold.Text = number.Text;
            box.basemoney = Convert.ToDouble(money.Text);
            NetDataController.sync s = new NetDataController.sync(box.UpdataSync);
            ((MainWindow)Owner).netdc.StockRefreshAdd(StockID, ref s);
            this.Close();
        }
    }
}

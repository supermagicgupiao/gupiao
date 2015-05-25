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

using Stock.UIController;

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
                if (number.Text == "0")
                    throw new Exception();
                if (type.Text == "买入" || type.Text == "卖空")
                {
                    if (Convert.ToInt32(number.Text) % 100 != 0)
                        throw new Exception();
                }
                else
                {
                    StockHoldEntity SHE = new StockHoldEntity();
                    SHE.id = id.Text;
                    DBSyncController.Handler().StockHoldRead(ref SHE);
                    if (SHE.hold < Convert.ToInt32(number.Text))
                    {
                        MessageBox.Show("卖出或补仓数目超出范围!");
                        return;
                    }
                }
                if (taxrate.Text[taxrate.Text.Length - 1] != '‰')
                    throw new Exception();
                if (commission.Text[commission.Text.Length - 1] != '‰')
                    throw new Exception();
            }
            catch (Exception)
            {
                MessageBox.Show("输入的数据有误\n请检查后输入");
                return;
            }
            DealListEntity DLE;
            DLE.name = name.Text;
            DLE.id = id.Text;
            DLE.date = Convert.ToDateTime(date.Text);
            DLE.type = type.Text;
            DLE.money = Convert.ToDouble(money.Text);
            DLE.number = Convert.ToInt32(number.Text);
            DLE.taxrate = Convert.ToDouble(taxrate.Text.Substring(0,taxrate.Text.IndexOf("‰")));
            DLE.commission = Convert.ToDouble(commission.Text.Substring(0,commission.Text.IndexOf("‰")));
            DLE.explain = explain.Text;
            DLE.remark = remark.Text;
            DBSyncController.Handler().DealListAdd(DLE);
            //MessageBox.Show("添加成功!");
            if (StockStateBoxController.Handler().Change(id.Text, type.Text, DLE.number, DLE.money))
            {
                this.Close();
                return;
            }
            StockStateBoxController.Handler().Add(id.Text, name.Text, Convert.ToInt32(number.Text), Convert.ToDouble(money.Text) * Convert.ToInt32(number.Text));
            this.Close();
        }
    }
}

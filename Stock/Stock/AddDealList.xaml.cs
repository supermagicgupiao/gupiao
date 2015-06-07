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
            List<string> user = UserPanelController.Handler().GetUserList();
            int index = -1;
            string u = DBSyncController.Handler().GetUserName();
            foreach (string s in user)
            {
                index++;
                if (s == u)
                    this.user.SelectedIndex = index;
                ComboBoxItem cbi = new ComboBoxItem();
                cbi.Content = s;
                this.user.Items.Add(cbi);
            }
            //List<string> user = UserPanelController.Handler().GetUserList();
            //foreach(string s in user)
            //{
            //    ComboBoxItem cbi = new ComboBoxItem();
            //    cbi.Content = s;
            //    this.user.Items.Add(cbi);
            //}
            change.Visibility = Visibility.Hidden;
            delete.Visibility = Visibility.Hidden;
        }
        private int deal = 0;
        public AddDealList(string user, int deal)
        {
            InitializeComponent();
            this.titlename.Text = "修改交易";
            add.Visibility = Visibility.Hidden;
            ComboBoxItem cbi = new ComboBoxItem();
            cbi.Content = user;
            this.user.Items.Add(cbi);
            this.user.SelectedIndex = 0;
            this.user.IsEnabled = false;
            DealListEntity DLE = UserPanelController.Handler().DBControllerByName(user).DealListReadByDeal(deal);
            this.deal = DLE.deal;
            name.Text = DLE.name;
            id.Text = DLE.id;
            date.Text = DLE.date.ToString();
            type.Text = DLE.type;
            money.Text = DLE.money.ToString();
            number.Text = DLE.number.ToString();
            taxrate.Text = DLE.taxrate.ToString() + "‰";
            commission.Text = DLE.commission.ToString() + "‰";
            explain.Text = DLE.explain;
            remark.Text = DLE.remark;
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
            string u = user.Text;
            try
            {
                Convert.ToDouble(money.Text);
                if (number.Text == "0")
                    throw new Exception();
                if (type.Text == "买入" || type.Text == "补仓")
                {
                    int buy = Convert.ToInt32(number.Text);
                    if (buy % 100 != 0)
                        throw new Exception();
                    if (buy * Convert.ToDouble(money.Text) > UserPanelController.Handler().DBControllerByName(u).NowMoneyRead()) 
                    {
                        MessageBox.Show("现金不足!");
                        return;
                    }
                }
                else if (type.Text == "卖出")
                {
                    StockHoldEntity SHE = new StockHoldEntity();
                    SHE.id = id.Text;
                    UserPanelController.Handler().DBControllerByName(u).StockHoldRead(ref SHE);
                    if (SHE.hold < Convert.ToInt32(number.Text))
                    {
                        MessageBox.Show("卖出数目超出范围!");
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
            DLE.deal = deal;
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
            UserPanelController.Handler().UserChange(u);
            UserPanelController.Handler().DBControllerByName(u).DealListAdd(DLE);
            //MessageBox.Show("添加成功!");
            //StockStateBoxController.Handler().Add(id.Text, name.Text, Convert.ToInt32(number.Text), Convert.ToDouble(money.Text) * Convert.ToInt32(number.Text));
            this.Close();
        }
        private void CButton_Click(object sender, RoutedEventArgs e)
        {
            string u = user.Text;
            try
            {
                Convert.ToDouble(money.Text);
                if (number.Text == "0")
                    throw new Exception();
                if (type.Text == "买入" || type.Text == "补仓")
                {
                    int buy = Convert.ToInt32(number.Text);
                    if (buy % 100 != 0)
                        throw new Exception();
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
            DLE.deal = deal;
            DLE.name = name.Text;
            DLE.id = id.Text;
            DLE.date = Convert.ToDateTime(date.Text);
            DLE.type = type.Text;
            DLE.money = Convert.ToDouble(money.Text);
            DLE.number = Convert.ToInt32(number.Text);
            DLE.taxrate = Convert.ToDouble(taxrate.Text.Substring(0, taxrate.Text.IndexOf("‰")));
            DLE.commission = Convert.ToDouble(commission.Text.Substring(0, commission.Text.IndexOf("‰")));
            DLE.explain = explain.Text;
            DLE.remark = remark.Text;
            string t = ((Button)sender).Content.ToString();
            if (t == "删除")
                UserPanelController.Handler().DBControllerByName(u).DealListDelete(DLE);
            else if (t == "修改")
                UserPanelController.Handler().DBControllerByName(u).DealListUpdate(DLE);
            this.Close();
        }
    }
}

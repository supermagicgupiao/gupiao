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

using Stock.UIController;

namespace Stock
{
    /// <summary>
    /// UserBox.xaml 的交互逻辑
    /// </summary>
    public partial class UserBox : UserControl
    {
        public static UserBox pre = null;
        public UserBox(string name,string pri)
        {
            InitializeComponent();
            pre = this;
            this.name.Content = name;
            this.pri.Content = "本金:" + pri;
        }
        public UserBox(string name)
        {
            InitializeComponent();
            this.name.Content = name;
            this.pri.Content = "";
            this.cm.Visibility = Visibility.Hidden;
//          this.item.Click -= new RoutedEventHandler(MenuItem_Click);
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            choose.BorderBrush = Brushes.Green;
            choose.BorderThickness = new Thickness(2);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            choose.BorderBrush = Brushes.DarkGray;
            choose.BorderThickness = new Thickness(1);
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //InputMoney im = new InputMoney();
            //im.name.Text = this.name.Content.ToString();
            //im.name.IsEnabled = false;
            //im.ShowDialog();
            //double now = DBSyncController.Handler().NowMoneyRead();
            //double pri = DBSyncController.Handler().PrincipalRead();
            //if (im.m == 0)
            //{
            //    return;
            //}
            //if (pri - im.m > now)
            //{
            //    MessageBox.Show("最低本金设置为:" + (pri - now).ToString());
            //    return;
            //}
            //this.pri.Content = "本金:" + im.m;
            //DBSyncController.Handler().PrincipalChange(im.m - pri);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            InputMoney im = new InputMoney();
            im.name.Text = this.name.Content.ToString();
            im.name.IsEnabled = false;
            string u = name.Content.ToString();
            double pri = UserPanelController.Handler().DBControllerByName(u).PrincipalRead();
            im.money.Text = pri.ToString();
            im.ShowDialog();
            double now = UserPanelController.Handler().DBControllerByName(u).NowMoneyRead();

            if (im.m == 0)
            {
                return;
            }
            if (pri - im.m > now)
            {
                MessageBox.Show("最低本金设置为:" + (pri - now).ToString());
                return;
            }
            this.pri.Content = "本金:" + im.m;
            UserPanelController.Handler().DBControllerByName(u).PrincipalChange(im.m - pri);
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = MessageBox.Show("是否删除账户:" + name.Content.ToString(), "删除账户", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            if (res == MessageBoxResult.Yes)
                UserPanelController.Handler().DelUser(name.Content.ToString());
        }

    }
}

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
    }
}

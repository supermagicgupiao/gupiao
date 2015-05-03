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

namespace Stock
{
    /// <summary>
    /// InputMoney.xaml 的交互逻辑
    /// </summary>
    public partial class InputMoney : Window
    {
        public InputMoney()
        {
            InitializeComponent();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            m = 0;
            this.Close();
        }
        public double m;
        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            if (Double.TryParse(money.Text, out m))
                this.Close();
            else
                MessageBox.Show("错误的本金金额！");
        }
    }
}

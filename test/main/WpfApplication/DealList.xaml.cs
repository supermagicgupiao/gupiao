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

namespace WpfApplication
{
    /// <summary>
    /// DealList.xaml 的交互逻辑
    /// </summary>
    public partial class DealList : Window
    {
        public DealList()
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
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string[] listheader = new string[] { "股票名称", "股票编号", "日期", "类型", "价格", "数量", "税率", "佣金", "说明", "备注", "操作" };
            GridViewColumn[] h = new GridViewColumn[listheader.Length];
            for (int i = 0; i < listheader.Length; i++) 
            {
                h[i] = new GridViewColumn();
                h[i].Header = listheader[i];
                h[i].Width = 60;
                ListHeader.Columns.Add(h[i]);
            }
        }
    }
}

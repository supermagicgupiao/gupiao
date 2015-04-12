using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Win32;

namespace WpfApplication
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
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
            Application.Current.Shutdown();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int id;
            if (int.TryParse(StockID.Text, out id) == false)
            {
                MessageBox.Show("错误的股票编号输入\n请输入纯数字编号");
                return;
            }
            StockInfo dlg = new StockInfo();
            dlg.StockID = StockID.Text;
            dlg.Show();
        }

        private void StockID_GotFocus(object sender, RoutedEventArgs e)
        {
            if (StockID.Text == "搜索:请输入股票编号")
            {
                StockID.Text = "";
            }
        }

        private void StockID_LostFocus(object sender, RoutedEventArgs e)
        {
            if (StockID.Text == "")
            {
                StockID.Text = "搜索:请输入股票编号";
            }
        }

        private void OpenExcle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择文件";
            openFileDialog.Filter = "xls,xlsx文件|*.xls;*.xlsx";
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.DefaultExt = "xls";
            if (openFileDialog.ShowDialog() == true)
            {
                MessageBox.Show(openFileDialog.FileName);
            }
            else
            {
                return;
            }
        }

        private void DealList_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DealList dlg = new DealList();
            dlg.Show();
        }

        private void Structure_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Structure dlg = new Structure();
            dlg.Show();
        }

        private void Yield_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Yield dlg = new Yield();
            dlg.Show();
        }
    }
}

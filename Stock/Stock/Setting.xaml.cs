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

using Stock.Controller.NetController;

namespace Stock
{
    /// <summary>
    /// Setting.xaml 的交互逻辑
    /// </summary>
    public partial class Setting : Window
    {
        private static int index = 0;
        public Setting()
        {
            InitializeComponent();
            api.SelectedIndex = index;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (api.Text == "新浪")
                MessageBox.Show("新浪api作为后备源\n部分k线图和历史记录功能无法完全代替\n仅在无法正常使用网易api时备用");
            NetDataController.ChangeApi(api.Text);
            index = api.SelectedIndex;
            this.Close();
        }

        private void Min_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Close_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
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

    }
}

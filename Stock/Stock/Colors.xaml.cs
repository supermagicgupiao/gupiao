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
    /// Color.xaml 的交互逻辑
    /// </summary>
    public partial class Colors : Window
    {
        public Colors()
        {
            InitializeComponent();
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

        private byte r, g, b;
        private void R_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            r = Convert.ToByte(R_Color.Value * 255 / 10);
            ColorChanged();
        }

        private void G_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            g = Convert.ToByte(G_Color.Value * 255 / 10);
            ColorChanged();
        }

        private void B_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            b = Convert.ToByte(B_Color.Value * 255 / 10);
            ColorChanged();
        }
        private void ColorSet(Color color)
        {
            r = color.R;
            g = color.G;
            b = color.B;
            R_Color.Value = Convert.ToDouble(r * 10) / 255;
            G_Color.Value = Convert.ToDouble(g * 10) / 255;
            B_Color.Value = Convert.ToDouble(b * 10) / 255;
        }
        private void ColorChanged()
        {
            MainWindow mw = (MainWindow)this.Owner;
            Brush br = new SolidColorBrush(Color.FromRgb(r, g, b));
            if (Check_UIA.IsChecked == true)
            {
                mw.colora.Color = Color.FromRgb(r, g, b);
                mw.colorc.Color = Color.FromRgb(r, g, b);
                mw.colore.Color = Color.FromRgb(r, g, b);
            }
            else if (Check_UIB.IsChecked == true)
            {
                mw.colorb.Color = Color.FromRgb(r, g, b);
                mw.colord.Color = Color.FromRgb(r, g, b);
            }
            else if (Check_STOCKLIST.IsChecked == true)
                mw.StockCanvas.Background = br;
            else if (Check_STOCKUI.IsChecked == true)
            {
                foreach (StockStateBox ui in mw.StockCanvas.Children)
                    ui.Background = br;
            }
        }

        private void Check_UIA_Checked(object sender, RoutedEventArgs e)
        {
           MainWindow mw = (MainWindow)this.Owner;
           ColorSet(mw.colora.Color);
        }

        private void Check_UIB_Checked(object sender, RoutedEventArgs e)
        {
            MainWindow mw = (MainWindow)this.Owner;
            ColorSet(mw.colorb.Color);
        }

        private void Check_STOCKLIST_Checked(object sender, RoutedEventArgs e)
        {
            MainWindow mw = (MainWindow)this.Owner;
            ColorSet(((SolidColorBrush)mw.StockCanvas.Background).Color);
        }

        private void Check_STOCKUI_Checked(object sender, RoutedEventArgs e)
        {
            MainWindow mw = (MainWindow)this.Owner;
            foreach (StockStateBox ui in mw.StockCanvas.Children)
            {
                ColorSet(((SolidColorBrush)ui.Background).Color);
                break;
            }
        }

    }
}

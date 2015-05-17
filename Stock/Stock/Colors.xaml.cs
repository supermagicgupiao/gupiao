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

        private byte a, r, g, b;
        private void A_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            a = Convert.ToByte(A_Color.Value * 255 / 10);
            ColorChanged();
        }
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
            a = color.A;
            r = color.R;
            g = color.G;
            b = color.B;
            A_Color.Value = Convert.ToDouble(a * 10) / 255;
            R_Color.Value = Convert.ToDouble(r * 10) / 255;
            G_Color.Value = Convert.ToDouble(g * 10) / 255;
            B_Color.Value = Convert.ToDouble(b * 10) / 255;
        }
        private void ColorChanged()
        {
            Color c = Color.FromArgb(a, r, g, b);
            if (Check_UIA.IsChecked == true || Check_UIB.IsChecked == true)
            {
                LinearGradientBrush br = Application.Current.TryFindResource("MainColorBrush") as LinearGradientBrush;
                LinearGradientBrush brush = br.Clone();
                bool i = true;
                foreach (GradientStop gs in brush.GradientStops)
                {
                    if (Check_UIA.IsChecked == true)
                    {
                        if (i)
                            gs.Color = c;
                    }
                    else if (Check_UIB.IsChecked == true)
                    {
                        if (!i)
                            gs.Color = c;
                    }
                    i = !i;
                }
                Application.Current.Resources.Remove("MainColorBrush");
                Application.Current.Resources.Add("MainColorBrush", brush);
            }
            else
            {
                string box;
                if (Check_STOCKLIST.IsChecked == true)
                {
                    box = "CanvasColor";
                }
                else if (Check_STOCKUI.IsChecked == true)
                {
                    box = "BoxColor";
                }
                else
                {
                    return;
                }
                SolidColorBrush br = Application.Current.TryFindResource(box) as SolidColorBrush;
                SolidColorBrush brush = br.Clone();
                brush.Color = c;
                Application.Current.Resources.Remove(box);
                Application.Current.Resources.Add(box, brush);
            }
        }
        private void UIAandUIBSetColor(bool b)
        {
            LinearGradientBrush brush = Application.Current.TryFindResource("MainColorBrush") as LinearGradientBrush;
            bool i = true;
            foreach (GradientStop gs in brush.GradientStops)
            {
                if (b == i)
                {
                    ColorSet(gs.Color);
                }
                i = !i;
            }
        }

        private void Check_UIA_Checked(object sender, RoutedEventArgs e)
        {
            UIAandUIBSetColor(true);
        }

        private void Check_UIB_Checked(object sender, RoutedEventArgs e)
        {
            UIAandUIBSetColor(false);
        }

        private void STOCKLISTandSTOCKUISetColor(bool b)
        {
            string box;
            if (b == true)
            {
                box = "CanvasColor";
            }
            else
            {
                box = "BoxColor";
            }
            SolidColorBrush brush = Application.Current.TryFindResource(box) as SolidColorBrush;
            ColorSet(brush.Color);
        }
        private void Check_STOCKLIST_Checked(object sender, RoutedEventArgs e)
        {
            STOCKLISTandSTOCKUISetColor(true);
        }

        private void Check_STOCKUI_Checked(object sender, RoutedEventArgs e)
        {
            STOCKLISTandSTOCKUISetColor(false);
        }

    }
}

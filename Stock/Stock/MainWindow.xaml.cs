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
using System.Threading;
using System.IO;

using Stock.Controller.NetController;
using Stock.Controller.DrawController;
using Stock.Controller.DBController;
using Stock.Controller.ExcelController;
using Stock.Controller.DBController.DBTable;

using Stock.UIController;

namespace Stock
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
        public static double principal;
        public static double price = 0;
        public static double upwin = 0;
        public static double daywin = 0;
        private void MainGrid_MouseMove(object sender, MouseEventArgs e)
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
        private void Search_Click(object sender, RoutedEventArgs e)
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
                ExcelDataController edc = new ExcelDataController();
                List<DealListEntity> DLEL;
                edc.Open(openFileDialog.FileName, out DLEL);
                DBSyncController.Handler().DealListAdd(DLEL);
                StockStateBoxController.Handler().StockBoxInit();
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InfoShowTimer = new Timer(ShowBoxCheck, null, 0, 2 * 1000);
            InfoShowTimer.Change(-1, 0);
            infoshow = new InfoShow();
            UserCanvas.Visibility = Visibility.Hidden;
            total.IsEnabled = false;
            now.IsEnabled = false;

            //程序开始准备
            this.Hide();
            StockStateBoxController.Create(ref StockCanvas);
            UserBoxController.Create(ref UserCanvas);
            DBDelegateBridge.UIMoney uim = new DBDelegateBridge.UIMoney(GetDelegateValues);
            DBDelegateBridge.UIStockHold uis = new DBDelegateBridge.UIStockHold(StockStateBoxController.Handler().GetDelegateValues);
            InfoDelegate.SetWin setwin = new InfoDelegate.SetWin(setWin);
            UserPanelController UPC = UserPanelController.Create(ref uim, ref uis, ref setwin);
            Adapter.ErrorAdapter.Show(NetState.Check("0000001"));
            this.Show();
            //UserPanelController.Create(ref UserPanel);
            //DBSyncController.Handler().SetMoneyDelegate(new DBDataController.ChangeMoney(setPrincipal), new DBDataController.ChangeMoney(setTotal), new DBDataController.ChangeMoney(setNow));
            //MoneyEntity ME;
            //DBSyncController.Handler().MoneyRead(out ME);
            //total.Text = String.Format("{0:F}", ME.total);
            //now.Text = String.Format("{0:F}", ME.now);    
            //StockBox();
        }
        public void setUser(string name)
        {
            user.Content = "(" + name + ")";
        }

        public void setPrincipal(double money)
        {
            principal = money;
        }
        public void setTotal(double money)
        {
            total.Text = Adapter.DataAdapter.RealTwo(money);
            setstate1();
            setstate2();
        }
        public void setNow(double money)
        {
            now.Text = Adapter.DataAdapter.RealTwo(money);
            setstate1();
            setstate2();
        }
        public void setWin(double money)
        {
            win = money;
            setTotal(Convert.ToDouble(now.Text) + win);
        }
        private void setstate1()
        {
            double c = Convert.ToDouble(total.Text) - principal;
            if (c > 0)
                state1.Foreground = new SolidColorBrush(Color.FromRgb(200, 0, 0));
            else
                state1.Foreground = new SolidColorBrush(Color.FromRgb(0, 100, 0));
            state1.Text = Adapter.DataAdapter.RealTwo(c);
        }
        private void setstate2()
        {
            double c = (Convert.ToDouble(total.Text) - principal) / principal * 100;
            if (c > 0)
                state2.Foreground = new SolidColorBrush(Color.FromRgb(200, 0, 0));
            else
                state2.Foreground = new SolidColorBrush(Color.FromRgb(0, 100, 0));
            state2.Text = Adapter.DataAdapter.RealTwo(c) + "%";
        }
        private double win = 0;

        private void GetDelegateValues(MoneyEntity ME)
        {
            setUser(ME.name);
            setPrincipal(ME.principal);
            setTotal(ME.now + win);
            setNow(ME.now);
        }

        //private void StockBox()
        //{
        //    StockStateBox.pre = null;
        //    StockCanvas.Children.Clear();
        //    List<StockHoldEntity> SHEL;
        //    DBSyncController.Handler().StockHoldReadAll(out SHEL);
        //    foreach (StockHoldEntity SHE in SHEL)
        //    {
        //        StockStateBoxController.Handler().Add(SHE.id, SHE.name, SHE.hold, SHE.money);
        //    }
        //    NetSyncController.Handler().StartRefresh();
        //}

        private Timer InfoShowTimer;
        private bool flag;
        private InfoShow infoshow;
        private delegate void DelegateShowInfoBox();
        private void Rectangle_MouseEnter(object sender, MouseEventArgs e)
        {
            InfoShowTimer.Change(-1, 0);
            InfoShowTimer = new Timer(ShowBoxCheck, null, 1000, 1000);
            flag = true;
        }

        private void Rectangle_MouseLeave(object sender, MouseEventArgs e)
        {
            infoshow.Hide();
            flag = false;
        }
        private void ShowBoxCheck(object obj)
        {
            InfoShowTimer.Change(-1, 0);
            if (flag == true)
            {
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.SystemIdle, new DelegateShowInfoBox(ShowInfoBox));
            }
        }
        private void ShowInfoBox()
        {
            infoshow.WindowStartupLocation = WindowStartupLocation.Manual;
            infoshow.Left = this.Left + this.ActualWidth + 5;
            infoshow.Top = this.Top;
            infoshow.principal.Text = Adapter.DataAdapter.RealTwo(principal);
            infoshow.total.Text = total.Text;
            infoshow.now.Text = now.Text;
            infoshow.win.Text = state1.Text;
            infoshow.upwin.Text = Adapter.DataAdapter.RealTwo(upwin);
            infoshow.daywin.Text = Adapter.DataAdapter.RealTwo(daywin);
            infoshow.price.Text = Adapter.DataAdapter.RealTwo(price);
            infoshow.Show();
        }

        //private double mark;
        private void now_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //if (now.IsReadOnly == true)
            //{
            //    mark = Convert.ToDouble(now.Text.ToString());
            //    now.IsReadOnly = false;
            //}
        }

        private void now_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            //if (now.IsReadOnly == false)
            //{
            //    now.IsReadOnly = true;
            //    double o;
            //    if (Double.TryParse(now.Text.ToString(), out o))
            //    {
            //        double t = Convert.ToDouble(total.Text) + o - mark;
            //        if (t >= 1000000000)
            //        {
            //            now.Text = Adapter.DataAdapter.RealTwo(mark);
            //        }
            //        DBSyncController.Handler().PrincipalChange(o - mark);
            //    }
            //    else
            //    {
            //        now.Text = Adapter.DataAdapter.RealTwo(mark);
            //    }
            //}
        }
        private void MainGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            state.Focus();
        }

        private void Color_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Colors dlg = new Colors();
            dlg.Show();
        }

        private void StockCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            int d = e.Delta;
            StockStateBox ssb;
            if (StockCanvas.Children.Count == 0)
                return;
            else
            {
                ssb = (StockStateBox)StockCanvas.Children[0];
                if ((StockCanvas.ActualHeight - 10) / (ssb.ActualHeight + 10) > StockCanvas.Children.Count) 
                    return;
            }
            if (d > 0)
            {
                if (StockCanvas.Children.Count > 0)
                {
                    if (ssb.Margin.Top == 5)
                    {
                        return;
                    }
                    else if (ssb.Margin.Top + 20 * d / 100 > 5)
                    {
                        double c = 5 - ssb.Margin.Top;
                        foreach (StockStateBox ui in StockCanvas.Children)
                        {
                            ui.Margin = new Thickness(5, ui.Margin.Top + c, 0, 0);
                        }
                        return;
                    }
                    else
                    {
                        foreach (StockStateBox ui in StockCanvas.Children)
                        {
                            ui.Margin = new Thickness(5, ui.Margin.Top + 20 * d / 100, 0, 0);
                        }
                    }
                }
            }
            if (d < 0)
            {
                if (StockCanvas.Children.Count > 0)
                {
                    ssb = (StockStateBox)StockCanvas.Children[StockCanvas.Children.Count - 1];
                    double h = ssb.Margin.Top + ssb.Height;
                    if (h == StockCanvas.Height - 5)
                        return;
                    else if (h + 20 * d / 100 < StockCanvas.Height - 5)
                    {
                        double c = StockCanvas.Height - 5 - h;
                        foreach (StockStateBox ui in StockCanvas.Children)
                        {
                            ui.Margin = new Thickness(5, ui.Margin.Top + c, 0, 0);
                        }
                        return;
                    }
                    else
                    {
                        foreach (StockStateBox ui in StockCanvas.Children)
                        {
                            ui.Margin = new Thickness(5, ui.Margin.Top + 20 * d / 100, 0, 0);
                        }
                    }
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                MessageBox.Show(NetDataController.GetLog());
            }
            if (e.Key == Key.F1)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "选择文件";
                openFileDialog.Filter = "png,jpg文件|*.png;*.jpg";
                openFileDialog.FileName = string.Empty;
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == true)
                {
                    CroppedBitmap cb = new CroppedBitmap(new BitmapImage(new Uri(openFileDialog.FileName, UriKind.Absolute)),new Int32Rect(0,0,(int)MainGrid.ActualWidth,(int)MainGrid.ActualHeight));
                    ImageBrush brush = new ImageBrush();
                    brush.ImageSource = cb;
                    //brush.Opacity = 0.95;
                    MainGrid.Background = brush;
                }
                else
                {
                    return;
                }
            }
            //if (e.Key == Key.F2) 
            //{
            //    BitmapSource bsrc = (BitmapSource)((ImageBrush)MainGrid.Background).ImageSource;
            //    PngBitmapEncoder pngE = new PngBitmapEncoder();

            //    pngE.Frames.Add(BitmapFrame.Create(bsrc));
            //    using (Stream stream = File.Create("background.png"))
            //    {
            //        pngE.Save(stream);
            //    }
            //}
        }


        private bool UserSwitchFlag = true;
        private void Switch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UserSwitchFlag = !UserSwitchFlag;
            if (UserSwitchFlag) UserCanvas.Visibility = Visibility.Hidden;
            else UserCanvas.Visibility = Visibility.Visible;
        }

        private void UserCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            int d = e.Delta;
            UserBox ub;
            if (UserCanvas.Children.Count <= 1)
                return;
            else
            {
                ub = (UserBox)UserCanvas.Children[0];
                if ((UserCanvas.ActualHeight - 10) / (ub.ActualHeight + 5) > UserCanvas.Children.Count) 
                    return;
            }
            if (d > 0)
            {
                if (UserCanvas.Children.Count > 1)
                {
                    ub = (UserBox)UserCanvas.Children[1];
                    if (ub.Margin.Top == 5)
                    {
                        return;
                    }
                    else if (ub.Margin.Top + 20 * d / 100 > 5)
                    {
                        double c = 5 - ub.Margin.Top;
                        foreach (UserBox ui in UserCanvas.Children)
                        {
                            ui.Margin = new Thickness(5, ui.Margin.Top + c, 0, 0);
                        }
                        return;
                    }
                    else
                    {
                        foreach (UserBox ui in UserCanvas.Children)
                        {
                            ui.Margin = new Thickness(5, ui.Margin.Top + 20 * d / 100, 0, 0);
                        }
                    }
                }
            }
            if (d < 0)
            {
                if (UserCanvas.Children.Count > 0)
                {
                    ub = (UserBox)UserCanvas.Children[0];
                    double h = ub.Margin.Top + ub.Height;
                    if (h == UserCanvas.Height - 5)
                        return;
                    else if (h + 20 * d / 100 < UserCanvas.Height - 5)
                    {
                        double c = UserCanvas.Height - 5 - h;
                        foreach (UserBox ui in UserCanvas.Children)
                        {
                            ui.Margin = new Thickness(5, ui.Margin.Top + c, 0, 0);
                        }
                        return;
                    }
                    else
                    {
                        foreach (UserBox ui in UserCanvas.Children)
                        {
                            ui.Margin = new Thickness(5, ui.Margin.Top + 20 * d / 100, 0, 0);
                        }
                    }
                }
            }
        }

        private void Setting_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Setting dlg = new Setting();
            dlg.ShowDialog();
        }

        private void MainGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (StockID.Text == "")
            {
                Search.Focus();
            }
        }

    }
}

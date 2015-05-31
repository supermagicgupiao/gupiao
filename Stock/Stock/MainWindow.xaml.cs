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
            Test();
            InitializeComponent();
            if (principal == 0)
                this.Close();
        }
        public static double principal;
        public static double price = 0;
        public static double upwin = 0;
        public static double daywin = 0;
        private void Test()
        {
            UserPanelController UPC = UserPanelController.Create();
            Adapter.ErrorAdapter.Show(NetState.Check("0000001"));
        }
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
                StockBox();
            }
            else
            {
                return;
            }
        }

        private void DealList_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DealList dlg = new DealList();
            DBSyncController.Handler().DealListReadAll(out dlg.DLEL);
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
            dlg = new InfoShow();
            total.IsEnabled = false;
            //now.IsEnabled = false;

            //程序开始准备

            StockStateBoxController.Create(ref StockCanvas);
            //UserPanelController.Create(ref UserPanel);
            //DBSyncController.Handler().SetMoneyDelegate(new DBDataController.ChangeMoney(setPrincipal), new DBDataController.ChangeMoney(setTotal), new DBDataController.ChangeMoney(setNow));
            //MoneyEntity ME;
            //DBSyncController.Handler().MoneyRead(out ME);
            //total.Text = String.Format("{0:F}", ME.total);
            //now.Text = String.Format("{0:F}", ME.now);
                     
            StockBox();
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
        private void setstate1()
        {
            state1.Text = Adapter.DataAdapter.RealTwo(Convert.ToDouble(total.Text) - principal);
        }
        private void setstate2()
        {
            state2.Text = Adapter.DataAdapter.RealTwo((Convert.ToDouble(total.Text) - principal) / principal * 100) + "%";
        }


        private void StockBox()
        {
            StockStateBox.pre = null;
            StockCanvas.Children.Clear();
            List<StockHoldEntity> SHEL;
            DBSyncController.Handler().StockHoldReadAll(out SHEL);
            foreach (StockHoldEntity SHE in SHEL)
            {
                StockStateBoxController.Handler().Add(SHE.id, SHE.name, SHE.hold, SHE.money);
            }
            NetSyncController.Handler().StartRefresh();
        }

        private Timer InfoShowTimer;
        private bool flag;
        private InfoShow dlg;
        private delegate void DelegateShowInfoBox();
        private void Rectangle_MouseEnter(object sender, MouseEventArgs e)
        {
            InfoShowTimer.Change(-1, 0);
            InfoShowTimer = new Timer(ShowBoxCheck, null, 1000, 1000);
            flag = true;
        }

        private void Rectangle_MouseLeave(object sender, MouseEventArgs e)
        {
            dlg.Hide();
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
            dlg.WindowStartupLocation = WindowStartupLocation.Manual;
            dlg.Left = this.Left + this.ActualWidth + 5;
            dlg.Top = this.Top;
            dlg.principal.Text = Adapter.DataAdapter.RealTwo(principal);
            dlg.total.Text = total.Text;
            dlg.now.Text = now.Text;
            dlg.win.Text = state1.Text;
            dlg.upwin.Text = Adapter.DataAdapter.RealTwo(upwin);
            dlg.daywin.Text = Adapter.DataAdapter.RealTwo(daywin);
            dlg.price.Text = Adapter.DataAdapter.RealTwo(price);
            dlg.Show();
        }

        private double mark;
        private void now_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (now.IsReadOnly == true)
            {
                mark = Convert.ToDouble(now.Text.ToString());
                now.IsReadOnly = false;
            }
        }

        private void now_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (now.IsReadOnly == false)
            {
                now.IsReadOnly = true;
                double o;
                if (Double.TryParse(now.Text.ToString(), out o))
                {
                    double t = Convert.ToDouble(total.Text) + o - mark;
                    if (t >= 1000000000)
                    {
                        now.Text = Adapter.DataAdapter.RealTwo(mark);
                    }
                    DBSyncController.Handler().PrincipalChange(o - mark);
                }
                else
                {
                    now.Text = Adapter.DataAdapter.RealTwo(mark);
                }
            }
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
            if (d > 0)
            {
                if (((StockStateBox)StockCanvas.Children[0]).Margin.Top >= 5)
                    return;
                foreach (StockStateBox ui in StockCanvas.Children)
                {
                    ui.Margin = new Thickness(5, ui.Margin.Top + 20 * d / 120, 0, 0);
                }
            }
            if (d < 0)
            {
                StockStateBox ssb = (StockStateBox)StockCanvas.Children[StockCanvas.Children.Count - 1];
                if (ssb.Margin.Top + ssb.Height <= StockCanvas.Height) 
                    return;
                foreach (StockStateBox ui in StockCanvas.Children)
                {
                    ui.Margin = new Thickness(5, ui.Margin.Top + 20 * d / 120, 0, 0);
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                MessageBox.Show(NetDataController.log);
            }
        }
    }
}

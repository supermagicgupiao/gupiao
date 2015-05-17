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

using Microsoft.Win32;
using System.IO;

using Stock.Controller.DrawController;
using Stock.Controller.NetController;
using Stock.Controller.DBController.DBTable;

namespace Stock
{
    /// <summary>
    /// Structure.xaml 的交互逻辑
    /// </summary>
    public partial class Structure : Window
    {
        private List<StockView> list = new List<StockView>();
        private List<string> select = new List<string>();
        private Dictionary<string, Color> stockcolor = new Dictionary<string, Color>();

        public Structure()
        {
            InitializeComponent();
            this.StockList.ItemsSource = list;
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DrawDataController DDC = new DrawDataController((int)(hold.Width), (int)(hold.Height));
            hold.Source = Adapter.ImageAdapter.ImageConvert(DDC.GetImage());
            List<StockHoldEntity> SHEL;
            Stock.MainWindow.dbc.StockHoldReadAll(out SHEL);
            List<string> idl = SHEL.Select(s => s.id).ToList();
            NetState.IdConvert(ref idl);
            List<DrawPieEntity> DPEL = new List<DrawPieEntity>();
            DrawPieEntity DPE = new DrawPieEntity();
            foreach(StockHoldEntity SHE in SHEL)
            {
                string id = idl.Where(s => s.Substring(1) == SHE.id).First();
                if (id == null)
                    continue;
                StockView sv = new StockView(SHE.name, id);
                list.Add(sv);
                stockcolor.Add(sv.StockID, sv.brush.Color);
                DPE.name = SHE.name;
                DPE.money = SHE.hold * NetState.PriceGet(sv.StockID);
                DPE.color = Adapter.ImageAdapter.ColorConvert(sv.brush.Color);
                DPEL.Add(DPE);
            }
            DDC = new DrawDataController((int)(pie.Width), (int)(pie.Height));
            DDC.DrawPieChart(DPEL);
            pie.Source = Adapter.ImageAdapter.ImageConvert(DDC.GetImage());
            this.StockList.Items.Refresh();
        }

        private void ALL_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.IsChecked == true)
            {
                select = list.Select(l => l.StockID).ToList();
                foreach(StockView sv in list)
                {
                    sv.Checked = true;
                }
                this.StockList.Items.Refresh();
            }
            else
            {
                select.Clear();
                foreach (StockView sv in list)
                {
                    sv.Checked = false;
                }
                this.StockList.Items.Refresh();
            }
        }

        private void STOCK_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            string id = cb.Tag.ToString();
            if (cb.IsChecked == true)
            {
                select.Add(id);
            } 
            else
            {
                select.Remove(id);
            }
        }

        private void ShowImage(object sender, RoutedEventArgs e)
        {
            if(select.Count == 0)
            {
                MessageBox.Show("请选择股票");
                return;
            }
            DateTime date;
            int days;
            try
            {
                date = Convert.ToDateTime(StartDate.Text);
                days = Convert.ToInt32(DateLong.Text);
            }
            catch(Exception)
            {
                MessageBox.Show("请输入正确的日期");
                return;
            }
            Dictionary<DateTime, double> money;
            List<HistoryStockHoldEntity> HSHEL;
            DrawDataController DDC = new DrawDataController((int)(hold.Width), (int)(hold.Height));
            Dictionary<System.Drawing.Color, List<DrawDataEntity>> dict = new Dictionary<System.Drawing.Color, List<DrawDataEntity>>();
            double max = 0;
            foreach(string id in select)
            {
                DrawDataEntity DDE = new DrawDataEntity();
                List<DrawDataEntity> DDEL = new List<DrawDataEntity>();
                Stock.MainWindow.dbc.HistoryStockHoldReadByRange(id.Substring(1), date, days, out HSHEL);
                if(HSHEL.Count == 0)
                    continue;
                NetDataController.HistoryMoney(id, date, days, out money);
                int index = 0;
                DateTime usable = DateTime.Parse("1970-01-01");
                for (int i = 0; i < days; i++) 
                {
                    DateTime dt = date.AddDays(i);
                    DDE.date = dt;
                    if (dt > DateTime.Now)
                    {
                        break;
                    }
                    if (money.ContainsKey(dt))
                    {
                        usable = dt;
                    }
                    else
                    {
                        if (usable == DateTime.Parse("1970-01-01"))
                            continue;
                    }
                    if (dt < HSHEL[index].date)
                    {
                        DDE.money = HSHEL[index].number * money[usable];
                    }
                    else
                    {
                        if (index >= HSHEL.Count - 1)
                        {
                            DDE.money = (HSHEL[index].number + HSHEL[index].change) * money[usable];
                        }
                        else
                        {
                            index += 1;
                            DDE.money = HSHEL[index].number * money[usable];
                        }
                    }
                    if (max < DDE.money)
                        max = DDE.money;
                    DDEL.Add(DDE);
                }
                dict.Add(Adapter.ImageAdapter.ColorConvert(stockcolor[id]), DDEL);
            }
            DDC.DrawData(dict, max);
            hold.Source = Adapter.ImageAdapter.ImageConvert(DDC.GetImage());
        }
        private void SaveImage(object sender, RoutedEventArgs e)
        {
            BitmapImage image = (BitmapImage)hold.Source;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            SaveFileDialog sfd = new SaveFileDialog();
            string imageFileName = "";
            sfd.Filter = "jpg文件(*.jpg)|*.jpg|png文件(*.png)|*.png";
            if (sfd.ShowDialog() == true)
            {
                imageFileName = sfd.FileName;
            }
            else
            {
                return;
            }
            FileStream fileStream = new FileStream(imageFileName, FileMode.Create, FileAccess.ReadWrite);
            encoder.Save(fileStream);
            fileStream.Close();
        }
    }
    public class StockView
    {
        public static Color c = Color.FromRgb(0, 0, 0);
        public StockView(string StockName, string StockID)
        {
            this.StockName = StockName;
            this.StockID = StockID;
            this.brush = new SolidColorBrush(Color.FromRgb((byte)(c.R + 30), (byte)(c.G + 60), (byte)(c.B + 90)));
            c = this.brush.Color;
            this.Checked = false;
        }
        public void IsChecked()
        {
            Checked = !Checked;
        }
        public bool Checked { get; set; }
        public string StockName { get; set; }
        public string StockID { get; set; }
        public SolidColorBrush brush { get; set; }
    }
}

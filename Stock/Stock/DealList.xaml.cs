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

using Stock.Controller.DBController;
using Stock.Controller.DBController.DBTable;
using Stock.UIController;

namespace Stock
{
    /// <summary>
    /// DealList.xaml 的交互逻辑
    /// </summary>
    public partial class DealList : Window
    {
        public DealList()
        {
            InitializeComponent();
            user.Content = "(" + UserPanelController.Handler().name + ")";
        }
        public List<DealListEntity> DLEL;
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
            string[] listheader = new string[] { "股票名称", "股票编号", "类型", "价格", "数量", "税率", "佣金", "日期", "说明", "备注" };
            string[] binding = new string[] { "name", "id", "type", "money", "number", "taxrate", "commission", "date", "explain", "remark" };
            int[] width = new int[] { 70, 70, 50, 80, 80, 60, 60, 80, 50, 50 };
            GridViewColumn[] h = new GridViewColumn[listheader.Length];
            for (int i = 0; i < listheader.Length; i++) 
            {
                h[i] = new GridViewColumn();
                h[i].Header = listheader[i];
                BindingBase b = new Binding(binding[i]);
                h[i].DisplayMemberBinding = b;
                h[i].Width = width[i];
                ListView.Columns.Add(h[i]);
            }
            foreach (DealListEntity DLE in DLEL)
            {
                ItemData data = new ItemData(DLE.name, DLE.id, DLE.date.ToString("yyyy/MM/dd"), DLE.type, DLE.money.ToString(), DLE.number.ToString(), DLE.taxrate.ToString(), DLE.commission.ToString(), DLE.explain, DLE.remark);
                DList.Items.Add(data);
            }
            DList.UpdateLayout();
        }

        private void DList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            object o = DList.SelectedItem;
            if (o == null)
                return;
            ItemData item = o as ItemData;
            MessageBox.Show("暂停未提供修改功能!");
        }
    }
    public class ItemData
    {
        public ItemData(string name, string id ,string date ,string type ,string money ,string number, string taxrate ,string commission ,string explain ,string remark)
        {
            this.name = name;
            this.id = id;
            this.date = date;
            this.type = type;
            this.money = money;
            this.number = number;
            this.taxrate = taxrate + "‰";
            this.commission = commission + "‰";
            this.explain = explain;
            this.remark = remark;
        }
        public string name { get; set; }
        public string id { get; set; }
        public string date { get; set; }
        public string type { get; set; }
        public string money { get; set; }
        public string number { get; set; }
        public string taxrate { get; set; }
        public string commission { get; set; }
        public string explain { get; set; }
        public string remark { get; set; }
    }
}

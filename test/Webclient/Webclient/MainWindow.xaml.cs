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

using System.Net;
using System.IO;
using System.Threading;

namespace Webclient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            flag = false;
        }
        private Timer t;
        private bool flag;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (flag == true)
            {
                flag = false;
                t.Change(-1, 0);
                test.Content = "开始";
                return;
            }
            else
            {
                if (StockNum.Text == "")
                {
                    MessageBox.Show("请输入股票编号");
                    return;
                }
                flag = true;
                NetText.Text = "";
                test.Content = "停止";
                t = new Timer(web_client, StockApi.Text + StockNum.Text, 0, int.Parse(Refresh.Text.Substring(0, Refresh.Text.Length - 1)) * 1000);
            }
        }
        private void UpdateText(TextBox NetText, string text)
        {
            NetText.Text += DateTime.Now.ToString() + "\n";
            NetText.Text += text + "\n";
        }
        private void web_client(object _url)
        {
            string url = (string)_url;
            WebClient client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/4.0");
            Stream data = client.OpenRead(url);
            StreamReader reader = new StreamReader(data);
            string s = reader.ReadToEnd();
            Action<TextBox, String> updateAction = new Action<TextBox, string>(UpdateText);
            NetText.Dispatcher.BeginInvoke(updateAction, NetText, s);
            data.Close();
            reader.Close();
        }
    }
}

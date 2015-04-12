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

using System.Data;
using System.Data.OleDb;
using Microsoft.Win32;

namespace Excel
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
        private void Button_Click(object sender, RoutedEventArgs e)
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
                InFileName.Text = openFileDialog.FileName;
            }
            else
            {
                return;
            }
            String sConn = "Provider=Microsoft.Ace.OleDb.12.0;"
                + "Data Source=" + openFileDialog.FileName + ";"
                + "Extended Properties='Excel 12.0;HDR=Yes;IMEX=0'";
            ListBox.Items.Clear();
            using (OleDbConnection ole_conn = new OleDbConnection(sConn))
            {
                ole_conn.Open();
                using (OleDbCommand ole_cmd = ole_conn.CreateCommand())
                {
                    System.Data.DataTable tableColumns = ole_conn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Columns, new object[] { null, null, "Sheet1$", null });
                    string all_column = "";
                    int column_count = tableColumns.Rows.Count;
                    string[] column = new string[column_count];
                    for (int i = 0; i < column_count; i++)
                    {
                        column[i] = tableColumns.Rows[i]["Column_Name"].ToString();
                        all_column += string.Format("{0,-10}|", column[i]);
                    }
                    ListBox.Items.Add(all_column);
                    ole_cmd.CommandText = "select * from [Sheet1$]";
                    OleDbDataAdapter adapter = new OleDbDataAdapter(ole_cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds, "Sheet1");
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        string row = "";
                        for (int j = 0; j < column_count; j++)
                            row += string.Format("{0,-10}|", ds.Tables[0].Rows[i][column[j]].ToString());
                        ListBox.Items.Add(row);
                    }
                }
            }
        }
    }
}

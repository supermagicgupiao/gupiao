using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.OleDb;
using System.Data;

using Stock.Controller.DBController.DBTable;
using Stock.Controller.DBController;

namespace Stock.Controller.ExcelController
{
    class ExcelOpen
    {
        public void open(string FileName,ref DBDataController DBC)
        {
            String sConn = "Provider=Microsoft.Ace.OleDb.12.0;"
                + "Data Source=" + FileName + ";"
                + "Extended Properties='Excel 12.0;HDR=Yes;IMEX=0'";
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
                    ole_cmd.CommandText = "select * from [Sheet1$]";
                    OleDbDataAdapter adapter = new OleDbDataAdapter(ole_cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds, "Sheet1");
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DealListEntity DLE = new DealListEntity();
                        for (int j = 0; j < column_count; j++)
                        {
                            string s;
                            int k;
                            switch(column[j])
                            {
                                case "价格":
                                    DLE.money = ds.Tables[0].Rows[i][column[j]].ToString();
                                    break;
                                case "佣金":
                                    s = ds.Tables[0].Rows[i][column[j]].ToString();
                                    k = s.IndexOf('‰');
                                    if (k > 0)
                                        DLE.commission = s.Substring(0, k);
                                    else
                                        DLE.commission = s;
                                    break;
                                case "备注":
                                    DLE.remark = ds.Tables[0].Rows[i][column[j]].ToString();
                                    break;
                                case "数量":
                                    DLE.number = ds.Tables[0].Rows[i][column[j]].ToString();
                                    break;
                                case "日期":
                                    s = ds.Tables[0].Rows[i][column[j]].ToString();
                                    k = s.IndexOf(' ');
                                    if (k > 0)
                                        DLE.date = s.Substring(0, k);
                                    else
                                        DLE.date = s;
                                    break;
                                case "税率":
                                    s = ds.Tables[0].Rows[i][column[j]].ToString();
                                    k = s.IndexOf('‰');
                                    if (k > 0)
                                        DLE.taxrate = s.Substring(0, k);
                                    else
                                        DLE.taxrate = s;
                                    break;
                                case "类型":
                                    DLE.type = ds.Tables[0].Rows[i][column[j]].ToString();
                                    break;
                                case "股票名称":
                                    DLE.name = ds.Tables[0].Rows[i][column[j]].ToString();
                                    break;
                                case "股票编号":
                                    DLE.id = ds.Tables[0].Rows[i][column[j]].ToString().PadLeft(6,'0');
                                    break;
                                case "说明":
                                    DLE.explain = ds.Tables[0].Rows[i][column[j]].ToString();
                                    break;
                            }
                        }
                        if (DLE.name != "" && DLE.id.Length == 6)
                            DBC.DealList_Insert(DLE);
                    }
                }
            }
        }
    }
}

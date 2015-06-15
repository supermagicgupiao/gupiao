using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.OleDb;
using System.Data;

//using System.Threading;

using Stock.Controller.DBController.DBTable;

namespace Stock.Controller.ExcelController
{
    public class ExcelDataController
    {
        public OPENEXCEL_ERROR Open(string FilePath, out List<DealListEntity> DLEL)
        {
            //Thread.Sleep(10000);
            DLEL = new List<DealListEntity>();
            String sConn;
            if (FilePath.Last() == 's')
            {
                sConn = OleStrCon(".xls", FilePath, true);
            }
            else
            {
                sConn = OleStrCon(".xlsx", FilePath, true);
            }
            DataSet ds = new DataSet();
            string[] column;
            //OLE读取excel
            try
            {
                using (OleDbConnection ole_conn = new OleDbConnection(sConn))
                {
                    try { ole_conn.Open();}
                    catch (Exception)
                    {
                        return OPENEXCEL_ERROR.OLE_ERROR;//XLSX没安装Microsoft.Ace.OleDb.12.0则会报错,XLS没安装Microsoft.Jet.OLEDB.4.0会报错
                    }
                    using (OleDbCommand ole_cmd = ole_conn.CreateCommand())
                    {
                        System.Data.DataTable tableColumns = ole_conn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Columns, new object[] { null, null, "Sheet1$", null });
                        string all_column = "";
                        int column_count = tableColumns.Rows.Count;
                        column = new string[column_count];
                        for (int i = 0; i < column_count; i++)//读出所有的列名
                        {
                            column[i] = tableColumns.Rows[i]["Column_Name"].ToString();
                            all_column += string.Format("{0,-10}|", column[i]);
                        }
                        ole_cmd.CommandText = "select * from [Sheet1$]";
                        OleDbDataAdapter adapter = new OleDbDataAdapter(ole_cmd);
                        adapter.Fill(ds, "Sheet1");
                    }
                }
            }
            catch(OleDbException)
            {
                return OPENEXCEL_ERROR.OLE_ERROR;
            }
            List<DealListEntity> DLELNoSort = new List<DealListEntity>();
            bool FORMAT = true;
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                DealListEntity DLE = new DealListEntity();
                int count = 0;
                for (int j = 0; j < column.Length; j++)
                {
                    string s;
                    int k;
                    count++;//数据必须存在以下需要的十列，其他多余的列不作处理
                    Type t = ds.Tables[0].Rows[i][column[j]].GetType();
                    switch(column[j])
                    {
                        case "价格":
                            if (t == DLE.money.GetType())
                                DLE.money = Convert.ToDouble(ds.Tables[0].Rows[i][column[j]]);
                            else
                                count--;
                            break;
                        case "佣金":
                            s = ds.Tables[0].Rows[i][column[j]].ToString();
                            k = s.IndexOf('‰');
                            if (k > 0)
                                DLE.commission = Convert.ToDouble(s.Substring(0, k));
                            else
                                count--;
                            break;
                        case "备注":
                            DLE.remark = ds.Tables[0].Rows[i][column[j]].ToString();
                            break;
                        case "数量":
                            int num;
                            if (Int32.TryParse(ds.Tables[0].Rows[i][column[j]].ToString(), out num) == true)
                                DLE.number = num;
                            else
                                count--;
                            break;
                        case "日期":
                            if (t == DLE.date.GetType())
                                DLE.date = Convert.ToDateTime(ds.Tables[0].Rows[i][column[j]]);
                            else
                                count--;
                            break;
                        case "税率":
                            s = ds.Tables[0].Rows[i][column[j]].ToString();
                            k = s.IndexOf('‰');
                            if (k > 0)
                                DLE.taxrate = Convert.ToDouble(s.Substring(0, k));
                            else
                                count--;
                            break;
                        case "类型":
                            DLE.type = ds.Tables[0].Rows[i][column[j]].ToString();
                            break;
                        case "股票名称":
                            DLE.name = ds.Tables[0].Rows[i][column[j]].ToString();
                            break;
                        case "股票编号":
                            int stock;
                            if (Int32.TryParse(ds.Tables[0].Rows[i][column[j]].ToString(), out stock) == true)
                                DLE.id = stock.ToString().PadLeft(6, '0');
                            else
                                count--;
                            break;
                        case "说明":
                            DLE.explain = ds.Tables[0].Rows[i][column[j]].ToString();
                            break;
                        default:
                            count--;
                            break;
                    }
                }
                if (count == 10)
                {
                    if (DLE.name != "" && DLE.id != "" && DLE.id.Length == 6)//股票名称和长度不对的行不保存
                        DLELNoSort.Add(DLE);
                }
                else
                    FORMAT = false;
            }
            List<DealListEntity> DLELNoSortByDate = new List<DealListEntity>();
            DLELNoSortByDate.AddRange(DLELNoSort.Where(s => s.type == "买入").ToList());
            DLELNoSortByDate.AddRange(DLELNoSort.Where(s => s.type == "卖空").ToList());
            DLELNoSortByDate.AddRange(DLELNoSort.Where(s => s.type == "卖出").ToList());
            DLELNoSortByDate.AddRange(DLELNoSort.Where(s => s.type == "补仓").ToList());
            DLEL = DLELNoSortByDate.OrderBy(s => s.date).ToList();//排序
            if (FORMAT == false)
                return OPENEXCEL_ERROR.FORMAT_ERROR;//缺少相应的列
            return OPENEXCEL_ERROR.OPEN_OK;
        }
        private string OleStrCon(string FileType,string FilePath, bool HasTitle)//ole连接构造
        {
            return string.Format("Provider={0};" +
                        "Extended Properties=\"Excel {1}.0;HDR={2};IMEX=1;\";" +
                        "data source={3};",
                        (FileType == ".xls" ? "Microsoft.Jet.OLEDB.4.0" : "Microsoft.ACE.OLEDB.12.0"), (FileType == ".xls" ? 8 : 12), (HasTitle ? "Yes" : "NO"), FilePath);
        }
    }
    public enum OPENEXCEL_ERROR : byte
    {
        OLE_ERROR,
        FORMAT_ERROR,
        OPEN_OK,
    }
}

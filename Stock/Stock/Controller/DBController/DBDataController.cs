using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;
using System.IO;
using Stock.Controller.DBController.DBTable;

namespace Stock.Controller.DBController
{
    class DBDataController
    {
        private string dbPath;
        private SQLiteConnection conn;
        public DBDataController()
        {
            dbPath = Environment.CurrentDirectory + "\\Stock.db";
        }
        public DBDataController(string s)
        {
            dbPath = s;
        }
        public DB_ERROR Check()
        {
            if (DBConnection())
            {
                DB_ERROR dbe = DBCheckTable();
                if (dbe == DB_ERROR.DB_TABLE_NOT_EXISTS)
                {
                    principal.Create();
                    return dbe;
                }
                else if (dbe == DB_ERROR.DB_TABLE_CRACK)
                {
                    conn.Close();
                    try { File.Delete(dbPath); }
                    catch (Exception)
                    {
                        return dbe;
                    }
                    conn.Open();
                    principal.Create();
                    return DB_ERROR.DB_TABLE_CRACK_FIX;
                }
                else
                {
                    return dbe;
                }
            }
            else
            {
                return DB_ERROR.DB_CANT_CONNECT;
            }
        }
        private bool DBConnection()
        {
            try
            {
                conn = new SQLiteConnection();
                SQLiteConnectionStringBuilder connstr = new SQLiteConnectionStringBuilder();
                connstr.DataSource = dbPath;
                //connstr.Password = "admin";
                conn.ConnectionString = connstr.ToString();
                conn.Open();
                deallist = new DealLists(conn);
                stockhold = new StockHold(conn);
                principal = new Principal(conn);
                StockHold_Init();
            }
            catch (SQLiteException)
            {
                return false;
            }
            return true;
        }
        private DB_ERROR DBCheckTable()
        {
            try
            {
                if (!principal.Exists())
                    return DB_ERROR.DB_TABLE_NOT_EXISTS;
                else if (principal.Select() == 0)
                    return DB_ERROR.DB_DATA_NOT_EXISTS;
            }
            catch (SQLiteException)
            {
                return DB_ERROR.DB_TABLE_CRACK;
            }
            return DB_ERROR.DB_OK;
        }
        private void StockHold_Init()
        {
            List<DealListEntity> DLEL;
            DealList_Selects(out DLEL);
            foreach(DealListEntity DLE in DLEL)
            {
                DealList_Insert_StockHold(DLE);
            }
        }
        public void DealList_Insert(DealListEntity DLE)
        {
            deallist.Insert(DLE);
            DealList_Insert_StockHold(DLE);
        }
        private void DealList_Insert_StockHold(DealListEntity DLE)
        {
            StockHoldEntity SHE;
            SHE.id = DLE.id;
            if (DLE.type == "买入")
                SHE.hold = DLE.number;
            else if (DLE.type == "卖出")
                SHE.hold = "-" + DLE.number;
            else if (DLE.type == "补仓")
                SHE.hold = DLE.number;
            else if (DLE.type == "卖空")
                SHE.hold = "-" + DLE.number;
            else
                SHE.hold = DLE.number;
            SHE.money = (Convert.ToDouble(DLE.money) * Convert.ToDouble(DLE.number)).ToString();
            if (stockhold.StockExists(DLE.id))
            {
                stockhold.Change(SHE);
            }
            else
            {
                stockhold.Insert(SHE);
            }
        }
        public void DealList_Selects(out List<DealListEntity> DLEL)
        {
            deallist.Selects(out DLEL);
        }
        public void StockHold_Selects(out List<StockHoldEntity> SHEL)
        {
            stockhold.Selects(out SHEL);
        }
        public string StockName(string id)
        {
            DealListEntity DLE = new DealListEntity();
            DLE.id = id;
            deallist.Select(ref DLE);
            return DLE.name;
        }
        public void Principal_Insert(double m)
        {
            principal.Insert(m);
        }
        public double Principal_Select()
        {
            return principal.Select();
        }
        private DealLists deallist;
        private StockHold stockhold;
        private Principal principal;
    }
    enum DB_ERROR : byte
    {
        DB_OK,
        DB_CANT_CONNECT,
        DB_TABLE_NOT_EXISTS,
        DB_TABLE_CRACK,
        DB_TABLE_CRACK_FIX,
        DB_DATA_NOT_EXISTS,
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Stock.Controller.DBController.DBTable;

namespace Stock.Controller.DBController
{
    public class DBDelegateBridge
    {
        public DBDelegateBridge(DBDataController DBC)
        {
            DBDataController.MoneyDelegate money = new DBDataController.MoneyDelegate(MoneyDelegate);
            DBC.SetMoneyDelegate(money);
            DBDataController.StockHoldDelegate stockhold = new DBDataController.StockHoldDelegate(StockHoldDelegate);
            DBC.SetStockHoldDelegate(stockhold);
        }
        private void MoneyDelegate(MoneyEntity ME)
        {

        }
        private void StockHoldDelegate(StockHoldEntity SHE)
        {

        }
    }
}

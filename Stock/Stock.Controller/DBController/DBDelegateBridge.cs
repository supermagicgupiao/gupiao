using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Stock.Controller.DBController.DBTable;

namespace Stock.Controller.DBController
{
    public class DBDelegateBridge
    {
        public delegate void UIMoney(MoneyEntity ME);
        private List<UIMoney> MoneyDelegateList = new List<UIMoney>();
        public delegate void UIStockHold(StockHoldEntity SHE);
        private List<UIStockHold> StockHoldDelegateList = new List<UIStockHold>();

        public DBDelegateBridge(DBDataController DBC)
        {
            DBDataController.MoneyDelegate money = new DBDataController.MoneyDelegate(MoneyDelegate);
            DBC.SetMoneyDelegate(money);
            DBDataController.StockHoldDelegate stockhold = new DBDataController.StockHoldDelegate(StockHoldDelegate);
            DBC.SetStockHoldDelegate(stockhold);
        }
        public void AddDelegate(UIMoney ui)
        {
            MoneyDelegateList.Add(ui);
        }
        public void AddDelegate(UIStockHold ui)
        {
            StockHoldDelegateList.Add(ui);
        }
        public void ClearDelegate()
        {
            MoneyDelegateList.Clear();
            StockHoldDelegateList.Clear();
        }
        private void MoneyDelegate(MoneyEntity ME)
        {
            foreach(UIMoney x in MoneyDelegateList)
            {
                x(ME);
            }
        }
        private void StockHoldDelegate(StockHoldEntity SHE)
        {
            foreach (UIStockHold x in StockHoldDelegateList)
            {
                x(SHE);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Controls;
using System.Windows;

using Stock.Controller.DBController;
using Stock.Controller.NetController;

namespace Stock.UIController
{
    class UserPanelController
    {
        private Dictionary<string, Canvas> CanvasDict;
        private static UserPanelController UPC;
        private UsersController users;
        private UserPanelController()
        {
            users = new UsersController();
            DB_ERROR DBE = users.GetLastError();
            if (DBE == DB_ERROR.DB_DATA_NOT_EXISTS)
            {
                InputMoney dlg = new InputMoney();
                dlg.ShowDialog();
                if (dlg.m == 0)
                    Application.Current.Shutdown();
                users.AddNewUser(dlg.n, dlg.m);
            }
            else
            {
                Adapter.ErrorAdapter.Show(DBE);
            }
        }
        public static UserPanelController Create()
        {
            if (UPC == null)
            {
                UPC = new UserPanelController();
                return UPC;
            }
            else
                return UPC;
        }
        public static UserPanelController Handler()
        {
            return UPC;
        }
    }
}

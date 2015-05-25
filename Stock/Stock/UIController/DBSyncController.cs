using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Stock.Controller.DBController;

namespace Stock.UIController
{
    class DBSyncController
    {
        private static DBDataController DBC = null;
        private DBSyncController() { }
        public static DBDataController Create()
        {
            if (DBC == null)
            {
                DBC = new DBDataController();
                return DBC;
            }
            else
                return DBC;
        }
        public static DBDataController Handler()
        {
            return DBC;
        }
    }
}

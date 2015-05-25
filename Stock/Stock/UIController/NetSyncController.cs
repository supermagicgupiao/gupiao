using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Stock.Controller.NetController;

namespace Stock.UIController
{
    class NetSyncController
    {
        private static NetDataController NDC = null;
        private NetSyncController() { }
        public static NetDataController Create()
        {
            if (NDC == null)
            {
                NDC = new NetDataController();
                return NDC;
            }
            else
                return NDC;
        }
        public static NetDataController Handler()
        {
            return NDC;
        }
    }
}

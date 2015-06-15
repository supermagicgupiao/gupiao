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
        private Dictionary<string, List<StockStateBox>> CanvasDict = new Dictionary<string, List<StockStateBox>>();
        private Dictionary<string, NetDataController> NetDict = new Dictionary<string, NetDataController>();
        private Dictionary<string, InfoDelegate> InfoSwitch = new Dictionary<string, InfoDelegate>();
        private InfoDelegate.SetWin setWin;
        private static UserPanelController UPC;
        private UsersController usersController;
        private DBDelegateBridge.UIMoney uim;
        private DBDelegateBridge.UIStockHold uis;
        public string name;
        private UserPanelController(ref DBDelegateBridge.UIMoney uim, ref DBDelegateBridge.UIStockHold uis, ref InfoDelegate.SetWin setWin)
        {
            this.uim = uim;
            this.uis = uis;
            this.setWin = setWin;
            usersController = new UsersController();
            DB_ERROR DBE = usersController.GetLastError();
            if (DBE == DB_ERROR.DB_DATA_NOT_EXISTS)
            {
                InputMoney dlg = new InputMoney();
                dlg.ShowDialog();
                if (dlg.m == 0)
                {
                    Application.Current.Shutdown();
                    return;
                }
                usersController.AddNewUser(dlg.n, dlg.m);
            }
            else if (DBE != DB_ERROR.DB_OK)
            {
                Adapter.ErrorAdapter.Show(DBE);
                Application.Current.Shutdown();
                return;
            }
            List<string> users = usersController.GetUserList();
            UserBoxController.Handler().setEventHandler(UserChange);
            foreach (string name in users)
            {
                //选择
                UserBoxController.Handler().Add(name, usersController.GetUserControler(name).PrincipalRead());
            }
            UserChange(users.First());
        }
        private void setCanvas(string name)
        {
            if (CanvasDict.ContainsKey(name))
            {
                List<StockStateBox> boxlist = CanvasDict[name];
                StockStateBoxController.Handler().setCanvas(ref boxlist);
            }
            else
            {
                List<StockStateBox> boxlist = new List<StockStateBox>();
                CanvasDict.Add(name, boxlist);
                StockStateBoxController.Handler().setCanvas(ref boxlist);
                StockStateBoxController.Handler().StockBoxInit();
            }
        }
        private DBDataController DBCmark = null;
        private void setDBDataController(string name)
        {
            if (DBCmark != null)
                DBCmark.DelegateController().ClearDelegate();
            DBDataController DBC = usersController.GetUserControler(name);
            DBSyncController.setDBDataController(ref DBC);
            if (uim != null)
            {
                DBC.DelegateController().AddDelegate(uim);
            }
            if (uis != null) 
            {
                DBC.DelegateController().AddDelegate(uis);
            }
            DBC.TriggerDelegate();
        }
        private NetDataController NDCmark = null;
        private void setNetDataController(string name)
        {
            if (NDCmark != null)
                NDCmark.StopRefresh();
            NetDataController NDC;
            if (NetDict.ContainsKey(name))
            {
                NDC = NetDict[name];
                NetSyncController.setNetDataController(ref NDC);
            }
            else
            {
                NDC = new NetDataController();
                NetDict.Add(name, NDC);
                NetSyncController.setNetDataController(ref NDC);
            }
            NDC.StartRefresh();
            NDCmark = NDC;
        }
        private void setInfoDelegate(string name)
        {
            InfoDelegate ID;
            if (InfoSwitch.ContainsKey(name))
            {
                ID = InfoSwitch[name];
            }
            else
            {
                ID = new InfoDelegate(setWin);
                InfoSwitch.Add(name, ID);
            }
            if (StockStateBoxController.ID != null)
                StockStateBoxController.ID.setDelegateFlag(false);
            StockStateBoxController.ID = ID;
            StockStateBoxController.ID.setDelegateFlag(true);
            ID.setValues();
        }
        private void UserChange(object sender, EventArgs e)
        {
            UserBox ub = (UserBox)sender;
            string name = ub.name.Content.ToString();
            if (this.name == name)
                return;
            if (name == "添加账户")
            {
                InputMoney dlg = new InputMoney();
                dlg.ShowDialog();
                if (dlg.m == 0)
                {
                    return;
                }
                name = dlg.n;
                if (usersController.GetUserControler(name) == null)
                {
                    UserPanelController.Handler().AddUser(dlg.n, dlg.m);
                    MainWindow.ShowNotifyMessage("成功添加新账户\n账户名:" + dlg.n + "本金:" + dlg.m);
                }
                else
                {
                    MessageBox.Show("账户名已存在");
                    return;
                }
            }
            UserChange(name);
        }
        public void UserChange(string name)
        {
            this.name = name;
            setDBDataController(name);
            setNetDataController(name);
            setInfoDelegate(name);
            setCanvas(name);
        }

        public void AddUser(string name,double m)
        {
            usersController.AddNewUser(name, m);
            ComboBoxItem cbi = new ComboBoxItem();
            cbi.Content = name;
            UserBoxController.Handler().Add(name, m);
        }
        public static UserPanelController Create(ref DBDelegateBridge.UIMoney uim, ref DBDelegateBridge.UIStockHold uis, ref InfoDelegate.SetWin setWin)
        {
            if (UPC == null)
            {
                UPC = new UserPanelController(ref uim, ref uis, ref setWin);
                return UPC;
            }
            else
                return UPC;
        }
        public static UserPanelController Handler()
        {
            return UPC;
        }
        public DBDataController DBControllerByName(string name)
        {
            return usersController.GetUserControler(name);
        }
        public List<string> GetUserList()
        {
            return usersController.GetUserList();
        }
        public void DelUser(string name)
        {
            if (usersController.GetUserList().Count == 1)
            {
                MessageBox.Show("不能删除最后一个账户!");
                return;
            }
            usersController.DelUser(name);
            UserBoxController.Handler().Remove(name);
            CanvasDict.Remove(name);
            NetDict.Remove(name);
            InfoSwitch.Remove(name);
            UserChange(usersController.GetUserList().First());
        }
    }
}

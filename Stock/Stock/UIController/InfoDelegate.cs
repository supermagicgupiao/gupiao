using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stock.UIController
{
    class InfoDelegate
    {
        private InfoEntity info;
        public delegate void SetWin(double m);
        private SetWin setWin;
        private bool delegateFlag = false;
        public InfoDelegate(SetWin setWin)
        {
            info = new InfoEntity();
            this.setWin = setWin;
        }
        public void change(InfoEntity e)
        {
            info.win += e.win;
            info.price += e.price;
            info.upwin += e.upwin;
            info.daywin += e.daywin;
            setValues();
        }
        public void setValues()
        {
            if (delegateFlag)
            {
                setWin(info.win);
                MainWindow.price = info.price;
                MainWindow.upwin = info.upwin;
                MainWindow.daywin = info.daywin;
            }
        }
        public void setDelegateFlag(bool b)
        {
            delegateFlag = b;
        }

    }
    public struct InfoEntity
    {
        public double win;
        public double price;
        public double upwin;
        public double daywin;
    }
}

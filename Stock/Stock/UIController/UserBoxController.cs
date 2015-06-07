using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace Stock.UIController
{
    class UserBoxController
    {
        private static UserBoxController UBC;
        private Canvas canvas;
        private MouseButtonEventHandler UserChange;
        private UserBoxController() { }
        public static UserBoxController Create(ref Canvas canvas)
        {
            if (UBC == null)
            {
                UBC = new UserBoxController();
                UBC.canvas = canvas;
                canvas.Children.Add(UBC.add);
                return UBC;
            }
            else
                return UBC;
        }
        public static UserBoxController Handler()
        {
            return UBC;
        }
        private UserBox add = new UserBox("添加账户");
        public void Add(string name,double pri)
        {
            double height = 0;
            if (UserBox.pre != null)
                height = UserBox.pre.Margin.Top + UserBox.pre.Height;
            UserBox ub = new UserBox(name, pri.ToString());
            ub.Margin = new Thickness(5, height + 5, 0, 0);
            ub.MouseLeftButtonDown += UserChange;
            //ub.MouseRightButtonDown += UserChange;
            canvas.Children.Add(ub);
            AddBoxMove();
            double h = add.Margin.Top + add.Height + 5;
            if (h < 677)
                canvas.Height = h;
        }
        public void Remove(string name)
        {
            bool flag = false;
            UserBox mark = null;
            UserBox premark = null;
            foreach(UserBox ub in canvas.Children)
            {
                if (flag == true)
                {
                    ub.Margin = new Thickness(5, ub.Margin.Top - ub.Height - 5, 0, 0);
                }
                if (ub.name.Content.ToString() == name)
                {
                    flag = true;
                    mark = ub;
                    if(ub == UserBox.pre)
                     UserBox.pre = premark;
                }
                premark = ub;
            }
            if (mark != null)
                canvas.Children.Remove(mark);
            AddBoxMove();
            double h = add.Margin.Top + add.Height + 5;
            if (h < 677)
                canvas.Height = h;
        }
        private void AddBoxMove()
        {
            add.Margin = new Thickness(5, UserBox.pre.Margin.Top + UserBox.pre.Height + 5, 0, 0);
        }
        public void setEventHandler(MouseButtonEventHandler UserChange)
        {
            this.UserChange = UserChange;
            add.MouseLeftButtonDown += UserChange;
        }
    }
}

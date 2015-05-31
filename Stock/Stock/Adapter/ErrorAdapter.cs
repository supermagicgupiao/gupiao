using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;

using Stock.Controller.DBController;
using Stock.Controller.NetController;

namespace Stock.Adapter
{
    class ErrorAdapter
    {
        public static void Show(DB_ERROR dbe)
        {
            if (dbe == DB_ERROR.DB_CANT_CONNECT)
            {
                MessageBox.Show("数据库无法连接");
            }
            else if (dbe == DB_ERROR.DB_TABLE_CRACK)
            {
                MessageBox.Show("数据库表损坏，无法重建");
            }
            else if (dbe == DB_ERROR.DB_DATA_NOT_EXISTS)
            {
                MessageBox.Show("数据不存在");
            }
            else if (dbe == DB_ERROR.DB_TABLE_CRACK_FIX)
            {
                MessageBox.Show("数据库损坏，已修复");
            }
            else if (dbe == DB_ERROR.DB_USER_TABLE_EXISTS)
            {
                MessageBox.Show("用户数据已经存在");
            }
            else if (dbe == DB_ERROR.DB_USER_EXISTS)
            {
                MessageBox.Show("用户已经存在");
            } 
            else if (dbe == DB_ERROR.DB_OK) 
            {
            }
        }
        public static void Show(NET_ERROR e)
        {
            if (e == NET_ERROR.NET_CANT_CONNECT)
            {
                MessageBox.Show("网络无法连接");
            }
            else if (e == NET_ERROR.NET_JSON_NOT_EXISTS)
            {
                MessageBox.Show("数据不存在");
            }
            else if (e == NET_ERROR.NET_REQ_ERROR)
            {
                MessageBox.Show("请求错误");
            }
        }
    }
}

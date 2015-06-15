using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Stock.Controller.DBController.DBTable;

namespace Stock.Controller.DBController
{
    public class DBDataThreadController
    {
        private DBDataController DBC;
        private DBDataThreadController(DBDataController DBC)
        {
            this.DBC = DBC;
        }
        public static DBDataThreadController DBDataThreadControllerHandler(DBDataController DBC)
        {
            return new DBDataThreadController(DBC);
        }
        //增加交易记录
        public void DealListAdd(DealListEntity DLE)
        {
            ThreadPool.QueueUserWorkItem(DealListAddThread, DLE);
        }
        private void DealListAddThread(object data)
        {
            DBC.DealListAdd((DealListEntity)data);
        }
        //批量增加
        public void DealListAdd(List<DealListEntity> DLEL)
        {
            ThreadPool.QueueUserWorkItem(DealListAddByListThread, DLEL);
        }
        private void DealListAddByListThread(object data)
        {
            DBC.DealListAdd((List<DealListEntity>)data);
        }
        //修改交易记录
        public void DealListUpdate(DealListEntity DLE)
        {
            ThreadPool.QueueUserWorkItem(DealListUpdateThread, DLE);
        }
        private void DealListUpdateThread(object data)
        {
            DBC.DealListUpdate((DealListEntity)data);
        }
        //删除交易记录
        public void DealListDelete(DealListEntity DLE)
        {
            ThreadPool.QueueUserWorkItem(DealListDeleteThread, DLE);
        }
        private void DealListDeleteThread(object data)
        {
            DBC.DealListDelete((DealListEntity)data);
        }
    }
}

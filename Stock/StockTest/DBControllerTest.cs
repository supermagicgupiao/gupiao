using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;
using System.Collections.Generic;

using Stock.Controller.DBController;
using Stock.Controller.DBController.DBTable;

namespace StockTest
{
    [TestClass]
    public class DBControllerTest
    {
        [TestMethod]
        //默认创建方式 检测数据库是否出现
        public void DBDataControllerCreateTest()
        {
            File.Delete(Environment.CurrentDirectory + "\\Stock.db");//先移除默认数据库
            DBDataController DBC = new DBDataController();
            DB_ERROR DBE = DBC.Check();
            Assert.AreEqual(DB_ERROR.DB_DATA_NOT_EXISTS, DBE);//正常返回数据不存在
            Assert.IsTrue(File.Exists(Environment.CurrentDirectory + "\\Stock.db"));//检测数据库是否生成
        }
        [TestMethod]
        //错误路径的构造方式
        public void DBDataControllerCheckConnectTest()
        {
            DBDataController DBC = new DBDataController("/r/n");
            DB_ERROR DBE = DBC.Check();
            Assert.AreEqual(DB_ERROR.DB_CANT_CONNECT, DBE);//数据库无法连接
        }
        [TestMethod]
        //crack数据库测试
        public void DBDataControllerCheckCrackTest()
        {
            DBDataController DBC = new DBDataController(Environment.CurrentDirectory + "\\TestDB\\crack.db");
            DB_ERROR DBE = DBC.Check();
            Assert.AreEqual(DB_ERROR.DB_TABLE_CRACK_FIX, DBE);//返回损坏并修复状态
        }
        [TestMethod]
        //没有本金数据库测试
        public void DBDataControllerCheckDataTest()
        {
            DBDataController DBC = new DBDataController(Environment.CurrentDirectory + "\\TestDB\\tableNoPrincipal.db");
            DB_ERROR DBE = DBC.Check();
            Assert.AreEqual(DB_ERROR.DB_DATA_NOT_EXISTS, DBE);//返回数据不存在
        }
        [TestMethod]
        //没表数据库测试
        public void DBDataControllerCheckTableTest()
        {
            DBDataController DBC = new DBDataController(Environment.CurrentDirectory + "\\TestDB\\noTable.db");
            DB_ERROR DBE = DBC.Check();
            Assert.AreEqual(DB_ERROR.DB_DATA_NOT_EXISTS, DBE);//返回数据不存在
        }
        [TestMethod]
        //数据库DealList读写测试
        public void DBDataControllerDealListTest()
        {
            File.Delete(Environment.CurrentDirectory + "\\Stock.db");//先移除默认数据库
            DBDataController DBC = new DBDataController();
            DB_ERROR DBE = DBC.Check();
            DealListEntity DLE = new DealListEntity();
            DLE.id = "600001";
            DLE.name = "邯郸钢铁";
            DBC.DealListAdd(DLE);//存入一条数据
            List<DealListEntity> DLEL;
            DBC.DealListReadAll(out DLEL);//读取
            Assert.AreEqual(1, DLEL.Count);
            Assert.AreEqual(DLE.id, DLEL[0].id);//判断读取与存入的数据是否一致
            Assert.AreEqual(DLE.name, DLEL[0].name);
        }
        [TestMethod]
        //数据库Principal读写测试
        public void DBDataControllerPrincipalTest()
        {
            File.Delete(Environment.CurrentDirectory + "\\Stock.db");//先移除默认数据库
            DBDataController DBC = new DBDataController();
            DB_ERROR DBE = DBC.Check();
            DBC.PrincipalCreate(100000000000000.00);
            Assert.AreEqual(100000000000000.00, DBC.PrincipalRead());
            for (int i = 0; i < 100; i++)
            {
                double m = 12.34 * i;
                DBC.PrincipalWrite(m);
                Assert.AreEqual(m.ToString(), DBC.PrincipalRead().ToString());
            }   
        }
        [TestMethod]
        //数据库Log读写测试
        public void DBDataControllerLogTest()
        {
            File.Delete(Environment.CurrentDirectory + "\\Stock.db");//先移除默认数据库
            DBDataController DBC = new DBDataController();
            DB_ERROR DBE = DBC.Check();
            LogEntity LE = new LogEntity();
            LE.state = "状态";
            LE.context = "错误内容";
            DBC.LogSave(LE);//存入一条数据
            List<LogEntity> LEL;
            DBC.LogRead(out LEL);//读取
            Assert.AreEqual(1, LEL.Count);
            Assert.AreEqual(LE.state, LEL[0].state);//判断读取与存入的数据是否一致
            Assert.AreEqual(LE.context, LEL[0].context);
        }
        [TestMethod]
        //数据库StockHold读测试
        public void DBDataControllerStockHoldTest()
        {
            File.Delete(Environment.CurrentDirectory + "\\Stock.db");//先移除默认数据库
            DBDataController DBC = new DBDataController();
            DB_ERROR DBE = DBC.Check();
            DealListEntity DLE = new DealListEntity();
            DLE.id = "600001";
            DLE.name = "邯郸钢铁";
            DLE.money = "10.11";
            DLE.number = "400";
            DLE.type = "买入";
            DBC.DealListAdd(DLE);//存入一条数据
            DLE.id = "600001";
            DLE.name = "邯郸钢铁";
            DLE.money = "5.11";
            DLE.number = "200";
            DLE.type = "卖出";
            DBC.DealListAdd(DLE);//再存入一条数据
            List<StockHoldEntity> SHEL;
            DBC.StockHoldReadAll(out SHEL);
            Assert.AreEqual(1, SHEL.Count);
            Assert.AreEqual(DLE.id, SHEL[0].id);//判断读取与存入的数据是否一致
            Assert.AreEqual(DLE.name, SHEL[0].name);
            Assert.AreEqual("200", SHEL[0].hold);//运算结果
            Assert.AreEqual("3022", SHEL[0].money);//运算结果
        }
    }
}

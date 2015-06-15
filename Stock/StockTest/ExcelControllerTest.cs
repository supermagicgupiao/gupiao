using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using Stock.Controller.ExcelController;
using Stock.Controller.DBController.DBTable;

namespace StockTest
{
    [TestClass]
    public class ExcelControllerTest
    {
        [TestMethod]
        //损坏测试
        public void ExcelControllerOleTest()
        {
            ExcelDataController EDC = new ExcelDataController();
            List<DealListEntity> DLEL;
            Assert.AreEqual(OPENEXCEL_ERROR.OLE_ERROR, EDC.Open(Environment.CurrentDirectory + "//TestExcel//股票数据损坏.xlsx", out DLEL));
        }
        [TestMethod]
        //缺列测试
        public void ExcelControllerFormatTest()
        {
            ExcelDataController EDC = new ExcelDataController();
            List<DealListEntity> DLEL;
            Assert.AreEqual(OPENEXCEL_ERROR.FORMAT_ERROR, EDC.Open(Environment.CurrentDirectory + "//TestExcel//股票数据缺列.xlsx", out DLEL));
            Assert.AreEqual(0, DLEL.Count);
        }
        [TestMethod]
        //xls和xlsx测试
        public void ExcelControllerVersionTest()
        {
            ExcelDataController EDC = new ExcelDataController();
            List<DealListEntity> DLEL;
            Assert.AreEqual(OPENEXCEL_ERROR.FORMAT_ERROR, EDC.Open(Environment.CurrentDirectory + "//TestExcel//股票数据.xlsx", out DLEL));
            Assert.AreEqual(OPENEXCEL_ERROR.FORMAT_ERROR, EDC.Open(Environment.CurrentDirectory + "//TestExcel//股票数据.xls", out DLEL));
        }
        [TestMethod]
        //读取数据测试
        public void ExcelControllerDataTest()
        {
            ExcelDataController EDC = new ExcelDataController();
            List<DealListEntity> DLEL;
            EDC.Open(Environment.CurrentDirectory + "//TestExcel//股票数据.xlsx", out DLEL);
            Assert.AreNotEqual(0, DLEL.Count);
            Assert.AreEqual("伊利股份", DLEL[0].name);
            Assert.AreEqual("600887", DLEL[0].id);
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading;

using Stock.Controller.NetController;

namespace StockTest
{
    [TestClass]
    public class NetControllerTest
    {
        [TestMethod]
        //正确的股票编号测试
        public void NetstatCheckReqOkTest()
        {
            Assert.AreEqual(NET_ERROR.NET_REQ_OK, NetState.Check("0600001"));
        }
        [TestMethod]
        //错误的股票编号测试
        public void NetstatCheckReqErrorTest()
        {
            Assert.AreEqual(NET_ERROR.NET_REQ_ERROR, NetState.Check("1600001"));
        }
        [TestMethod]
        //股票名称获取测试
        public void NetstatCheckNameTest()
        {
            string name;
            NetState.CheckName("0600001",out name);
            Assert.AreEqual("邯郸钢铁", name);
        }
        [TestMethod]
        //股票编号加sz或sh测试
        public void NetstatIdConvertTest()
        {
            string id = "600001";
            NetState.IdConvert(ref id);
            Assert.AreEqual("0600001", id);
        }
        [TestMethod]
        //网络定时获取数据委托测试
        public void NetDataControllerStartRefreshTest()
        {
            NetDataController NDC = new NetDataController();
            NetDataController.sync s=new NetDataController.sync(sync);
            NDC.StockRefreshAdd("0600001",ref s);//添加委托
            NDC.StartRefresh(1);//刷新时间1s
            Thread.Sleep(3000);//等待3s看是否成功
            Assert.AreEqual("邯郸钢铁", SIE.name);
        }
        private StockInfoEntity SIE;
        private void sync(StockInfoEntity SIE)
        {
            this.SIE = SIE;//委托修改
        }
    }
}

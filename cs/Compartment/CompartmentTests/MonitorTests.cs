using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MonitorPower.Tests
{
    [TestClass()]
    public class MonitorTests
    {
        [TestMethod()]
        public void PowerSaveTest()
        {
            Monitor.PowerSave();
            //Assert.Fail();
        }

        [TestMethod()]
        public void PowerOffTest()
        {
            Monitor.PowerOff();
            //Assert.Fail();
        }

        [TestMethod()]
        public void PowerOnTest()
        {
            Monitor.PowerOff();
            System.Threading.Thread.Sleep(1000);
            Monitor.PowerOn();
            //Assert.Fail();
        }
    }
}
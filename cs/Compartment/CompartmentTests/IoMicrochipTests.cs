using Microsoft.VisualStudio.TestTools.UnitTesting;
using Compartment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Compartment.Tests
{
    [TestClass()]
    public class IoMicrochipTests
    {
        [TestMethod()]
        public void StoreByteEdgeTest()
        {
            IoMicrochip microchip = new IoMicrochip();

            //var privateObject = new PrivateObject(microchip);

            //microchip.OnRecieveData(null, null);

            //var hoge = privateObject.GetFieldOrProperty("UsbRecieveData");

            byte nowData = 0b11101011;
            byte recentData = 0b11001011;
            byte resultCorrect = 0b00100000;

            byte result = microchip.StoreByteEdge(nowData, recentData);

            Debug.WriteLine("0x" + resultCorrect.ToString("X4"));
            Debug.WriteLine("0x" + result.ToString("X4"));

            Assert.AreEqual(resultCorrect, result);


            nowData = 0b11101011;
            recentData = 0b00001011;
            resultCorrect = 0b11100000;

            result = microchip.StoreByteEdge(nowData, recentData);

            Debug.WriteLine("0x" + resultCorrect.ToString("X4"));
            Debug.WriteLine("0x" + result.ToString("X4"));

            Assert.AreEqual(resultCorrect, result);
        }

        [TestMethod()]
        public void StoreByteFEdgeTest()
        {
            IoMicrochip microchip = new IoMicrochip();

            byte nowData = 0b11100011;
            byte recentData = 0b11101011;
            byte resultCorrect = 0b00001000;

            byte result = microchip.StoreByteFEdge(nowData, recentData);

            Debug.WriteLine("0x" + resultCorrect.ToString("X4"));
            Debug.WriteLine("0x" + result.ToString("X4"));

            Assert.AreEqual(resultCorrect, result);

            nowData = 0b11100011;
            recentData = 0b01001011;
            resultCorrect = 0b00001000;

            result = microchip.StoreByteFEdge(nowData, recentData);

            Debug.WriteLine("0x" + resultCorrect.ToString("X4"));
            Debug.WriteLine("0x" + result.ToString("X4"));

            Assert.AreEqual(resultCorrect, result);

        }

        [TestMethod()]
        public void SetUpperStateOfDOutTest()
        {
            IoMicrochip microchip = new IoMicrochip();
            microchip.AcquireDevice();

            microchip.SetUpperStateOfDOut(IoBoardDOutLogicalName.RoomLampOn);
            Task.Delay(100).Wait();
            microchip.SetUpperStateOfDOut(IoBoardDOutLogicalName.FeedLampOn);
            Task.Delay(100).Wait();
            microchip.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.ExtraFeederStart, true, 0x4);
            Task.Delay(200).Wait();
            microchip.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.ExtraFeederStart, false, 0x4);
            Task.Delay(100).Wait();
            microchip.SetUpperStateOfDOut(IoBoardDOutLogicalName.FeedLampOff);
            Task.Delay(100).Wait();
            var ret = microchip.SetUpperStateOfDOut(IoBoardDOutLogicalName.RoomLampOff);
            Task.Delay(100).Wait();
            Assert.IsTrue(ret);
            microchip.ReleaseDevice();
        }
        [TestMethod()]
        public void FeedLampTest()
        {
            IoMicrochip microchip = new IoMicrochip();
            microchip.AcquireDevice();

            microchip.SetUpperStateOfDOut(IoBoardDOutLogicalName.FeedLampOn);
            Task.Delay(200).Wait();
            microchip.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.ExtraFeederStart, true, 0x4);
            Task.Delay(100).Wait();
            microchip.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.ExtraFeeder2Start, true, 0x4);
            Task.Delay(100).Wait();
            var ret = microchip.SetUpperStateOfDOut(IoBoardDOutLogicalName.FeedLampOff);
            Task.Delay(200).Wait();
            microchip.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.ExtraFeederStart, false, 0x4);
            Task.Delay(100).Wait();
            microchip.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.ExtraFeeder2Start, false, 0x4);
            Task.Delay(300).Wait();
            Assert.IsTrue(ret);
            microchip.ReleaseDevice();
        }
    }
}
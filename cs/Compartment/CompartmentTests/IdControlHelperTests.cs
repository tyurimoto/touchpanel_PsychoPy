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
    public class IdControlHelperTests
    {
        [TestMethod()]
        public void IdControlHelperTest()
        {
            IdControlHelper idControlHelper = new IdControlHelper();
            Assert.IsNotNull(idControlHelper);
        }

        [TestMethod()]
        public void CheckExpireTest()
        {
            IdControlHelper idControlHelper = new IdControlHelper();
            idControlHelper.AddEntry("392144000336485");
            idControlHelper.AddEntry("392144000347682");
            idControlHelper.AddEntry("392144000336481");

            idControlHelper.CheckExpire();
            System.Threading.Thread.Sleep(1000);
            idControlHelper.CheckExpire();
        }

        [TestMethod()]
        public void AddEntryTest()
        {
            IdControlHelper idControlHelper = new IdControlHelper();
            idControlHelper.AddEntry("392144000336485");
            idControlHelper.AddEntry("392144000347682");
            idControlHelper.AddEntry("392144000336481");

            Assert.IsTrue(idControlHelper.FindId("392144000336485"));
            Assert.IsTrue(idControlHelper.FindId("392144000347682"));
            Assert.IsTrue(idControlHelper.FindId("392144000336481"));

        }

        [TestMethod()]
        public void CreateSourceDictionaryTest()
        {
            IdControlHelper idControlHelper = new IdControlHelper();
            idControlHelper.CreateSourceDictionary();
            Assert.IsTrue(idControlHelper.FindId("392144000336485"));
            Assert.IsTrue(idControlHelper.FindId("392144000347682"));

        }
        [TestMethod()]
        public void EntryPerformanceTest()
        {
            const int TestCount = 100000;

            IdControlHelper idControlHelper = new IdControlHelper();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Parallel.For(0, TestCount, i => idControlHelper.AddEntry(i.ToString()));
            //for(int i=0;i<TestCount;i++)
            //{
            //    idControlHelper.AddEntry(i.ToString());
            //}
            stopwatch.Stop();

            // Entry update.
            idControlHelper.AddEntry("392144000336485");

            Debug.WriteLine("AddEntry: " + stopwatch.ElapsedMilliseconds + "ms");
            Debug.WriteLine("EntryCount: " + idControlHelper.KeyPairsCount);

            stopwatch.Restart();
            idControlHelper.CheckExpire();
            stopwatch.Stop();
            Debug.WriteLine("CheckExpire: " + stopwatch.ElapsedMilliseconds + "ms");
            Debug.WriteLine("EntryCount: " + idControlHelper.KeyPairsCount);
        }

        [TestMethod()]
        public void AddCountTest()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            IdControlHelper idControlHelper = new IdControlHelper();
            Debug.WriteLine(stopwatch.ElapsedMilliseconds + " ms");
            stopwatch.Restart();
            // Entry update.
            idControlHelper.AddEntry("392144000336485");
            Debug.WriteLine(idControlHelper.GetCount("392144000336485").ToString() + " " + stopwatch.ElapsedMilliseconds + " ms");
            stopwatch.Restart();
            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(i, idControlHelper.GetCount("392144000336485"));
                idControlHelper.AddCount("392144000336485");
                Debug.WriteLine(idControlHelper.GetCount("392144000336485").ToString() + " " + stopwatch.ElapsedTicks + " tick");
            }
        }

        [TestMethod()]
        public void SaveIdTest()
        {
            const int TestCount = 100000;

            IdControlHelper idControlHelper = new IdControlHelper();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Parallel.For(0, TestCount, i => idControlHelper.AddEntry(i.ToString()));
            //for(int i=0;i<TestCount;i++)
            //{
            //    idControlHelper.AddEntry(i.ToString());
            //}
            stopwatch.Stop();

            // Entry update.
            idControlHelper.AddEntry("392144000336485");

            Debug.WriteLine("AddEntry: " + stopwatch.ElapsedMilliseconds + "ms");
            Debug.WriteLine("EntryCount: " + idControlHelper.KeyPairsCount);

            stopwatch.Restart();
            idControlHelper.CheckExpire();
            stopwatch.Stop();
            Debug.WriteLine("CheckExpire: " + stopwatch.ElapsedMilliseconds + "ms");
            Debug.WriteLine("EntryCount: " + idControlHelper.KeyPairsCount);
            idControlHelper.SaveId(@"C:\Users\kodama\source\repos\compartment\cs\Compartment\Compartment\bin\Debug\hoge.json");

            IdControlHelper idControlHelper1 = new IdControlHelper(@"C:\Users\kodama\source\repos\compartment\cs\Compartment\Compartment\bin\Debug\hoge.json");

            Assert.AreEqual(idControlHelper.KeyPairsCount, idControlHelper1.KeyPairsCount);
        }

        [TestMethod()]
        public void UpdateEntryTest()
        {
            const string Id = "392144000336485";
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            IdControlHelper idControlHelper = new IdControlHelper();
            Debug.WriteLine(stopwatch.ElapsedMilliseconds + " ms");
            stopwatch.Restart();
            // Entry update.
            idControlHelper.AddEntry(Id);
            long expireTime1 = idControlHelper.GetEntry(Id).Item1;
            Debug.WriteLine(expireTime1 + " " + stopwatch.ElapsedTicks + " tick");
            stopwatch.Restart();
            System.Threading.Thread.Sleep(10);
            idControlHelper.UpdateEntry(Id);

            long expireTime2 = idControlHelper.GetEntry(Id).Item1;
            Debug.WriteLine(expireTime2 + " " + stopwatch.ElapsedTicks + " tick");
            Assert.AreNotEqual(expireTime1, expireTime2);
        }

        [TestMethod()]
        public void RemoveEntryTest()
        {
            IdControlHelper idControlHelper = new IdControlHelper();
            idControlHelper.CreateSourceDictionary();
            Assert.IsTrue(idControlHelper.FindId("392144000336485"));
            Assert.IsTrue(idControlHelper.FindId("392144000347682"));
            idControlHelper.RemoveEntry("392144000336485");
            Assert.IsFalse(idControlHelper.FindId("392144000336485"));
            idControlHelper.RemoveEntry("392144000347682");
            Assert.IsFalse(idControlHelper.FindId("392144000347682"));
        }

        [TestMethod()]
        public void CheckElaspedTimeTest()
        {
            const string Id = "392144000336485";
            
            IdControlHelper idControlHelper = new IdControlHelper();
            idControlHelper.AddEntry(Id);
            System.Threading.Thread.Sleep(1000);
            Assert.IsTrue(idControlHelper.CheckElaspedTime(Id, 1));

            idControlHelper.UpdateEntry(Id);
            Assert.IsFalse(idControlHelper.CheckElaspedTime(Id, 1));

        }
    }
}
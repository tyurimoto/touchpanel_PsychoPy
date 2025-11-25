using Microsoft.VisualStudio.TestTools.UnitTesting;
using Compartment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Concurrent;

namespace Compartment.Tests
{
    [TestClass()]
    public class OperationActionHelperTests
    {
        private readonly object SyncObject = new object();
        [TestMethod()]
        public void DoAllTaskTest()
        {
            OperationActionHelper oah = new OperationActionHelper();
            int a = 1;
            List<int> output = new List<int>();
            for (int i = 0; i < 10; i++)
            {
                oah.SetAction(() =>
                {
                    lock (SyncObject)
                    {
                        a++;
                    }
                    Debug.WriteLine(a);
                    output.Add(a);
                });
            }
            oah.DoAllTask().Wait();
            Assert.IsTrue(a == 11);
            List<int> expected = Enumerable.Range(2, 10).ToList();
            CollectionAssert.AreEqual(expected, output);
        }
        [TestMethod()]
        public void DoAllTaskrunTest()
        {
            OperationActionHelper oah = new OperationActionHelper();
            int a = 1;
            for (int i = 0; i < 10; i++)
            {
                oah.SetAction(() =>
                {
                    lock (SyncObject)
                    {
                        a++;
                    }
                    Debug.WriteLine(a);
                });
            }
            var task = oah.DoAllTask();

            Assert.IsFalse(oah.InterruptFlag);

            task.Wait();
            Assert.IsFalse(oah.InterruptFlag);
        }
        [TestMethod()]
        public void DoAllTaskRepeatTest()
        {
            OperationActionHelper oah = new OperationActionHelper(5);
            int a = 1;
            List<int> output = new List<int>();
            for (int i = 0; i < 10; i++)
            {
                oah.SetAction(() =>
                {
                    lock (SyncObject)
                    {
                        a++;
                    }
                    Debug.WriteLine(a);
                    output.Add(a);
                });
            }
            oah.DoAllTask().Wait();

            Assert.IsTrue(a == 51);
            List<int> expected = Enumerable.Range(2, 50).ToList();
            CollectionAssert.AreEqual(expected, output);

        }
        [TestMethod()]
        public void AddEventTest()
        {
            var mainForm = new FormMain();
            var preferenceDat = new PreferencesDat();
            var privateObject = new PrivateObject(mainForm);

            var ret = privateObject.Invoke("InitializeComponentOnUcOperation");

            UcOperationBlock uob = new UcOperationBlock(mainForm);

            uob.DrawScreenBackColor();
            uob.DrawScreenReset();
            uob.Start();

        }

        [TestMethod()]
        public void StatingEndingTest()
        {
            OperationActionHelper oah = new OperationActionHelper(5);
            int a = 1;
            List<int> outputValue = new List<int>();
            for (int i = 0; i < 10; i++)
            {
                oah.SetAction(() =>
                {
                    lock (SyncObject)
                    {
                        a++;
                    }
                    Debug.WriteLine(a);
                    outputValue.Add(a);
                });
            }
            oah.SetStartingAction(() =>
            {
                lock (SyncObject)
                {
                    a = 10;
                }
                Debug.WriteLine(a);
                outputValue.Add(a);
            });
            oah.SetEndingAction(() =>
            {
                lock (SyncObject)
                {
                    a = 51;
                }
                Debug.WriteLine(a);
                outputValue.Add(a);
            });
            oah.DoAllTask().Wait();

            Assert.IsTrue(a == 51);
            //List<int> expected = new List<int> { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 51, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 51, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 51, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 51, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 51 };
            var baseRange = Enumerable.Range(10, 11).Concat(new[] { 51 });
            var expectedRange = Enumerable.Repeat(baseRange, 5).SelectMany(x => x).ToList();

            CollectionAssert.AreEqual(expectedRange, outputValue);
        }
        [TestMethod()]
        public void OperationHelper多重Test()
        {
            OperationActionHelper oah = new OperationActionHelper(5);
            int a = 1;
            List<int> outputValue = new List<int>();
            object SyncObjectlocal = new object();
            for (int i = 0; i < 10; i++)
            {
                oah.SetAction(() =>
                {
                    lock (SyncObjectlocal)
                    {
                        a++;
                    }
                    Debug.WriteLine(a);
                    outputValue.Add(a);
                });
            }
            oah.SetStartingAction(() =>
            {
                lock (SyncObjectlocal)
                {
                    a = 10;
                }
                Debug.WriteLine(a);
                outputValue.Add(a);
            });
            oah.SetEndingAction(() =>
            {
                lock (SyncObjectlocal)
                {
                    a = 51;
                }
                Debug.WriteLine(a);
                outputValue.Add(a);
            });
            oah.DoAllTask().Wait();
            //await oah.DoAllTask();
            Assert.AreEqual(0, oah.CurrentActionCount);
            Assert.IsTrue(a == 51);

            var baseRange = Enumerable.Range(10, 11).Concat(new[] { 51 });
            var expectedRange = Enumerable.Repeat(baseRange, oah.RepeatCount).SelectMany(x => x).ToList();

            CollectionAssert.AreEqual(expectedRange, outputValue);
            Debug.WriteLine("---");

            //oah.ClearAction();
            outputValue.Clear();
            oah.RepeatCount = 1;
            for (int i = 0; i < 5; i++)
            {
                oah.SetAction(() =>
                {
                    lock (SyncObjectlocal)
                    {
                        a++;
                    }
                    Debug.WriteLine(a);
                    outputValue.Add(a);
                });
            }
            oah.SetStartingAction(() =>
            {
                lock (SyncObjectlocal)
                {
                    a = 15;
                }
                Debug.WriteLine(a);
                outputValue.Add(a);
            });
            oah.SetEndingAction(() =>
            {
                lock (SyncObjectlocal)
                {
                    a = 51;
                }
                Debug.WriteLine(a);
                outputValue.Add(a);
            });
            oah.DoAllTask().Wait();
            Assert.AreEqual(0, oah.CurrentActionCount);
            baseRange = Enumerable.Range(15, 6).Concat(new[] { 51 });
            expectedRange = Enumerable.Repeat(baseRange, oah.RepeatCount).SelectMany(x => x).ToList();

            CollectionAssert.AreEqual(expectedRange, outputValue);
            Debug.WriteLine("---");
            outputValue.Clear();
            oah.RepeatCount = 3;
            for (int i = 0; i < 3; i++)
            {
                oah.SetAction(() =>
                {
                    lock (SyncObjectlocal)
                    {
                        a++;
                    }
                    Debug.WriteLine(a);
                    outputValue.Add(a);
                });
            }
            oah.SetStartingAction(() =>
            {
                lock (SyncObjectlocal)
                {
                    a = 5;
                }
                Debug.WriteLine(a);
                outputValue.Add(a);
            });
            oah.SetEndingAction(() =>
            {
                lock (SyncObjectlocal)
                {
                    a = 51;
                }
                Debug.WriteLine(a);
                outputValue.Add(a);
            });
            oah.DoAllTask().Wait();
            Assert.AreEqual(0, oah.CurrentActionCount);
            baseRange = Enumerable.Range(5, 4).Concat(new[] { 51 });
            expectedRange = Enumerable.Repeat(baseRange, oah.RepeatCount).SelectMany(x => x).ToList();

            CollectionAssert.AreEqual(expectedRange, outputValue);
        }
        [TestMethod()]
        public void StatingEndingAsyncTest()
        {
            int repeatCount = 5;
            OperationActionHelper oah = new OperationActionHelper(repeatCount);
            int a = 1;
            List<int> outputValue = new List<int>();
            for (int i = 0; i < 10; i++)
            {
                // Asyncで実行
                oah.SetActionAsync(() =>
                {
                    lock (SyncObject)
                    {
                        a++;
                    }
                    Debug.WriteLine(a);
                    outputValue.Add(a);
                });
            }
            oah.SetStartingAction(() =>
            {
                lock (SyncObject)
                {
                    a = 10;
                }
                Debug.WriteLine(a);
                outputValue.Add(a);
            });
            oah.SetEndingAction(() =>
            {
                lock (SyncObject)
                {
                    a = 51;
                }
                Debug.WriteLine(a);
                outputValue.Add(a);
            });
            oah.DoAllTask().Wait();

            Assert.IsTrue(a == 51);
            //List<int> expected = new List<int> { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 51, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 51, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 51, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 51, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 51 };
            Assert.AreEqual(10, outputValue[12 * 0]);
            Assert.AreEqual(10, outputValue[12 * 1]);
            Assert.AreEqual(10, outputValue[12 * 2]);
            Assert.AreEqual(10, outputValue[12 * 3]);
            Assert.AreEqual(10, outputValue[12 * 4]);
            Assert.AreEqual(51, outputValue[11]);
            Assert.AreEqual(51, outputValue[11 + 12]);
            Assert.AreEqual(51, outputValue[11 + 12 * 2]);
            Assert.AreEqual(51, outputValue[11 + 12 * 3]);
            Assert.AreEqual(51, outputValue[11 + 12 * 4]);

            Assert.AreEqual(12 * repeatCount, outputValue.Count);
        }
        [TestMethod()]
        public void SetConcurrentQueueTest()
        {
            var initValue = new List<int> { 1, 2, 3 };
            ConcurrentQueue<int> baseQueue = new ConcurrentQueue<int>();
            ConcurrentQueue<int> cq = new ConcurrentQueue<int>(initValue);
            OperationActionHelper.SetConcurrentQueue(ref baseQueue, cq);

            var baseQueueArray = baseQueue.ToArray();
            var initArray = initValue.ToArray();

            CollectionAssert.AreEqual(initArray, baseQueueArray);
        }
        [TestMethod()]
        public void SetConcurrentQueueExcecuteTest()
        {
            var initValue = new List<int> { 1, 2, 3 };
            ConcurrentQueue<int> baseQueue = new ConcurrentQueue<int>();
            ConcurrentQueue<int> cq = new ConcurrentQueue<int>(initValue);
            OperationActionHelper.SetConcurrentQueue(ref baseQueue, cq);

            while (cq.Count > 0)
            {
                cq.TryDequeue(out int _);
            }

            var baseQueueArray = baseQueue.ToArray();
            var initArray = initValue.ToArray();

            CollectionAssert.AreEqual(initArray, baseQueueArray);

            cq = new ConcurrentQueue<int>(initValue);
            OperationActionHelper.SetConcurrentQueue(ref baseQueue, cq);
            while (baseQueue.Count > 0)
            {
                baseQueue.TryDequeue(out int _);
            }

            var cqArray = cq.ToArray();
            initArray = initValue.ToArray();

            CollectionAssert.AreEqual(initArray, cqArray);
        }
    }
}
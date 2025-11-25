using Microsoft.VisualStudio.TestTools.UnitTesting;
using Compartment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;
using System.Threading;
using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace Compartment.Tests
{
    [TestClass()]
    public class EDoorTests
    {
        [TestMethod()]
        public void eDoorDurabilityTest()
        {
            FormMain form = new FormMain();

            var privateObject = new PrivateObject(form);

            var ioBoard = privateObject.GetFieldOrProperty("ioBoardDevice");

            Assert.IsTrue(((IoMicrochip)ioBoard).AcquireDevice());
            form.eDoor = new EDoor((IoMicrochip)ioBoard, form);
            form.eDoor.Start();
            try
            {
                form.eDoor.DoorOpen(200);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
        [TestMethod()]
        public void DoorCloseTest()
        {
            Task[] tasks = new Task[5];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = doorTestSubMethod();
            }
            //var task1 = doorTestSubMethod();
            //var task2 = doorTestSubMethod();
            //var task3 = doorTestSubMethod();
            //var task4 = doorTestSubMethod();
            //var task5 = doorwaitMethod();
            //Console.WriteLine("Start");
            //task1.Wait();
            //task2.Wait();
            //task3.Wait();
            //task4.Wait();
            ////task5.Wait();
            ////Task.WhenAll(task1, task2, task3, task4);
            //task5.Wait();
            try
            {
                Task.WaitAll(tasks);
            }
            catch (AggregateException ae)
            {
                Console.WriteLine("One or more exceptions occurred:");
                foreach (var ex in ae.InnerExceptions)
                    Console.WriteLine("   {0}: {1}", ex.GetType().Name, ex.Message);
            }

            Console.WriteLine("\nStatus of tasks:");
            foreach (var t in tasks)
            {
                Console.WriteLine("   Task #{0}: {1}", t.Id, t.Status);
                if (t.Exception != null)
                {
                    foreach (var ex in t.Exception.InnerExceptions)
                        Console.WriteLine("      {0}: {1}", ex.GetType().Name, ex.Message);
                }

            }
        }

        [TestMethod()]
        public void DoorCancelTest()
        {
            SyncObject<bool> DoorOperationBusy = new SyncObject<bool>(true);
            var task = Task.Run(() =>
            {
                while (DoorOperationBusy.Value)
                {
                    Thread.Sleep(100);
                }
            });
            DoorOperationBusy.Value = false;
            task.Wait();
            Console.WriteLine("   Task #{0}: {1}", task.Id, task.Status);
        }

        private Task doorTestSubMethod()
        {
            SyncObject<bool> DoorOperationBusy = new SyncObject<bool>(false);
            Stopwatch sw = new Stopwatch();
            sw.Reset();

            DoorOperationBusy.Value = true;
            var task = Task.Run(() =>
            {
                sw.Start();
                while (DoorOperationBusy.Value)
                {
                    Thread.Sleep(100);
                    if (sw.ElapsedMilliseconds > 10000)
                    {
                        DoorOperationBusy.Value = false;
                        Console.WriteLine("Task end.");
                    }
                }
            });
            task.ConfigureAwait(false);
            return task;
        }

        public Task doorwaitMethod()
        {
            SyncObject<bool> DoorOperationBusy = new SyncObject<bool>(false);
            Stopwatch sw = new Stopwatch();
            sw.Reset();

            DoorOperationBusy.Value = true;
            if (!DoorOperationBusy.Value)
            {
                return Task.FromResult<string>("hoge");
            }
            var tcs = new TaskCompletionSource<string>();

            Action a = () =>
                {
                    sw.Start();
                    while (DoorOperationBusy.Value)
                    {
                        Thread.Sleep(100);
                        if (sw.ElapsedMilliseconds > 10000)
                        {
                            DoorOperationBusy.Value = false;
                            Console.WriteLine("Task end.TaskCompletion");
                            tcs.TrySetResult("Task end");
                        }
                    }
                };

            a();

            return tcs.Task;
        }
    }

}
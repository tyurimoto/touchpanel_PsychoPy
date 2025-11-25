using Compartment;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Compartment.Tests
{
    [TestClass()]
    public class UcOperationBlockTests
    {
        [TestMethod()]
        public void JsonToOperationProcTest()
        {

            UcOperationBlock ucOperationBlock = new UcOperationBlock(new FormMain());
            string inputStr = "[{\"ActionName\":\"hogehoge\",\"param1\":19,\"param2\":0},{\"ActionName\":\"hogehoA\",\"param1\":0,\"param2\":0},{\"ActionName\":\"Ahogechan\",\"param1\":0,\"param2\":0},{\"ActionName\":\"hoge\",\"param1\":121123,\"param2\":0},{\"ActionName\":\"hogehage\",\"param1\":1,\"param2\":0},{\"ActionName\":\"hogehogege\",\"param1\":0,\"param2\":0},{\"ActionName\":\"Delay\",\"param1\":10,\"param2\":1000}]";
            Assert.ThrowsException<Exception>(() => { ucOperationBlock.JsonToOperationProc(inputStr); });
            //Assert.Fail();
        }

        [TestMethod()]
        public void ScriptToJsonRegexTest()
        {
            string str = "hogehoge(19)\r\nhogehoA\r\nAhogechan(aa)\r\nhoge(121123)\r\nhogehage(1)\r\nhogehogege\r\nDelay(10-1000))";
            string[] strList = str.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            var rx = new Regex(@"\(\d+\)", RegexOptions.IgnoreCase);
            var selected = from hoge in strList
                               //where rx.IsMatch(hoge)
                           select hoge;

            List<MatchCollection> mc = new List<MatchCollection>();
            List<string> stringParams = new List<string>();


            foreach (string s in selected)
            {
                if (Regex.IsMatch(s, @"\(\d+\)"))
                {
                    string[] ss = Regex.Split(s, @"\(");
                    Debug.WriteLine(ss[0]);
                    mc.Add(Regex.Matches(s, @"\d+"));
                    stringParams.Add(Regex.Match(s, @"\d+").ToString());
                }
                else
                {
                    string hoge = Regex.Match(s, @"[a-z]*", RegexOptions.IgnoreCase).ToString();
                    Debug.WriteLine(hoge);
                    stringParams.Add("0");
                }


            }
            foreach (string n in stringParams)
            {
                Debug.WriteLine(n);
            }
            //MatchCollection mc2 = null;
            //foreach (string s in selected)
            //{
            //    mc2 = Regex.Matches(s, @"\(\d+\)");
            //}
            //foreach (var s in selected)
            //{
            //    Debug.WriteLine(s.ToString());
            //    //Assert.AreEqual(s, "hogehoge(19)");
            //}
            //foreach (var m in mc)
            //{
            //    foreach (var mm in m)
            //        Debug.WriteLine(mm.ToString());
            //}
            //foreach (var m in mc2)
            //{
            //    Debug.WriteLine(m.ToString());
            //}


        }
        [TestMethod()]
        public void PlaySoundTest()
        {
            var mainForm = new FormMain();
            var preferenceDat = new PreferencesDat();
            var privateObject = new PrivateObject(mainForm);

            var ret = privateObject.Invoke("InitializeComponentOnUcOperation");

            UcOperationBlock uob = new UcOperationBlock(mainForm);
            preferenceDat.SoundFileOfCorrect = @"C:\\Windows\\Media\\tada.wav";
            uob.PlaySound();
            //uob.DrawScreenReset();
            uob.Start();

        }
        [TestMethod()]
        public void ScriptToJsonValueTest()
        {
            string str = "hogehoge(19)\r\nhogehoA\r\nAhogechan(aa)\r\nhoge(121123)\r\nhogehage(1)\r\nhogehogege\r\nDelay(10-1000))";
            string[] strList = str.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            string resultStr = UcOperationBlock.ScriptToJson(str);
            Debug.WriteLine(resultStr);
            Assert.AreEqual(resultStr, "[{\"ActionName\":\"hogehoge\",\"param1\":19,\"param2\":0,\"param3\":null},{\"ActionName\":\"hogehoA\",\"param1\":0,\"param2\":0,\"param3\":null},{\"ActionName\":\"Ahogechan\",\"param1\":0,\"param2\":0,\"param3\":null},{\"ActionName\":\"hoge\",\"param1\":121123,\"param2\":0,\"param3\":null},{\"ActionName\":\"hogehage\",\"param1\":1,\"param2\":0,\"param3\":null},{\"ActionName\":\"hogehogege\",\"param1\":0,\"param2\":0,\"param3\":null},{\"ActionName\":\"Delay\",\"param1\":10,\"param2\":1000,\"param3\":null}]");
        }

        [TestMethod()]
        public void JsonToScriptValueTest()
        {
            string str = "[{\"ActionName\":\"hogehoge\",\"param1\":19,\"param2\":0},{\"ActionName\":\"hogehoA\",\"param1\":0,\"param2\":0},{\"ActionName\":\"Ahogechan\",\"param1\":0,\"param2\":0},{\"ActionName\":\"hoge\",\"param1\":121123,\"param2\":0},{\"ActionName\":\"hogehage\",\"param1\":1,\"param2\":0},{\"ActionName\":\"hogehogege\",\"param1\":0,\"param2\":0},{\"ActionName\":\"Delay\",\"param1\":10,\"param2\":1000}]";
            string[] strList = str.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            string resultStr = UcOperationBlock.JsonToScript(str);
            Debug.WriteLine(resultStr);
            Assert.AreEqual(resultStr, "hogehoge(19)\r\nhogehoA\r\nAhogechan\r\nhoge(121123)\r\nhogehage(1)\r\nhogehogege\r\nDelay(10-1000)\r\n");
        }

        [TestMethod()]
        public void GenerateRandomTest()
        {
            UcOperationBlock ucOperationBlock = new UcOperationBlock(new FormMain());
            object syncObject = new object();
            ParallelOptions options = new ParallelOptions();
            Parallel.For(0, 100, options, (i, loopkstate) =>
            {
                int n = ucOperationBlock.GenerateRandom(1, 100);
                lock (syncObject)
                {
                    Assert.IsTrue(n < 100);
                    Assert.IsTrue(0 < n);
                }
                Debug.WriteLine(n.ToString());
            }
            );
        }
        [TestMethod()]
        public void ActionGenerateRandomTest()
        {
            UcOperationBlock ucOperationBlock = new UcOperationBlock(new FormMain());
            object syncObject = new object();
            ParallelOptions options = new ParallelOptions();

            Func<int> aAction = () => { return ucOperationBlock.GenerateRandom(1, 100); };
            Func<int> bAction = () => { return aAction(); };
            Func<int> c;
            ConcurrentQueue<Func<int>> queue = new ConcurrentQueue<Func<int>>();
            _ = Parallel.For(0, 100, options, (i, loopkstate) =>
              {
                  //int n = ucOperationBlock.GenerateRandom(1, 100);
                  queue.Enqueue(() => { return bAction(); });
                  int n = 0;
                  queue.TryDequeue(out c);
                  n = c();
                  lock (syncObject)
                  {
                      Assert.IsTrue(n < 100);
                      Assert.IsTrue(0 < n);
                  }
                  Debug.WriteLine(n.ToString());
              }
);
        }
    }
}
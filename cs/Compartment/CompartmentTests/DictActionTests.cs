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
    public class DictActionTests
    {
        [TestMethod()]
        public void DictPrameterTest()
        {
            int inputValue = 10;
            DictAction dictAction = new DictAction();
            Action<int, int> TestAction = (x, y) => { testMethod(x, y); };

            dictAction.SetAction(TestAction, "test");

            //Action<int> resultAction = (x) => { dictAction.GetAction("test", x); };

            //resultAction(inputValue);

            (dictAction.GetAction("test") as Action<int, int>)?.Invoke(inputValue, 0);

        }

        public void testMethod(int x)
        {
            Debug.WriteLine(x);
        }
        public void testMethod(int x, int y)
        {
            Debug.WriteLine(x.ToString() + " " + y.ToString());
        }
        [TestMethod()]
        public void DictDenpanTest()
        {
            int inputValue = 10;
            DictAction dictAction = new DictAction();
            Action<int, int> TestAction = (x, y) => { testMethod(x); };

            dictAction.SetAction(TestAction, "test");

            (dictAction.GetAction("test") as Action<int, int>)?.Invoke(inputValue, 0);



        }
    }
}
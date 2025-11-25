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
    public class SettingSearcherTests
    {
        [TestMethod()]
        public void SearchSettingTest()
        {
            Debug.WriteLine(SettingSearcher.SearchSetting());
            
        }
    }
}
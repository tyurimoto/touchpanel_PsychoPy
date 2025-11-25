using Microsoft.VisualStudio.TestTools.UnitTesting;
using Darknet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Policy;

namespace Darknet.Tests
{
    [TestClass()]
    public class YoloWrapperTests
    {
        [TestMethod()]
        public void YoloWrapperTest()
        {
            string cfg = System.IO.Path.Combine(@"..\..\..\Compartment\", "darknet", "yolov7-tiny_marmo_test.cfg");
            string weights = System.IO.Path.Combine(@"..\..\..\Compartment\", "darknet", "yolov7-tiny_marmo_training_1400000.weights");
            Assert.IsTrue(System.IO.File.Exists(cfg));
            Assert.IsTrue(System.IO.File.Exists(weights));
            YoloWrapper yoloWrapper = new YoloWrapper(cfg, weights, 0);
            Assert.IsNotNull(yoloWrapper);
        }
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Compartment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom;

namespace Compartment.Tests
{
    [TestClass()]
    public class FileRelatedActionParamTests
    {
        [TestMethod()]
        public void CompareActionParamTest()
        {
            FileRelatedActionParam fileRelatedActionParam = new FileRelatedActionParam();
            fileRelatedActionParam.FilePath = "12311223_latestOperationProc.json";
            fileRelatedActionParam.UpdateActionParam();

            Assert.IsTrue(fileRelatedActionParam.CompareActionParam("[{\"ActionName\":\"DrawScreenReset\",\"param1\":0,\"param2\":0},{\"ActionName\":\"ViewTriggerImage\",\"param1\":0,\"param2\":0,\"param3\":\"C:\\\\Users\\\\kodama\\\\OneDrive - アトミック株式会社\\\\実験動物中央研究所\\\\add_image.png\"},{\"ActionName\":\"WaitTouchTrigger\",\"param1\":0,\"param2\":0},{\"ActionName\":\"DrawScreenReset\",\"param1\":0,\"param2\":0},{\"ActionName\":\"Delay\",\"param1\":1000,\"param2\":1000},{\"ActionName\":\"ViewCorrectImage\",\"param1\":0,\"param2\":0,\"param3\":\"C:\\\\Users\\\\kodama\\\\OneDrive - アトミック株式会社\\\\実験動物中央研究所\\\\48.jpg\"},{\"ActionName\":\"Delay\",\"param1\":1000,\"param2\":1000},{\"ActionName\":\"DrawScreenReset\",\"param1\":0,\"param2\":0},{\"ActionName\":\"Delay\",\"param1\":1000,\"param2\":1000},{\"ActionName\":\"ViewCorrectWrongImage\",\"param1\":0,\"param2\":0},{\"ActionName\":\"WaitCorrectTouchTrigger\",\"param1\":0,\"param2\":0},{\"ActionName\":\"DrawScreenReset\",\"param1\":0,\"param2\":0},{\"ActionName\":\"PlaySound\",\"param1\":0,\"param2\":0,\"param3\":\"C:\\\\Windows\\\\Media\\\\Alarm01.wav\"},{\"ActionName\":\"FeedSound\",\"param1\":1000,\"param2\":1000,\"param3\":\"C:\\\\Windows\\\\Media\\\\notify.wav\"},{\"ActionName\":\"OutputResult\",\"param1\":0,\"param2\":0},{\"ActionName\":\"TouchDelay\",\"param1\":1000,\"param2\":1000}]"));
        }

        [TestMethod()]
        public void UpdateActionParamTest()
        {
            FileRelatedActionParam fileRelatedActionParam = new FileRelatedActionParam();
            fileRelatedActionParam.FilePath = "12311223_latestOperationProc.json";
            fileRelatedActionParam.UpdateActionParam();
            Assert.AreEqual("[{\"ActionName\":\"DrawScreenReset\",\"param1\":0,\"param2\":0},{\"ActionName\":\"ViewTriggerImage\",\"param1\":0,\"param2\":0,\"param3\":\"C:\\\\Users\\\\kodama\\\\OneDrive - アトミック株式会社\\\\実験動物中央研究所\\\\add_image.png\"},{\"ActionName\":\"WaitTouchTrigger\",\"param1\":0,\"param2\":0},{\"ActionName\":\"DrawScreenReset\",\"param1\":0,\"param2\":0},{\"ActionName\":\"Delay\",\"param1\":1000,\"param2\":1000},{\"ActionName\":\"ViewCorrectImage\",\"param1\":0,\"param2\":0,\"param3\":\"C:\\\\Users\\\\kodama\\\\OneDrive - アトミック株式会社\\\\実験動物中央研究所\\\\48.jpg\"},{\"ActionName\":\"Delay\",\"param1\":1000,\"param2\":1000},{\"ActionName\":\"DrawScreenReset\",\"param1\":0,\"param2\":0},{\"ActionName\":\"Delay\",\"param1\":1000,\"param2\":1000},{\"ActionName\":\"ViewCorrectWrongImage\",\"param1\":0,\"param2\":0},{\"ActionName\":\"WaitCorrectTouchTrigger\",\"param1\":0,\"param2\":0},{\"ActionName\":\"DrawScreenReset\",\"param1\":0,\"param2\":0},{\"ActionName\":\"PlaySound\",\"param1\":0,\"param2\":0,\"param3\":\"C:\\\\Windows\\\\Media\\\\Alarm01.wav\"},{\"ActionName\":\"FeedSound\",\"param1\":1000,\"param2\":1000,\"param3\":\"C:\\\\Windows\\\\Media\\\\notify.wav\"},{\"ActionName\":\"OutputResult\",\"param1\":0,\"param2\":0},{\"ActionName\":\"TouchDelay\",\"param1\":1000,\"param2\":1000}]", fileRelatedActionParam.ActionParams);
        }

        [TestMethod()]
        public void GetActionParamsTest()
        {
            FileRelatedActionParam fileRelatedActionParam = new FileRelatedActionParam();
            fileRelatedActionParam.FilePath = "12311223_latestOperationProc.json";
            fileRelatedActionParam.UpdateActionParam();
            UcOperationBlock.ActionParam[] actionParam = fileRelatedActionParam.GetActionParams();
            Assert.AreEqual("DrawScreenReset", actionParam[0].ActionName);
            Assert.AreEqual(0, actionParam[0].param1);
            Assert.AreEqual(0, actionParam[0].param2);
            Assert.AreEqual("ViewTriggerImage", actionParam[1].ActionName);
            Assert.AreEqual(0, actionParam[1].param1);
            Assert.AreEqual(0, actionParam[1].param2);
            Assert.AreEqual("C:\\Users\\kodama\\OneDrive - アトミック株式会社\\実験動物中央研究所\\add_image.png", actionParam[1].param3);
            Assert.AreEqual("WaitTouchTrigger", actionParam[2].ActionName);
            Assert.AreEqual("DrawScreenReset", actionParam[3].ActionName);
            Assert.AreEqual("Delay", actionParam[4].ActionName);
            Assert.AreEqual("ViewCorrectImage", actionParam[5].ActionName);
            Assert.AreEqual("Delay", actionParam[6].ActionName);
            Assert.AreEqual("DrawScreenReset", actionParam[7].ActionName);
            Assert.AreEqual("Delay", actionParam[8].ActionName);
            Assert.AreEqual("ViewCorrectWrongImage", actionParam[9].ActionName);
            Assert.AreEqual("WaitCorrectTouchTrigger", actionParam[10].ActionName);
            Assert.AreEqual("DrawScreenReset", actionParam[11].ActionName);
            Assert.AreEqual("PlaySound", actionParam[12].ActionName);
            Assert.AreEqual("FeedSound", actionParam[13].ActionName);
            Assert.AreEqual("OutputResult", actionParam[14].ActionName);
            Assert.AreEqual("TouchDelay", actionParam[15].ActionName);

        }

        [TestMethod()]
        public void CompareFileActionParamTest()
        {
            FileRelatedActionParam fileRelatedActionParam = new FileRelatedActionParam();
            fileRelatedActionParam.FilePath = "12311223_latestOperationProc.json";
            fileRelatedActionParam.UpdateActionParam();
            Assert.IsTrue(fileRelatedActionParam.CompareFileActionParam());
        }

        [TestMethod()]
        public void SaveToJsonTest()
        {
            FileRelatedActionParam fileRelatedActionParam = new FileRelatedActionParam();
            fileRelatedActionParam.FilePath = "12311223_latestOperationProc.json";
            fileRelatedActionParam.UpdateActionParam();
            var oldActonParam = fileRelatedActionParam.ActionParams;
            fileRelatedActionParam.SaveToJson();
            fileRelatedActionParam.UpdateActionParam();
            Assert.AreEqual(oldActonParam, fileRelatedActionParam.ActionParams);
        }
    }
}
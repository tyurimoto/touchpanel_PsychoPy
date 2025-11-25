using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BlockProgramming;

namespace WPFonForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            userControl11.SendJsonEventHandler += (object sender, EventArgs _) => { System.Diagnostics.Debug.WriteLine(sender); };
        }

        private void button2_Click(object sender, EventArgs e)
        {

            var json =
                @"[
                    {
                        ""ActionName"": ""DrawScreenReset""
                    },
                    {
                        ""ActionName"": ""WaitTouchTrigger"",
                        ""Param1"": 1000
                    },
                    {
                        ""ActionName"": ""Delay"",
                        ""Param1"": 3000,
                        ""Param2"": 4000
                    },
                    {
                        ""ActionName"": ""PathTest"",
                        ""Param1"": 0,
                        ""Param2"": 0,
                        ""Param3"": ""C:\\""
                    }
                ]";

            userControl11.JsonLoad(json);
        }
    }
}

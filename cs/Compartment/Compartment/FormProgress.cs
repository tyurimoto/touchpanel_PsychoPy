using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compartment
{
    public partial class FormProgress : Form
    {
        public FormProgress()
        {
            InitializeComponent();
            //timerProgress.Enabled = false;
        }

        private void FormProgress_Load(object sender, EventArgs e)
        {
            Text = "・・・";
        }

        private void timerProgress_Tick(object sender, EventArgs e)
        {
            //if (progressBarWaiting.Value < progressBarWaiting.Maximum)
            //{
            //    progressBarWaiting.PerformStep();
            //}
            //else
            //{
            //    progressBarWaiting.Value = 0;
            //}
            //Refresh();
        }

        private void FormProgress_Activated(object sender, EventArgs e)
        {
            //timerProgress.Enabled = true;
        }
    }
}

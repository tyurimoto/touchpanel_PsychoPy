using System;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Reflection;

namespace Compartment
{
    public partial class FormSelectEngine : Form
    {
        public FormSelectEngine()
        {
            string subVersion = "c";

            InitializeComponent();
            radioButtonBlockEngine.Checked = true;

            // 対応ケージ判別表示
            labelVersion.Text = "A-Cage Version with eDoor&&CAM Multi ID";
            labelVersion.ForeColor = Color.Red;
            // 位置ラベル長に合わせて自動
            labelVersion.Location = new System.Drawing.Point(530 - labelVersion.Size.Width, labelVersion.Location.Y);

            labelAssemblyVersion.Text = "V" + Assembly.GetExecutingAssembly().GetName().Version.ToString() + subVersion;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButtonBlockEngine.Checked)
            {
                Program.EnableNewEngine = true;
            }
            else
            {
                Program.EnableNewEngine = false;
            }
            Close();
        }
    }
}

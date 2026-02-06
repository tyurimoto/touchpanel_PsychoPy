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
            radioButtonPsychoPy.Checked = true;  // デフォルトをPsychoPyに変更

            // 対応ケージ判別表示
            labelVersion.Text = "A-Cage Version with eDoor&&CAM Multi ID";
            labelVersion.ForeColor = Color.Red;
            // 位置ラベル長に合わせて自動
            labelVersion.Location = new System.Drawing.Point(530 - labelVersion.Size.Width, labelVersion.Location.Y);

            labelAssemblyVersion.Text = "V" + Assembly.GetExecutingAssembly().GetName().Version.ToString() + subVersion;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButtonPsychoPy.Checked)
            {
                Program.SelectedEngine = Program.EEngineType.PsychoPy;
            }
            else if (radioButtonBlockEngine.Checked)
            {
                Program.SelectedEngine = Program.EEngineType.BlockProgramming;
            }
            else // radioButton1 (Old Engine)
            {
                Program.SelectedEngine = Program.EEngineType.OldEngine;
            }
            Close();
        }
    }
}

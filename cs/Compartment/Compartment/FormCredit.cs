using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compartment
{
    public partial class FormCredit : Form
    {
        public FormCredit()
        {
            InitializeComponent();
        }

        private void FormCredit_Load(object sender, EventArgs e)
        {
            textBoxLicenses.Text = string.Empty;
            string licenseFilePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "license.txt");
            if(File.Exists(licenseFilePath))
            {
                textBoxLicenses.Text=File.ReadAllText(licenseFilePath);
            }
        }
    }
}

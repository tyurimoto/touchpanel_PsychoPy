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
    public partial class SelectFileList : UserControl
    {
        OpenFileDialog ofd =new OpenFileDialog();
        private string _fileName;

        [Category("Data")]
        [Description("ファイルパスおよびファイルネーム")]
        [DefaultValue(typeof(string), "")]
        [Browsable(true)]
        public string FileName
        {
            set
            {
                _fileName = value;
                textBoxFileName.Text = _fileName;
            }
            get => _fileName;
        }
        //[AmbientValue(typeof(Color), "Empty")]
        //[Category("Appearance")]
        [Category("Action")]
        [Description("初期拡張子を指定します。")]
        [DefaultValue(typeof(string), "")]
        [Browsable(true)]
        public string DefaultExt
        {
            get => ofd.DefaultExt;
            set => ofd.DefaultExt = value;
        }
        [Category("Action")]
        [DefaultValue(typeof(bool), "true")]
        [Browsable(true)]
        public bool AddExtension
        {
            get => ofd.AddExtension;
            set => ofd.AddExtension = value;
        }
        [Category("Action")]
        [DefaultValue(typeof(bool), "true")]
        [Browsable(true)]
        public bool CheckFileExists
        {
            get => ofd.CheckFileExists;
            set => ofd.CheckFileExists = value;
        }
        [Category("Action")]
        [DefaultValue(typeof(bool), "true")]
        [Browsable(true)]
        public bool CheckPathExists
        {
            get => ofd.CheckPathExists;
            set => ofd.CheckPathExists = value;
        }
        [Category("Action")]
        [DefaultValue(typeof(string), "")]
        [Browsable(true)]
        public string Filter
        {
            get => ofd.Filter;
            set => ofd.Filter = value;
        }
        [Category("Action")]
        [DefaultValue(typeof(bool), "true")]
        [Browsable(true)]
        public bool Multiselect
        {
            get => ofd.Multiselect;
            set => ofd.Multiselect = value;
        }

        public SelectFileList()
        {
            InitializeComponent();
            buttonFileSelect.Height = textBoxFileName.Height - buttonFileSelect.Margin.Top;

        }

        private void buttonFileSelect_Click(object sender, EventArgs e)
        {
            ofd.ShowDialog();
            FileName = ofd.FileName;
            textBoxFileName.Text = FileName;
        }
    }
}

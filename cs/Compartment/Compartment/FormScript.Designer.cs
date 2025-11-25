
namespace Compartment
{
    partial class FormScript
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.buttonScriptToJson = new System.Windows.Forms.Button();
            this.button登録 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.buttonOpenTips = new System.Windows.Forms.Button();
            this.saveScriptFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openScriptFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonOpen = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.userControl11 = new BlockProgramming.UserControl1();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.textBoxScript = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.idSelectGroupBox = new System.Windows.Forms.GroupBox();
            this.idsearchTextBox = new System.Windows.Forms.TextBox();
            this.comboBoxIds = new System.Windows.Forms.ComboBox();
            this.idAddDeleteGroupBox = new System.Windows.Forms.GroupBox();
            this.comboBoxAddIDs = new System.Windows.Forms.ComboBox();
            this.idSetTextBox = new System.Windows.Forms.TextBox();
            this.idAddButton = new System.Windows.Forms.Button();
            this.idDeleteButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxIdFileName = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.idSelectGroupBox.SuspendLayout();
            this.idAddDeleteGroupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonScriptToJson
            // 
            this.buttonScriptToJson.Location = new System.Drawing.Point(351, 541);
            this.buttonScriptToJson.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonScriptToJson.Name = "buttonScriptToJson";
            this.buttonScriptToJson.Size = new System.Drawing.Size(190, 91);
            this.buttonScriptToJson.TabIndex = 0;
            this.buttonScriptToJson.Text = "アクション登録";
            this.buttonScriptToJson.UseVisualStyleBackColor = true;
            this.buttonScriptToJson.Visible = false;
            this.buttonScriptToJson.Click += new System.EventHandler(this.buttonScriptToJSON_Click);
            // 
            // button登録
            // 
            this.button登録.Location = new System.Drawing.Point(12, 592);
            this.button登録.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button登録.Name = "button登録";
            this.button登録.Size = new System.Drawing.Size(160, 40);
            this.button登録.TabIndex = 0;
            this.button登録.Text = "登録";
            this.button登録.UseVisualStyleBackColor = true;
            this.button登録.Click += new System.EventHandler(this.buttonScriptToJSON_Click);
            // 
            // button4
            // 
            this.button4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button4.Font = new System.Drawing.Font("游ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button4.Location = new System.Drawing.Point(4, 55);
            this.button4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(186, 40);
            this.button4.TabIndex = 0;
            this.button4.Text = "Register func";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Visible = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button5.Font = new System.Drawing.Font("游ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button5.Location = new System.Drawing.Point(4, 5);
            this.button5.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(186, 40);
            this.button5.TabIndex = 2;
            this.button5.Text = "&Close";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(12, 541);
            this.button6.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(160, 40);
            this.button6.TabIndex = 3;
            this.button6.Text = "SampleJSON";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.TestJsonButton_Click);
            // 
            // buttonOpenTips
            // 
            this.buttonOpenTips.Location = new System.Drawing.Point(182, 541);
            this.buttonOpenTips.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonOpenTips.Name = "buttonOpenTips";
            this.buttonOpenTips.Size = new System.Drawing.Size(160, 40);
            this.buttonOpenTips.TabIndex = 4;
            this.buttonOpenTips.Text = "Tips";
            this.buttonOpenTips.UseVisualStyleBackColor = true;
            this.buttonOpenTips.Click += new System.EventHandler(this.buttonOpenTips_Click);
            // 
            // saveScriptFileDialog
            // 
            this.saveScriptFileDialog.DefaultExt = "json";
            this.saveScriptFileDialog.Filter = "json|*.json|すべてのファイル|*.*";
            // 
            // openScriptFileDialog
            // 
            this.openScriptFileDialog.FileName = "openFileDialog1";
            this.openScriptFileDialog.Filter = "json|*.json|すべてのファイル|*.*";
            // 
            // buttonSave
            // 
            this.buttonSave.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonSave.Font = new System.Drawing.Font("游ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonSave.Location = new System.Drawing.Point(4, 155);
            this.buttonSave.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(186, 40);
            this.buttonSave.TabIndex = 5;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonOpen
            // 
            this.buttonOpen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonOpen.Font = new System.Drawing.Font("游ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonOpen.Location = new System.Drawing.Point(4, 105);
            this.buttonOpen.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(186, 40);
            this.buttonOpen.TabIndex = 5;
            this.buttonOpen.Text = "Open";
            this.buttonOpen.UseVisualStyleBackColor = true;
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.flowLayoutPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(1249, 788);
            this.splitContainer1.SplitterDistance = 1040;
            this.splitContainer1.SplitterWidth = 6;
            this.splitContainer1.TabIndex = 7;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1040, 788);
            this.tabControl1.TabIndex = 8;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.elementHost1);
            this.tabPage2.Location = new System.Drawing.Point(4, 30);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage2.Size = new System.Drawing.Size(1032, 754);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "ブロック";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // elementHost1
            // 
            this.elementHost1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elementHost1.Location = new System.Drawing.Point(4, 5);
            this.elementHost1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(1024, 744);
            this.elementHost1.TabIndex = 7;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = this.userControl11;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.button6);
            this.tabPage1.Controls.Add(this.buttonOpenTips);
            this.tabPage1.Controls.Add(this.buttonScriptToJson);
            this.tabPage1.Controls.Add(this.button登録);
            this.tabPage1.Controls.Add(this.textBoxScript);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage1.Size = new System.Drawing.Size(1032, 762);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "スクリプト登録";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // textBoxScript
            // 
            this.textBoxScript.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBoxScript.Location = new System.Drawing.Point(4, 5);
            this.textBoxScript.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxScript.Multiline = true;
            this.textBoxScript.Name = "textBoxScript";
            this.textBoxScript.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxScript.Size = new System.Drawing.Size(1024, 426);
            this.textBoxScript.TabIndex = 2;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.button5);
            this.flowLayoutPanel1.Controls.Add(this.button4);
            this.flowLayoutPanel1.Controls.Add(this.buttonOpen);
            this.flowLayoutPanel1.Controls.Add(this.buttonSave);
            this.flowLayoutPanel1.Controls.Add(this.idSelectGroupBox);
            this.flowLayoutPanel1.Controls.Add(this.idAddDeleteGroupBox);
            this.flowLayoutPanel1.Controls.Add(this.groupBox1);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(203, 788);
            this.flowLayoutPanel1.TabIndex = 6;
            this.flowLayoutPanel1.SizeChanged += new System.EventHandler(this.flowLayoutPanel1_SizeChanged);
            // 
            // idSelectGroupBox
            // 
            this.idSelectGroupBox.Controls.Add(this.idsearchTextBox);
            this.idSelectGroupBox.Controls.Add(this.comboBoxIds);
            this.idSelectGroupBox.Location = new System.Drawing.Point(3, 203);
            this.idSelectGroupBox.Name = "idSelectGroupBox";
            this.idSelectGroupBox.Size = new System.Drawing.Size(188, 100);
            this.idSelectGroupBox.TabIndex = 13;
            this.idSelectGroupBox.TabStop = false;
            this.idSelectGroupBox.Text = "ID選択・検索";
            // 
            // idsearchTextBox
            // 
            this.idsearchTextBox.Location = new System.Drawing.Point(7, 59);
            this.idsearchTextBox.MaxLength = 38;
            this.idsearchTextBox.Name = "idsearchTextBox";
            this.idsearchTextBox.Size = new System.Drawing.Size(175, 33);
            this.idsearchTextBox.TabIndex = 7;
            this.idsearchTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchIDTextBox_KeyDown);
            // 
            // comboBoxIds
            // 
            this.comboBoxIds.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxIds.FormattingEnabled = true;
            this.comboBoxIds.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.comboBoxIds.Items.AddRange(new object[] {
            "0(default)"});
            this.comboBoxIds.Location = new System.Drawing.Point(6, 23);
            this.comboBoxIds.Name = "comboBoxIds";
            this.comboBoxIds.Size = new System.Drawing.Size(176, 29);
            this.comboBoxIds.TabIndex = 6;
            this.comboBoxIds.SelectedIndexChanged += new System.EventHandler(this.ComboBoxIds_SelectedIndexChanged);
            // 
            // idAddDeleteGroupBox
            // 
            this.idAddDeleteGroupBox.Controls.Add(this.comboBoxAddIDs);
            this.idAddDeleteGroupBox.Controls.Add(this.idSetTextBox);
            this.idAddDeleteGroupBox.Controls.Add(this.idAddButton);
            this.idAddDeleteGroupBox.Controls.Add(this.idDeleteButton);
            this.idAddDeleteGroupBox.Location = new System.Drawing.Point(3, 309);
            this.idAddDeleteGroupBox.Name = "idAddDeleteGroupBox";
            this.idAddDeleteGroupBox.Size = new System.Drawing.Size(188, 178);
            this.idAddDeleteGroupBox.TabIndex = 12;
            this.idAddDeleteGroupBox.TabStop = false;
            this.idAddDeleteGroupBox.Text = "ID追加・削除";
            // 
            // comboBoxAddIDs
            // 
            this.comboBoxAddIDs.FormattingEnabled = true;
            this.comboBoxAddIDs.Location = new System.Drawing.Point(7, 62);
            this.comboBoxAddIDs.Name = "comboBoxAddIDs";
            this.comboBoxAddIDs.Size = new System.Drawing.Size(175, 29);
            this.comboBoxAddIDs.TabIndex = 11;
            this.comboBoxAddIDs.Enter += new System.EventHandler(this.comboBoxAddIDs_Enter);
            this.comboBoxAddIDs.Leave += new System.EventHandler(this.comboBoxAddIDs_Leave);
            // 
            // idSetTextBox
            // 
            this.idSetTextBox.Location = new System.Drawing.Point(7, 23);
            this.idSetTextBox.MaxLength = 38;
            this.idSetTextBox.Name = "idSetTextBox";
            this.idSetTextBox.Size = new System.Drawing.Size(175, 33);
            this.idSetTextBox.TabIndex = 8;
            this.idSetTextBox.Enter += new System.EventHandler(this.idSetTextBox_Enter);
            this.idSetTextBox.Leave += new System.EventHandler(this.idSetTextBox_Leave);
            // 
            // idAddButton
            // 
            this.idAddButton.Location = new System.Drawing.Point(7, 104);
            this.idAddButton.Name = "idAddButton";
            this.idAddButton.Size = new System.Drawing.Size(75, 34);
            this.idAddButton.TabIndex = 9;
            this.idAddButton.Text = "追加";
            this.idAddButton.UseVisualStyleBackColor = true;
            this.idAddButton.Click += new System.EventHandler(this.AddIDButton_Click);
            // 
            // idDeleteButton
            // 
            this.idDeleteButton.Location = new System.Drawing.Point(75, 104);
            this.idDeleteButton.Name = "idDeleteButton";
            this.idDeleteButton.Size = new System.Drawing.Size(75, 33);
            this.idDeleteButton.TabIndex = 10;
            this.idDeleteButton.Text = "削除";
            this.idDeleteButton.UseVisualStyleBackColor = true;
            this.idDeleteButton.Click += new System.EventHandler(this.DeleteIDButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxIdFileName);
            this.groupBox1.Location = new System.Drawing.Point(3, 493);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(188, 100);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ファイル名";
            // 
            // textBoxIdFileName
            // 
            this.textBoxIdFileName.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBoxIdFileName.Location = new System.Drawing.Point(3, 29);
            this.textBoxIdFileName.Name = "textBoxIdFileName";
            this.textBoxIdFileName.ReadOnly = true;
            this.textBoxIdFileName.Size = new System.Drawing.Size(182, 33);
            this.textBoxIdFileName.TabIndex = 0;
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 500;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ReshowDelay = 100;
            // 
            // FormScript
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1249, 788);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("游ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "FormScript";
            this.Text = "Script用 登録ウィンドウ";
            this.Activated += new System.EventHandler(this.FormScript_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormScript_FormClosing);
            this.Load += new System.EventHandler(this.FormScript_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.idSelectGroupBox.ResumeLayout(false);
            this.idSelectGroupBox.PerformLayout();
            this.idAddDeleteGroupBox.ResumeLayout(false);
            this.idAddDeleteGroupBox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button buttonScriptToJson;
        private System.Windows.Forms.Button button登録;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button buttonOpenTips;
        private System.Windows.Forms.SaveFileDialog saveScriptFileDialog;
        private System.Windows.Forms.OpenFileDialog openScriptFileDialog;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonOpen;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox textBoxScript;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private BlockProgramming.UserControl1 userControl11;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.ComboBox comboBoxIds;
        private System.Windows.Forms.TextBox idSetTextBox;
        private System.Windows.Forms.Button idAddButton;
        private System.Windows.Forms.Button idDeleteButton;
        private System.Windows.Forms.GroupBox idSelectGroupBox;
        private System.Windows.Forms.GroupBox idAddDeleteGroupBox;
        private System.Windows.Forms.TextBox idsearchTextBox;
        private System.Windows.Forms.ComboBox comboBoxAddIDs;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxIdFileName;
    }
}
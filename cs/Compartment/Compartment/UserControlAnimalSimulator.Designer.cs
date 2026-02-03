namespace Compartment
{
    partial class UserControlAnimalSimulator
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.labelTitle = new System.Windows.Forms.Label();
            this.groupBoxAnimalPosition = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanelPosition = new System.Windows.Forms.TableLayoutPanel();
            this.radioButtonOutside = new System.Windows.Forms.RadioButton();
            this.radioButtonEntering = new System.Windows.Forms.RadioButton();
            this.radioButtonInside = new System.Windows.Forms.RadioButton();
            this.radioButtonExiting = new System.Windows.Forms.RadioButton();
            this.radioButtonAtLever = new System.Windows.Forms.RadioButton();
            this.checkBoxPressingLever = new System.Windows.Forms.CheckBox();
            this.buttonApply = new System.Windows.Forms.Button();
            this.labelStatus = new System.Windows.Forms.Label();
            this.tableLayoutPanelMain.SuspendLayout();
            this.groupBoxAnimalPosition.SuspendLayout();
            this.tableLayoutPanelPosition.SuspendLayout();
            this.SuspendLayout();
            //
            // tableLayoutPanelMain
            //
            this.tableLayoutPanelMain.ColumnCount = 2;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanelMain.Controls.Add(this.labelTitle, 0, 0);
            this.tableLayoutPanelMain.Controls.Add(this.groupBoxAnimalPosition, 0, 1);
            this.tableLayoutPanelMain.Controls.Add(this.buttonApply, 1, 1);
            this.tableLayoutPanelMain.Controls.Add(this.labelStatus, 0, 2);
            this.tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 3;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(600, 150);
            this.tableLayoutPanelMain.TabIndex = 0;
            //
            // labelTitle
            //
            this.labelTitle.AutoSize = true;
            this.tableLayoutPanelMain.SetColumnSpan(this.labelTitle, 2);
            this.labelTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTitle.Font = new System.Drawing.Font("MS UI Gothic", 10F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(3, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(594, 30);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "動物位置シミュレーター";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // groupBoxAnimalPosition
            //
            this.groupBoxAnimalPosition.Controls.Add(this.tableLayoutPanelPosition);
            this.groupBoxAnimalPosition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxAnimalPosition.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.groupBoxAnimalPosition.Location = new System.Drawing.Point(3, 33);
            this.groupBoxAnimalPosition.Name = "groupBoxAnimalPosition";
            this.groupBoxAnimalPosition.Size = new System.Drawing.Size(414, 89);
            this.groupBoxAnimalPosition.TabIndex = 1;
            this.groupBoxAnimalPosition.TabStop = false;
            this.groupBoxAnimalPosition.Text = "動物の位置";
            //
            // tableLayoutPanelPosition
            //
            this.tableLayoutPanelPosition.ColumnCount = 3;
            this.tableLayoutPanelPosition.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tableLayoutPanelPosition.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tableLayoutPanelPosition.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.34F));
            this.tableLayoutPanelPosition.Controls.Add(this.radioButtonOutside, 0, 0);
            this.tableLayoutPanelPosition.Controls.Add(this.radioButtonEntering, 1, 0);
            this.tableLayoutPanelPosition.Controls.Add(this.radioButtonInside, 2, 0);
            this.tableLayoutPanelPosition.Controls.Add(this.radioButtonExiting, 0, 1);
            this.tableLayoutPanelPosition.Controls.Add(this.radioButtonAtLever, 1, 1);
            this.tableLayoutPanelPosition.Controls.Add(this.checkBoxPressingLever, 2, 1);
            this.tableLayoutPanelPosition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelPosition.Location = new System.Drawing.Point(3, 15);
            this.tableLayoutPanelPosition.Name = "tableLayoutPanelPosition";
            this.tableLayoutPanelPosition.RowCount = 2;
            this.tableLayoutPanelPosition.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelPosition.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelPosition.Size = new System.Drawing.Size(408, 71);
            this.tableLayoutPanelPosition.TabIndex = 0;
            //
            // radioButtonOutside
            //
            this.radioButtonOutside.AutoSize = true;
            this.radioButtonOutside.Checked = true;
            this.radioButtonOutside.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButtonOutside.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.radioButtonOutside.Location = new System.Drawing.Point(3, 3);
            this.radioButtonOutside.Name = "radioButtonOutside";
            this.radioButtonOutside.Size = new System.Drawing.Size(129, 29);
            this.radioButtonOutside.TabIndex = 0;
            this.radioButtonOutside.TabStop = true;
            this.radioButtonOutside.Text = "外（部屋の外）";
            this.radioButtonOutside.UseVisualStyleBackColor = true;
            //
            // radioButtonEntering
            //
            this.radioButtonEntering.AutoSize = true;
            this.radioButtonEntering.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButtonEntering.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.radioButtonEntering.Location = new System.Drawing.Point(138, 3);
            this.radioButtonEntering.Name = "radioButtonEntering";
            this.radioButtonEntering.Size = new System.Drawing.Size(129, 29);
            this.radioButtonEntering.TabIndex = 1;
            this.radioButtonEntering.Text = "入室中";
            this.radioButtonEntering.UseVisualStyleBackColor = true;
            //
            // radioButtonInside
            //
            this.radioButtonInside.AutoSize = true;
            this.radioButtonInside.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButtonInside.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.radioButtonInside.Location = new System.Drawing.Point(273, 3);
            this.radioButtonInside.Name = "radioButtonInside";
            this.radioButtonInside.Size = new System.Drawing.Size(132, 29);
            this.radioButtonInside.TabIndex = 2;
            this.radioButtonInside.Text = "在室中";
            this.radioButtonInside.UseVisualStyleBackColor = true;
            //
            // radioButtonExiting
            //
            this.radioButtonExiting.AutoSize = true;
            this.radioButtonExiting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButtonExiting.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.radioButtonExiting.Location = new System.Drawing.Point(3, 38);
            this.radioButtonExiting.Name = "radioButtonExiting";
            this.radioButtonExiting.Size = new System.Drawing.Size(129, 30);
            this.radioButtonExiting.TabIndex = 3;
            this.radioButtonExiting.Text = "退室中";
            this.radioButtonExiting.UseVisualStyleBackColor = true;
            //
            // radioButtonAtLever
            //
            this.radioButtonAtLever.AutoSize = true;
            this.radioButtonAtLever.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButtonAtLever.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.radioButtonAtLever.Location = new System.Drawing.Point(138, 38);
            this.radioButtonAtLever.Name = "radioButtonAtLever";
            this.radioButtonAtLever.Size = new System.Drawing.Size(129, 30);
            this.radioButtonAtLever.TabIndex = 4;
            this.radioButtonAtLever.Text = "レバー付近";
            this.radioButtonAtLever.UseVisualStyleBackColor = true;
            //
            // checkBoxPressingLever
            //
            this.checkBoxPressingLever.AutoSize = true;
            this.checkBoxPressingLever.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkBoxPressingLever.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.checkBoxPressingLever.Location = new System.Drawing.Point(273, 38);
            this.checkBoxPressingLever.Name = "checkBoxPressingLever";
            this.checkBoxPressingLever.Size = new System.Drawing.Size(132, 30);
            this.checkBoxPressingLever.TabIndex = 5;
            this.checkBoxPressingLever.Text = "レバー押下中";
            this.checkBoxPressingLever.UseVisualStyleBackColor = true;
            //
            // buttonApply
            //
            this.buttonApply.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonApply.Font = new System.Drawing.Font("MS UI Gothic", 10F, System.Drawing.FontStyle.Bold);
            this.buttonApply.Location = new System.Drawing.Point(423, 33);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(174, 89);
            this.buttonApply.TabIndex = 2;
            this.buttonApply.Text = "適用";
            this.buttonApply.UseVisualStyleBackColor = true;
            //
            // labelStatus
            //
            this.labelStatus.AutoSize = true;
            this.tableLayoutPanelMain.SetColumnSpan(this.labelStatus, 2);
            this.labelStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelStatus.Font = new System.Drawing.Font("MS UI Gothic", 8F);
            this.labelStatus.ForeColor = System.Drawing.Color.Blue;
            this.labelStatus.Location = new System.Drawing.Point(3, 125);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(594, 25);
            this.labelStatus.TabIndex = 3;
            this.labelStatus.Text = "状態: 外（部屋の外）";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // UserControlAnimalSimulator
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightYellow;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.tableLayoutPanelMain);
            this.Name = "UserControlAnimalSimulator";
            this.Size = new System.Drawing.Size(600, 150);
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.tableLayoutPanelMain.PerformLayout();
            this.groupBoxAnimalPosition.ResumeLayout(false);
            this.tableLayoutPanelPosition.ResumeLayout(false);
            this.tableLayoutPanelPosition.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.GroupBox groupBoxAnimalPosition;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelPosition;
        private System.Windows.Forms.RadioButton radioButtonOutside;
        private System.Windows.Forms.RadioButton radioButtonEntering;
        private System.Windows.Forms.RadioButton radioButtonInside;
        private System.Windows.Forms.RadioButton radioButtonExiting;
        private System.Windows.Forms.RadioButton radioButtonAtLever;
        private System.Windows.Forms.CheckBox checkBoxPressingLever;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.Label labelStatus;
    }
}

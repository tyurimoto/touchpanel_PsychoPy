
namespace Compartment
{
	partial class UserControlMain
	{
		/// <summary> 
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region コンポーネント デザイナーで生成されたコード

		/// <summary> 
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
            this.buttonOperationOnUserControlMain = new System.Windows.Forms.Button();
            this.buttonCheckDeviceOnUserControlMain = new System.Windows.Forms.Button();
            this.buttonPreferencesOnUserControlMain = new System.Windows.Forms.Button();
            this.buttonEndOnUserControlMain = new System.Windows.Forms.Button();
            this.buttonCheckIoOnUserControlMain = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonBlockProgramming = new System.Windows.Forms.Button();
            this.checkBoxEnableDebugMode = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOperationOnUserControlMain
            // 
            this.buttonOperationOnUserControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonOperationOnUserControlMain.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonOperationOnUserControlMain.Font = new System.Drawing.Font("MS UI Gothic", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonOperationOnUserControlMain.Location = new System.Drawing.Point(3, 3);
            this.buttonOperationOnUserControlMain.Name = "buttonOperationOnUserControlMain";
            this.buttonOperationOnUserControlMain.Size = new System.Drawing.Size(600, 81);
            this.buttonOperationOnUserControlMain.TabIndex = 0;
            this.buttonOperationOnUserControlMain.Text = "Operation";
            this.buttonOperationOnUserControlMain.UseVisualStyleBackColor = true;
            // 
            // buttonCheckDeviceOnUserControlMain
            // 
            this.buttonCheckDeviceOnUserControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonCheckDeviceOnUserControlMain.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCheckDeviceOnUserControlMain.Font = new System.Drawing.Font("MS UI Gothic", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonCheckDeviceOnUserControlMain.Location = new System.Drawing.Point(3, 90);
            this.buttonCheckDeviceOnUserControlMain.Name = "buttonCheckDeviceOnUserControlMain";
            this.buttonCheckDeviceOnUserControlMain.Size = new System.Drawing.Size(600, 81);
            this.buttonCheckDeviceOnUserControlMain.TabIndex = 1;
            this.buttonCheckDeviceOnUserControlMain.Text = "Check device";
            this.buttonCheckDeviceOnUserControlMain.UseVisualStyleBackColor = true;
            // 
            // buttonPreferencesOnUserControlMain
            // 
            this.buttonPreferencesOnUserControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonPreferencesOnUserControlMain.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPreferencesOnUserControlMain.Font = new System.Drawing.Font("MS UI Gothic", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonPreferencesOnUserControlMain.Location = new System.Drawing.Point(3, 351);
            this.buttonPreferencesOnUserControlMain.Name = "buttonPreferencesOnUserControlMain";
            this.buttonPreferencesOnUserControlMain.Size = new System.Drawing.Size(600, 81);
            this.buttonPreferencesOnUserControlMain.TabIndex = 2;
            this.buttonPreferencesOnUserControlMain.Text = "Preferences";
            this.buttonPreferencesOnUserControlMain.UseVisualStyleBackColor = true;
            // 
            // buttonEndOnUserControlMain
            // 
            this.buttonEndOnUserControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonEndOnUserControlMain.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonEndOnUserControlMain.Font = new System.Drawing.Font("MS UI Gothic", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonEndOnUserControlMain.Location = new System.Drawing.Point(3, 438);
            this.buttonEndOnUserControlMain.Name = "buttonEndOnUserControlMain";
            this.buttonEndOnUserControlMain.Size = new System.Drawing.Size(600, 86);
            this.buttonEndOnUserControlMain.TabIndex = 3;
            this.buttonEndOnUserControlMain.Text = "Close application";
            this.buttonEndOnUserControlMain.UseVisualStyleBackColor = true;
            this.buttonEndOnUserControlMain.Click += new System.EventHandler(this.buttonEndOnUserControlMain_Click);
            // 
            // buttonCheckIoOnUserControlMain
            // 
            this.buttonCheckIoOnUserControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonCheckIoOnUserControlMain.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCheckIoOnUserControlMain.Font = new System.Drawing.Font("MS UI Gothic", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonCheckIoOnUserControlMain.Location = new System.Drawing.Point(3, 177);
            this.buttonCheckIoOnUserControlMain.Name = "buttonCheckIoOnUserControlMain";
            this.buttonCheckIoOnUserControlMain.Size = new System.Drawing.Size(600, 81);
            this.buttonCheckIoOnUserControlMain.TabIndex = 4;
            this.buttonCheckIoOnUserControlMain.Text = "Check IO";
            this.buttonCheckIoOnUserControlMain.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            //
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.buttonOperationOnUserControlMain, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonEndOnUserControlMain, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.buttonCheckIoOnUserControlMain, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.buttonPreferencesOnUserControlMain, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.buttonCheckDeviceOnUserControlMain, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.buttonBlockProgramming, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxEnableDebugMode, 0, 6);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Font = new System.Drawing.Font("MS UI Gothic", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(606, 587);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // buttonBlockProgramming
            //
            this.buttonBlockProgramming.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonBlockProgramming.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonBlockProgramming.Font = new System.Drawing.Font("MS UI Gothic", 48F, System.Drawing.FontStyle.Bold);
            this.buttonBlockProgramming.Location = new System.Drawing.Point(3, 264);
            this.buttonBlockProgramming.Name = "buttonBlockProgramming";
            this.buttonBlockProgramming.Size = new System.Drawing.Size(600, 81);
            this.buttonBlockProgramming.TabIndex = 5;
            this.buttonBlockProgramming.Text = "Block Programming";
            this.buttonBlockProgramming.UseVisualStyleBackColor = true;
            //
            // checkBoxEnableDebugMode
            //
            this.checkBoxEnableDebugMode.AutoSize = true;
            this.checkBoxEnableDebugMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkBoxEnableDebugMode.Font = new System.Drawing.Font("MS UI Gothic", 24F, System.Drawing.FontStyle.Bold);
            this.checkBoxEnableDebugMode.Location = new System.Drawing.Point(3, 530);
            this.checkBoxEnableDebugMode.Name = "checkBoxEnableDebugMode";
            this.checkBoxEnableDebugMode.Size = new System.Drawing.Size(600, 54);
            this.checkBoxEnableDebugMode.TabIndex = 6;
            this.checkBoxEnableDebugMode.Text = "デバッグモードで実行";
            this.checkBoxEnableDebugMode.UseVisualStyleBackColor = true;
            //
            // UserControlMain
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "UserControlMain";
            this.Size = new System.Drawing.Size(606, 587);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		public System.Windows.Forms.Button buttonOperationOnUserControlMain;
		public System.Windows.Forms.Button buttonCheckDeviceOnUserControlMain;
		public System.Windows.Forms.Button buttonPreferencesOnUserControlMain;
		public System.Windows.Forms.Button buttonEndOnUserControlMain;
		public System.Windows.Forms.Button buttonCheckIoOnUserControlMain;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        public System.Windows.Forms.Button buttonBlockProgramming;
        public System.Windows.Forms.CheckBox checkBoxEnableDebugMode;
    }
}

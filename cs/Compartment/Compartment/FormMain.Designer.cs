
namespace Compartment
{
	partial class FormMain
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

		#region Windows フォーム デザイナーで生成されたコード

		/// <summary>
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.panelBase = new System.Windows.Forms.Panel();
            this.userControlInputComOnFormMain = new Compartment.UserControlInputCom();
            this.userControlCheckIoOnFormMain = new Compartment.UserControlCheckIo();
            this.userControlPreferencesTabOnFormMain = new Compartment.UserControlPreferencesTab();
            this.userControlOperationOnFormMain = new Compartment.UserControlOperation();
            this.userControlMainOnFormMain = new Compartment.UserControlMain();
            this.userControlCheckDeviceOnFormMain = new Compartment.UserControlCheckDevice();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.timerCheckIo = new System.Windows.Forms.Timer(this.components);
            this.timerOperation = new System.Windows.Forms.Timer(this.components);
            this.timerOperationStateMachine = new System.Windows.Forms.Timer(this.components);
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.panelBase.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelBase
            // 
            this.panelBase.Controls.Add(this.userControlInputComOnFormMain);
            this.panelBase.Controls.Add(this.userControlCheckIoOnFormMain);
            this.panelBase.Controls.Add(this.userControlPreferencesTabOnFormMain);
            this.panelBase.Controls.Add(this.userControlOperationOnFormMain);
            this.panelBase.Controls.Add(this.userControlMainOnFormMain);
            this.panelBase.Controls.Add(this.userControlCheckDeviceOnFormMain);
            this.panelBase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBase.Location = new System.Drawing.Point(0, 0);
            this.panelBase.Margin = new System.Windows.Forms.Padding(4);
            this.panelBase.Name = "panelBase";
            this.panelBase.Size = new System.Drawing.Size(293, 213);
            this.panelBase.TabIndex = 0;
            // 
            // userControlInputComOnFormMain
            // 
            this.userControlInputComOnFormMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userControlInputComOnFormMain.Location = new System.Drawing.Point(0, 0);
            this.userControlInputComOnFormMain.Margin = new System.Windows.Forms.Padding(4);
            this.userControlInputComOnFormMain.Name = "userControlInputComOnFormMain";
            this.userControlInputComOnFormMain.Size = new System.Drawing.Size(293, 213);
            this.userControlInputComOnFormMain.TabIndex = 6;
            // 
            // userControlCheckIoOnFormMain
            // 
            this.userControlCheckIoOnFormMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userControlCheckIoOnFormMain.Location = new System.Drawing.Point(0, 0);
            this.userControlCheckIoOnFormMain.Margin = new System.Windows.Forms.Padding(4);
            this.userControlCheckIoOnFormMain.Name = "userControlCheckIoOnFormMain";
            this.userControlCheckIoOnFormMain.Size = new System.Drawing.Size(293, 213);
            this.userControlCheckIoOnFormMain.TabIndex = 5;
            // 
            // userControlPreferencesTabOnFormMain
            // 
            this.userControlPreferencesTabOnFormMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userControlPreferencesTabOnFormMain.Location = new System.Drawing.Point(0, 0);
            this.userControlPreferencesTabOnFormMain.Margin = new System.Windows.Forms.Padding(4);
            this.userControlPreferencesTabOnFormMain.Name = "userControlPreferencesTabOnFormMain";
            this.userControlPreferencesTabOnFormMain.Size = new System.Drawing.Size(293, 213);
            this.userControlPreferencesTabOnFormMain.TabIndex = 4;
            // 
            // userControlOperationOnFormMain
            // 
            this.userControlOperationOnFormMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userControlOperationOnFormMain.Location = new System.Drawing.Point(0, 0);
            this.userControlOperationOnFormMain.Margin = new System.Windows.Forms.Padding(4);
            this.userControlOperationOnFormMain.Name = "userControlOperationOnFormMain";
            this.userControlOperationOnFormMain.Size = new System.Drawing.Size(293, 213);
            this.userControlOperationOnFormMain.TabIndex = 1;
            // 
            // userControlMainOnFormMain
            // 
            this.userControlMainOnFormMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userControlMainOnFormMain.Location = new System.Drawing.Point(0, 0);
            this.userControlMainOnFormMain.Margin = new System.Windows.Forms.Padding(4);
            this.userControlMainOnFormMain.Name = "userControlMainOnFormMain";
            this.userControlMainOnFormMain.Size = new System.Drawing.Size(293, 213);
            this.userControlMainOnFormMain.TabIndex = 0;
            // 
            // userControlCheckDeviceOnFormMain
            // 
            this.userControlCheckDeviceOnFormMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userControlCheckDeviceOnFormMain.Location = new System.Drawing.Point(0, 0);
            this.userControlCheckDeviceOnFormMain.Margin = new System.Windows.Forms.Padding(4);
            this.userControlCheckDeviceOnFormMain.Name = "userControlCheckDeviceOnFormMain";
            this.userControlCheckDeviceOnFormMain.Size = new System.Drawing.Size(293, 213);
            this.userControlCheckDeviceOnFormMain.TabIndex = 2;
            // 
            // serialPort1
            // 
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // timerCheckIo
            // 
            this.timerCheckIo.Tick += new System.EventHandler(this.timerCheckIo_Tick);
            // 
            // timerOperation
            // 
            this.timerOperation.Interval = 1;
            this.timerOperation.Tick += new System.EventHandler(this.timerOperation_Tick);
            // 
            // timerOperationStateMachine
            // 
            this.timerOperationStateMachine.Enabled = true;
            this.timerOperationStateMachine.Tick += new System.EventHandler(this.timerOperationStateMachine_Tick);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // backgroundWorker2
            // 
            this.backgroundWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker2_DoWork);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(293, 213);
            this.Controls.Add(this.panelBase);
            this.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormMain";
            this.Text = "Compartment";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.panelBase.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panelBase;
		private UserControlCheckDevice userControlCheckDeviceOnFormMain;
		private UserControlOperation userControlOperationOnFormMain;
		private UserControlMain userControlMainOnFormMain;
		private System.IO.Ports.SerialPort serialPort1;
		private UserControlPreferencesTab userControlPreferencesTabOnFormMain;
		private UserControlCheckIo userControlCheckIoOnFormMain;
		private UserControlInputCom userControlInputComOnFormMain;
		private System.Windows.Forms.Timer timerCheckIo;
		private System.Windows.Forms.Timer timerOperation;
		private System.Windows.Forms.Timer timerOperationStateMachine;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
    }
}


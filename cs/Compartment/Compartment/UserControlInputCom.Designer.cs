
namespace Compartment
{
	partial class UserControlInputCom
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
			this.labelComPort = new System.Windows.Forms.Label();
			this.comboBoxComPort = new System.Windows.Forms.ComboBox();
			this.buttonCOMSense = new System.Windows.Forms.Button();
			this.buttonEnd = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// labelComPort
			// 
			this.labelComPort.AutoSize = true;
			this.labelComPort.Location = new System.Drawing.Point(22, 22);
			this.labelComPort.Name = "labelComPort";
			this.labelComPort.Size = new System.Drawing.Size(56, 12);
			this.labelComPort.TabIndex = 0;
			this.labelComPort.Text = "COM port:";
			// 
			// comboBoxComPort
			// 
			this.comboBoxComPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxComPort.FormattingEnabled = true;
			this.comboBoxComPort.Location = new System.Drawing.Point(82, 19);
			this.comboBoxComPort.Name = "comboBoxComPort";
			this.comboBoxComPort.Size = new System.Drawing.Size(106, 20);
			this.comboBoxComPort.TabIndex = 1;
			// 
			// buttonCOMSense
			// 
			this.buttonCOMSense.Location = new System.Drawing.Point(84, 55);
			this.buttonCOMSense.Name = "buttonCOMSense";
			this.buttonCOMSense.Size = new System.Drawing.Size(103, 22);
			this.buttonCOMSense.TabIndex = 2;
			this.buttonCOMSense.Text = "COM sense";
			this.buttonCOMSense.UseVisualStyleBackColor = true;
			// 
			// buttonEnd
			// 
			this.buttonEnd.Location = new System.Drawing.Point(85, 93);
			this.buttonEnd.Name = "buttonEnd";
			this.buttonEnd.Size = new System.Drawing.Size(103, 22);
			this.buttonEnd.TabIndex = 3;
			this.buttonEnd.Text = "End";
			this.buttonEnd.UseVisualStyleBackColor = true;
			// 
			// UserControlInputCom
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.buttonEnd);
			this.Controls.Add(this.buttonCOMSense);
			this.Controls.Add(this.comboBoxComPort);
			this.Controls.Add(this.labelComPort);
			this.Name = "UserControlInputCom";
			this.Size = new System.Drawing.Size(399, 214);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelComPort;
		public System.Windows.Forms.ComboBox comboBoxComPort;
		public System.Windows.Forms.Button buttonCOMSense;
		public System.Windows.Forms.Button buttonEnd;
	}
}

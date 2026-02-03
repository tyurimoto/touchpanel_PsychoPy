namespace Compartment
{
    partial class UserControlDebugPanel
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
            if (disposing)
            {
                if (updateTimer != null)
                {
                    updateTimer.Stop();
                    updateTimer.Dispose();
                }
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private System.Windows.Forms.Timer updateTimer;

        #region コンポーネント デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.labelTitle = new System.Windows.Forms.Label();
            this.groupBoxSensors = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanelSensors = new System.Windows.Forms.TableLayoutPanel();
            this.labelEntranceName = new System.Windows.Forms.Label();
            this.buttonEntranceOn = new System.Windows.Forms.Button();
            this.buttonEntranceOff = new System.Windows.Forms.Button();
            this.labelEntranceStatus = new System.Windows.Forms.Label();
            this.labelExitName = new System.Windows.Forms.Label();
            this.buttonExitOn = new System.Windows.Forms.Button();
            this.buttonExitOff = new System.Windows.Forms.Button();
            this.labelExitStatus = new System.Windows.Forms.Label();
            this.labelStayName = new System.Windows.Forms.Label();
            this.buttonStayOn = new System.Windows.Forms.Button();
            this.buttonStayOff = new System.Windows.Forms.Button();
            this.labelStayStatus = new System.Windows.Forms.Label();
            this.labelLeverSwName = new System.Windows.Forms.Label();
            this.buttonLeverSwOn = new System.Windows.Forms.Button();
            this.buttonLeverSwOff = new System.Windows.Forms.Button();
            this.labelLeverSwStatus = new System.Windows.Forms.Label();
            this.groupBoxDeviceControl = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanelDeviceControl = new System.Windows.Forms.TableLayoutPanel();
            this.labelDoorControl = new System.Windows.Forms.Label();
            this.buttonDoorOpen = new System.Windows.Forms.Button();
            this.buttonDoorClose = new System.Windows.Forms.Button();
            this.labelLeverControl = new System.Windows.Forms.Label();
            this.buttonLeverExtend = new System.Windows.Forms.Button();
            this.buttonLeverRetract = new System.Windows.Forms.Button();
            this.labelFeedControl = new System.Windows.Forms.Label();
            this.buttonFeedDispense = new System.Windows.Forms.Button();
            this.labelRoomLampControl = new System.Windows.Forms.Label();
            this.buttonRoomLampOn = new System.Windows.Forms.Button();
            this.buttonRoomLampOff = new System.Windows.Forms.Button();
            this.labelLeverLampControl = new System.Windows.Forms.Label();
            this.buttonLeverLampOn = new System.Windows.Forms.Button();
            this.buttonLeverLampOff = new System.Windows.Forms.Button();
            this.groupBoxSensorStatus = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanelSensorStatus = new System.Windows.Forms.TableLayoutPanel();
            this.labelDoorOpenTitle = new System.Windows.Forms.Label();
            this.labelDoorOpenStatus = new System.Windows.Forms.Label();
            this.labelDoorCloseTitle = new System.Windows.Forms.Label();
            this.labelDoorCloseStatus = new System.Windows.Forms.Label();
            this.labelLeverInTitle = new System.Windows.Forms.Label();
            this.labelLeverInStatus = new System.Windows.Forms.Label();
            this.labelLeverOutTitle = new System.Windows.Forms.Label();
            this.labelLeverOutStatus = new System.Windows.Forms.Label();
            this.groupBoxRFID = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanelRFID = new System.Windows.Forms.TableLayoutPanel();
            this.labelRFIDTitle = new System.Windows.Forms.Label();
            this.textBoxRFID = new System.Windows.Forms.TextBox();
            this.buttonRFIDRandom = new System.Windows.Forms.Button();
            this.buttonRFIDSet = new System.Windows.Forms.Button();
            this.buttonRFIDClear = new System.Windows.Forms.Button();
            this.groupBoxLog = new System.Windows.Forms.GroupBox();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.tableLayoutPanelMain.SuspendLayout();
            this.groupBoxSensors.SuspendLayout();
            this.tableLayoutPanelSensors.SuspendLayout();
            this.groupBoxDeviceControl.SuspendLayout();
            this.tableLayoutPanelDeviceControl.SuspendLayout();
            this.groupBoxSensorStatus.SuspendLayout();
            this.tableLayoutPanelSensorStatus.SuspendLayout();
            this.groupBoxRFID.SuspendLayout();
            this.tableLayoutPanelRFID.SuspendLayout();
            this.groupBoxLog.SuspendLayout();
            this.SuspendLayout();
            //
            // tableLayoutPanelMain
            //
            this.tableLayoutPanelMain.ColumnCount = 1;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.Controls.Add(this.labelTitle, 0, 0);
            this.tableLayoutPanelMain.Controls.Add(this.groupBoxSensors, 0, 1);
            this.tableLayoutPanelMain.Controls.Add(this.groupBoxDeviceControl, 0, 2);
            this.tableLayoutPanelMain.Controls.Add(this.groupBoxSensorStatus, 0, 3);
            this.tableLayoutPanelMain.Controls.Add(this.groupBoxRFID, 0, 4);
            this.tableLayoutPanelMain.Controls.Add(this.groupBoxLog, 0, 5);
            this.tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 6;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(500, 800);
            this.tableLayoutPanelMain.TabIndex = 0;
            //
            // labelTitle
            //
            this.labelTitle.AutoSize = true;
            this.labelTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTitle.Font = new System.Drawing.Font("MS UI Gothic", 14F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(3, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(494, 40);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "デバッグコントロールパネル";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // groupBoxSensors
            //
            this.groupBoxSensors.Controls.Add(this.tableLayoutPanelSensors);
            this.groupBoxSensors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxSensors.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold);
            this.groupBoxSensors.Location = new System.Drawing.Point(3, 43);
            this.groupBoxSensors.Name = "groupBoxSensors";
            this.groupBoxSensors.Size = new System.Drawing.Size(494, 174);
            this.groupBoxSensors.TabIndex = 1;
            this.groupBoxSensors.TabStop = false;
            this.groupBoxSensors.Text = "センサー（手動シミュレート）";
            //
            // tableLayoutPanelSensors
            //
            this.tableLayoutPanelSensors.ColumnCount = 4;
            this.tableLayoutPanelSensors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tableLayoutPanelSensors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tableLayoutPanelSensors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tableLayoutPanelSensors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.34F));
            this.tableLayoutPanelSensors.Controls.Add(this.labelEntranceName, 0, 0);
            this.tableLayoutPanelSensors.Controls.Add(this.buttonEntranceOn, 1, 0);
            this.tableLayoutPanelSensors.Controls.Add(this.buttonEntranceOff, 2, 0);
            this.tableLayoutPanelSensors.Controls.Add(this.labelEntranceStatus, 3, 0);
            this.tableLayoutPanelSensors.Controls.Add(this.labelExitName, 0, 1);
            this.tableLayoutPanelSensors.Controls.Add(this.buttonExitOn, 1, 1);
            this.tableLayoutPanelSensors.Controls.Add(this.buttonExitOff, 2, 1);
            this.tableLayoutPanelSensors.Controls.Add(this.labelExitStatus, 3, 1);
            this.tableLayoutPanelSensors.Controls.Add(this.labelStayName, 0, 2);
            this.tableLayoutPanelSensors.Controls.Add(this.buttonStayOn, 1, 2);
            this.tableLayoutPanelSensors.Controls.Add(this.buttonStayOff, 2, 2);
            this.tableLayoutPanelSensors.Controls.Add(this.labelStayStatus, 3, 2);
            this.tableLayoutPanelSensors.Controls.Add(this.labelLeverSwName, 0, 3);
            this.tableLayoutPanelSensors.Controls.Add(this.buttonLeverSwOn, 1, 3);
            this.tableLayoutPanelSensors.Controls.Add(this.buttonLeverSwOff, 2, 3);
            this.tableLayoutPanelSensors.Controls.Add(this.labelLeverSwStatus, 3, 3);
            this.tableLayoutPanelSensors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelSensors.Location = new System.Drawing.Point(3, 17);
            this.tableLayoutPanelSensors.Name = "tableLayoutPanelSensors";
            this.tableLayoutPanelSensors.RowCount = 4;
            this.tableLayoutPanelSensors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanelSensors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanelSensors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanelSensors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanelSensors.Size = new System.Drawing.Size(488, 154);
            this.tableLayoutPanelSensors.TabIndex = 0;
            //
            // labelEntranceName
            //
            this.labelEntranceName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelEntranceName.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.labelEntranceName.Location = new System.Drawing.Point(3, 0);
            this.labelEntranceName.Name = "labelEntranceName";
            this.labelEntranceName.Size = new System.Drawing.Size(104, 38);
            this.labelEntranceName.TabIndex = 0;
            this.labelEntranceName.Text = "入室センサー";
            this.labelEntranceName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // buttonEntranceOn
            //
            this.buttonEntranceOn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonEntranceOn.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.buttonEntranceOn.Location = new System.Drawing.Point(113, 3);
            this.buttonEntranceOn.Name = "buttonEntranceOn";
            this.buttonEntranceOn.Size = new System.Drawing.Size(116, 32);
            this.buttonEntranceOn.TabIndex = 1;
            this.buttonEntranceOn.Text = "ON";
            this.buttonEntranceOn.UseVisualStyleBackColor = true;
            //
            // buttonEntranceOff
            //
            this.buttonEntranceOff.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonEntranceOff.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.buttonEntranceOff.Location = new System.Drawing.Point(235, 3);
            this.buttonEntranceOff.Name = "buttonEntranceOff";
            this.buttonEntranceOff.Size = new System.Drawing.Size(116, 32);
            this.buttonEntranceOff.TabIndex = 2;
            this.buttonEntranceOff.Text = "OFF";
            this.buttonEntranceOff.UseVisualStyleBackColor = true;
            //
            // labelEntranceStatus
            //
            this.labelEntranceStatus.BackColor = System.Drawing.Color.Gray;
            this.labelEntranceStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelEntranceStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelEntranceStatus.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold);
            this.labelEntranceStatus.Location = new System.Drawing.Point(357, 0);
            this.labelEntranceStatus.Name = "labelEntranceStatus";
            this.labelEntranceStatus.Size = new System.Drawing.Size(128, 38);
            this.labelEntranceStatus.TabIndex = 3;
            this.labelEntranceStatus.Text = "OFF";
            this.labelEntranceStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // labelExitName
            //
            this.labelExitName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelExitName.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.labelExitName.Location = new System.Drawing.Point(3, 38);
            this.labelExitName.Name = "labelExitName";
            this.labelExitName.Size = new System.Drawing.Size(104, 38);
            this.labelExitName.TabIndex = 4;
            this.labelExitName.Text = "退室センサー";
            this.labelExitName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // buttonExitOn
            //
            this.buttonExitOn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonExitOn.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.buttonExitOn.Location = new System.Drawing.Point(113, 41);
            this.buttonExitOn.Name = "buttonExitOn";
            this.buttonExitOn.Size = new System.Drawing.Size(116, 32);
            this.buttonExitOn.TabIndex = 5;
            this.buttonExitOn.Text = "ON";
            this.buttonExitOn.UseVisualStyleBackColor = true;
            //
            // buttonExitOff
            //
            this.buttonExitOff.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonExitOff.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.buttonExitOff.Location = new System.Drawing.Point(235, 41);
            this.buttonExitOff.Name = "buttonExitOff";
            this.buttonExitOff.Size = new System.Drawing.Size(116, 32);
            this.buttonExitOff.TabIndex = 6;
            this.buttonExitOff.Text = "OFF";
            this.buttonExitOff.UseVisualStyleBackColor = true;
            //
            // labelExitStatus
            //
            this.labelExitStatus.BackColor = System.Drawing.Color.Gray;
            this.labelExitStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelExitStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelExitStatus.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold);
            this.labelExitStatus.Location = new System.Drawing.Point(357, 38);
            this.labelExitStatus.Name = "labelExitStatus";
            this.labelExitStatus.Size = new System.Drawing.Size(128, 38);
            this.labelExitStatus.TabIndex = 7;
            this.labelExitStatus.Text = "OFF";
            this.labelExitStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // labelStayName
            //
            this.labelStayName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelStayName.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.labelStayName.Location = new System.Drawing.Point(3, 76);
            this.labelStayName.Name = "labelStayName";
            this.labelStayName.Size = new System.Drawing.Size(104, 38);
            this.labelStayName.TabIndex = 8;
            this.labelStayName.Text = "在室センサー";
            this.labelStayName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // buttonStayOn
            //
            this.buttonStayOn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonStayOn.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.buttonStayOn.Location = new System.Drawing.Point(113, 79);
            this.buttonStayOn.Name = "buttonStayOn";
            this.buttonStayOn.Size = new System.Drawing.Size(116, 32);
            this.buttonStayOn.TabIndex = 9;
            this.buttonStayOn.Text = "ON";
            this.buttonStayOn.UseVisualStyleBackColor = true;
            //
            // buttonStayOff
            //
            this.buttonStayOff.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonStayOff.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.buttonStayOff.Location = new System.Drawing.Point(235, 79);
            this.buttonStayOff.Name = "buttonStayOff";
            this.buttonStayOff.Size = new System.Drawing.Size(116, 32);
            this.buttonStayOff.TabIndex = 10;
            this.buttonStayOff.Text = "OFF";
            this.buttonStayOff.UseVisualStyleBackColor = true;
            //
            // labelStayStatus
            //
            this.labelStayStatus.BackColor = System.Drawing.Color.Gray;
            this.labelStayStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelStayStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelStayStatus.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold);
            this.labelStayStatus.Location = new System.Drawing.Point(357, 76);
            this.labelStayStatus.Name = "labelStayStatus";
            this.labelStayStatus.Size = new System.Drawing.Size(128, 38);
            this.labelStayStatus.TabIndex = 11;
            this.labelStayStatus.Text = "OFF";
            this.labelStayStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // labelLeverSwName
            //
            this.labelLeverSwName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelLeverSwName.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.labelLeverSwName.Location = new System.Drawing.Point(3, 114);
            this.labelLeverSwName.Name = "labelLeverSwName";
            this.labelLeverSwName.Size = new System.Drawing.Size(104, 40);
            this.labelLeverSwName.TabIndex = 12;
            this.labelLeverSwName.Text = "レバーSW";
            this.labelLeverSwName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // buttonLeverSwOn
            //
            this.buttonLeverSwOn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonLeverSwOn.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.buttonLeverSwOn.Location = new System.Drawing.Point(113, 117);
            this.buttonLeverSwOn.Name = "buttonLeverSwOn";
            this.buttonLeverSwOn.Size = new System.Drawing.Size(116, 34);
            this.buttonLeverSwOn.TabIndex = 13;
            this.buttonLeverSwOn.Text = "ON";
            this.buttonLeverSwOn.UseVisualStyleBackColor = true;
            //
            // buttonLeverSwOff
            //
            this.buttonLeverSwOff.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonLeverSwOff.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.buttonLeverSwOff.Location = new System.Drawing.Point(235, 117);
            this.buttonLeverSwOff.Name = "buttonLeverSwOff";
            this.buttonLeverSwOff.Size = new System.Drawing.Size(116, 34);
            this.buttonLeverSwOff.TabIndex = 14;
            this.buttonLeverSwOff.Text = "OFF";
            this.buttonLeverSwOff.UseVisualStyleBackColor = true;
            //
            // labelLeverSwStatus
            //
            this.labelLeverSwStatus.BackColor = System.Drawing.Color.Gray;
            this.labelLeverSwStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelLeverSwStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelLeverSwStatus.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold);
            this.labelLeverSwStatus.Location = new System.Drawing.Point(357, 114);
            this.labelLeverSwStatus.Name = "labelLeverSwStatus";
            this.labelLeverSwStatus.Size = new System.Drawing.Size(128, 40);
            this.labelLeverSwStatus.TabIndex = 15;
            this.labelLeverSwStatus.Text = "OFF";
            this.labelLeverSwStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // groupBoxDeviceControl
            //
            this.groupBoxDeviceControl.Controls.Add(this.tableLayoutPanelDeviceControl);
            this.groupBoxDeviceControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxDeviceControl.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold);
            this.groupBoxDeviceControl.Location = new System.Drawing.Point(3, 223);
            this.groupBoxDeviceControl.Name = "groupBoxDeviceControl";
            this.groupBoxDeviceControl.Size = new System.Drawing.Size(494, 194);
            this.groupBoxDeviceControl.TabIndex = 2;
            this.groupBoxDeviceControl.TabStop = false;
            this.groupBoxDeviceControl.Text = "デバイス制御";
            //
            // tableLayoutPanelDeviceControl
            //
            this.tableLayoutPanelDeviceControl.ColumnCount = 3;
            this.tableLayoutPanelDeviceControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tableLayoutPanelDeviceControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelDeviceControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelDeviceControl.Controls.Add(this.labelDoorControl, 0, 0);
            this.tableLayoutPanelDeviceControl.Controls.Add(this.buttonDoorOpen, 1, 0);
            this.tableLayoutPanelDeviceControl.Controls.Add(this.buttonDoorClose, 2, 0);
            this.tableLayoutPanelDeviceControl.Controls.Add(this.labelLeverControl, 0, 1);
            this.tableLayoutPanelDeviceControl.Controls.Add(this.buttonLeverExtend, 1, 1);
            this.tableLayoutPanelDeviceControl.Controls.Add(this.buttonLeverRetract, 2, 1);
            this.tableLayoutPanelDeviceControl.Controls.Add(this.labelFeedControl, 0, 2);
            this.tableLayoutPanelDeviceControl.Controls.Add(this.buttonFeedDispense, 1, 2);
            this.tableLayoutPanelDeviceControl.Controls.Add(this.labelRoomLampControl, 0, 3);
            this.tableLayoutPanelDeviceControl.Controls.Add(this.buttonRoomLampOn, 1, 3);
            this.tableLayoutPanelDeviceControl.Controls.Add(this.buttonRoomLampOff, 2, 3);
            this.tableLayoutPanelDeviceControl.Controls.Add(this.labelLeverLampControl, 0, 4);
            this.tableLayoutPanelDeviceControl.Controls.Add(this.buttonLeverLampOn, 1, 4);
            this.tableLayoutPanelDeviceControl.Controls.Add(this.buttonLeverLampOff, 2, 4);
            this.tableLayoutPanelDeviceControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelDeviceControl.Location = new System.Drawing.Point(3, 17);
            this.tableLayoutPanelDeviceControl.Name = "tableLayoutPanelDeviceControl";
            this.tableLayoutPanelDeviceControl.RowCount = 5;
            this.tableLayoutPanelDeviceControl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanelDeviceControl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanelDeviceControl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanelDeviceControl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanelDeviceControl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanelDeviceControl.Size = new System.Drawing.Size(488, 174);
            this.tableLayoutPanelDeviceControl.TabIndex = 0;
            //
            // labelDoorControl
            //
            this.labelDoorControl.AutoSize = true;
            this.labelDoorControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelDoorControl.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.labelDoorControl.Location = new System.Drawing.Point(3, 0);
            this.labelDoorControl.Name = "labelDoorControl";
            this.labelDoorControl.Size = new System.Drawing.Size(104, 34);
            this.labelDoorControl.TabIndex = 0;
            this.labelDoorControl.Text = "ドア:";
            this.labelDoorControl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // buttonDoorOpen
            //
            this.buttonDoorOpen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonDoorOpen.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.buttonDoorOpen.Location = new System.Drawing.Point(113, 3);
            this.buttonDoorOpen.Name = "buttonDoorOpen";
            this.buttonDoorOpen.Size = new System.Drawing.Size(183, 28);
            this.buttonDoorOpen.TabIndex = 1;
            this.buttonDoorOpen.Text = "開く";
            this.buttonDoorOpen.UseVisualStyleBackColor = true;
            //
            // buttonDoorClose
            //
            this.buttonDoorClose.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonDoorClose.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.buttonDoorClose.Location = new System.Drawing.Point(302, 3);
            this.buttonDoorClose.Name = "buttonDoorClose";
            this.buttonDoorClose.Size = new System.Drawing.Size(183, 28);
            this.buttonDoorClose.TabIndex = 2;
            this.buttonDoorClose.Text = "閉じる";
            this.buttonDoorClose.UseVisualStyleBackColor = true;
            //
            // labelLeverControl
            //
            this.labelLeverControl.AutoSize = true;
            this.labelLeverControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelLeverControl.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.labelLeverControl.Location = new System.Drawing.Point(3, 34);
            this.labelLeverControl.Name = "labelLeverControl";
            this.labelLeverControl.Size = new System.Drawing.Size(104, 34);
            this.labelLeverControl.TabIndex = 3;
            this.labelLeverControl.Text = "レバー:";
            this.labelLeverControl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // buttonLeverExtend
            //
            this.buttonLeverExtend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonLeverExtend.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.buttonLeverExtend.Location = new System.Drawing.Point(113, 37);
            this.buttonLeverExtend.Name = "buttonLeverExtend";
            this.buttonLeverExtend.Size = new System.Drawing.Size(183, 28);
            this.buttonLeverExtend.TabIndex = 4;
            this.buttonLeverExtend.Text = "出す";
            this.buttonLeverExtend.UseVisualStyleBackColor = true;
            //
            // buttonLeverRetract
            //
            this.buttonLeverRetract.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonLeverRetract.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.buttonLeverRetract.Location = new System.Drawing.Point(302, 37);
            this.buttonLeverRetract.Name = "buttonLeverRetract";
            this.buttonLeverRetract.Size = new System.Drawing.Size(183, 28);
            this.buttonLeverRetract.TabIndex = 5;
            this.buttonLeverRetract.Text = "引っ込める";
            this.buttonLeverRetract.UseVisualStyleBackColor = true;
            //
            // labelFeedControl
            //
            this.labelFeedControl.AutoSize = true;
            this.labelFeedControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelFeedControl.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.labelFeedControl.Location = new System.Drawing.Point(3, 68);
            this.labelFeedControl.Name = "labelFeedControl";
            this.labelFeedControl.Size = new System.Drawing.Size(104, 34);
            this.labelFeedControl.TabIndex = 6;
            this.labelFeedControl.Text = "給餌:";
            this.labelFeedControl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // buttonFeedDispense
            //
            this.tableLayoutPanelDeviceControl.SetColumnSpan(this.buttonFeedDispense, 2);
            this.buttonFeedDispense.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonFeedDispense.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.buttonFeedDispense.Location = new System.Drawing.Point(113, 71);
            this.buttonFeedDispense.Name = "buttonFeedDispense";
            this.buttonFeedDispense.Size = new System.Drawing.Size(372, 28);
            this.buttonFeedDispense.TabIndex = 7;
            this.buttonFeedDispense.Text = "給餌実行 (1秒)";
            this.buttonFeedDispense.UseVisualStyleBackColor = true;
            //
            // labelRoomLampControl
            //
            this.labelRoomLampControl.AutoSize = true;
            this.labelRoomLampControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelRoomLampControl.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.labelRoomLampControl.Location = new System.Drawing.Point(3, 102);
            this.labelRoomLampControl.Name = "labelRoomLampControl";
            this.labelRoomLampControl.Size = new System.Drawing.Size(104, 34);
            this.labelRoomLampControl.TabIndex = 8;
            this.labelRoomLampControl.Text = "ルームランプ:";
            this.labelRoomLampControl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // buttonRoomLampOn
            //
            this.buttonRoomLampOn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonRoomLampOn.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.buttonRoomLampOn.Location = new System.Drawing.Point(113, 105);
            this.buttonRoomLampOn.Name = "buttonRoomLampOn";
            this.buttonRoomLampOn.Size = new System.Drawing.Size(183, 28);
            this.buttonRoomLampOn.TabIndex = 9;
            this.buttonRoomLampOn.Text = "ON";
            this.buttonRoomLampOn.UseVisualStyleBackColor = true;
            //
            // buttonRoomLampOff
            //
            this.buttonRoomLampOff.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonRoomLampOff.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.buttonRoomLampOff.Location = new System.Drawing.Point(302, 105);
            this.buttonRoomLampOff.Name = "buttonRoomLampOff";
            this.buttonRoomLampOff.Size = new System.Drawing.Size(183, 28);
            this.buttonRoomLampOff.TabIndex = 10;
            this.buttonRoomLampOff.Text = "OFF";
            this.buttonRoomLampOff.UseVisualStyleBackColor = true;
            //
            // labelLeverLampControl
            //
            this.labelLeverLampControl.AutoSize = true;
            this.labelLeverLampControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelLeverLampControl.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.labelLeverLampControl.Location = new System.Drawing.Point(3, 136);
            this.labelLeverLampControl.Name = "labelLeverLampControl";
            this.labelLeverLampControl.Size = new System.Drawing.Size(104, 38);
            this.labelLeverLampControl.TabIndex = 11;
            this.labelLeverLampControl.Text = "レバーランプ:";
            this.labelLeverLampControl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // buttonLeverLampOn
            //
            this.buttonLeverLampOn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonLeverLampOn.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.buttonLeverLampOn.Location = new System.Drawing.Point(113, 139);
            this.buttonLeverLampOn.Name = "buttonLeverLampOn";
            this.buttonLeverLampOn.Size = new System.Drawing.Size(183, 32);
            this.buttonLeverLampOn.TabIndex = 12;
            this.buttonLeverLampOn.Text = "ON";
            this.buttonLeverLampOn.UseVisualStyleBackColor = true;
            //
            // buttonLeverLampOff
            //
            this.buttonLeverLampOff.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonLeverLampOff.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.buttonLeverLampOff.Location = new System.Drawing.Point(302, 139);
            this.buttonLeverLampOff.Name = "buttonLeverLampOff";
            this.buttonLeverLampOff.Size = new System.Drawing.Size(183, 32);
            this.buttonLeverLampOff.TabIndex = 13;
            this.buttonLeverLampOff.Text = "OFF";
            this.buttonLeverLampOff.UseVisualStyleBackColor = true;
            //
            // groupBoxSensorStatus
            //
            this.groupBoxSensorStatus.Controls.Add(this.tableLayoutPanelSensorStatus);
            this.groupBoxSensorStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxSensorStatus.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold);
            this.groupBoxSensorStatus.Location = new System.Drawing.Point(3, 423);
            this.groupBoxSensorStatus.Name = "groupBoxSensorStatus";
            this.groupBoxSensorStatus.Size = new System.Drawing.Size(494, 134);
            this.groupBoxSensorStatus.TabIndex = 3;
            this.groupBoxSensorStatus.TabStop = false;
            this.groupBoxSensorStatus.Text = "センサー状態（リアルタイム表示）";
            //
            // tableLayoutPanelSensorStatus
            //
            this.tableLayoutPanelSensorStatus.ColumnCount = 2;
            this.tableLayoutPanelSensorStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanelSensorStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSensorStatus.Controls.Add(this.labelDoorOpenTitle, 0, 0);
            this.tableLayoutPanelSensorStatus.Controls.Add(this.labelDoorOpenStatus, 1, 0);
            this.tableLayoutPanelSensorStatus.Controls.Add(this.labelDoorCloseTitle, 0, 1);
            this.tableLayoutPanelSensorStatus.Controls.Add(this.labelDoorCloseStatus, 1, 1);
            this.tableLayoutPanelSensorStatus.Controls.Add(this.labelLeverInTitle, 0, 2);
            this.tableLayoutPanelSensorStatus.Controls.Add(this.labelLeverInStatus, 1, 2);
            this.tableLayoutPanelSensorStatus.Controls.Add(this.labelLeverOutTitle, 0, 3);
            this.tableLayoutPanelSensorStatus.Controls.Add(this.labelLeverOutStatus, 1, 3);
            this.tableLayoutPanelSensorStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelSensorStatus.Location = new System.Drawing.Point(3, 17);
            this.tableLayoutPanelSensorStatus.Name = "tableLayoutPanelSensorStatus";
            this.tableLayoutPanelSensorStatus.RowCount = 4;
            this.tableLayoutPanelSensorStatus.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanelSensorStatus.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanelSensorStatus.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanelSensorStatus.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanelSensorStatus.Size = new System.Drawing.Size(488, 114);
            this.tableLayoutPanelSensorStatus.TabIndex = 0;
            //
            // labelDoorOpenTitle
            //
            this.labelDoorOpenTitle.AutoSize = true;
            this.labelDoorOpenTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelDoorOpenTitle.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.labelDoorOpenTitle.Location = new System.Drawing.Point(3, 0);
            this.labelDoorOpenTitle.Name = "labelDoorOpenTitle";
            this.labelDoorOpenTitle.Size = new System.Drawing.Size(144, 28);
            this.labelDoorOpenTitle.TabIndex = 0;
            this.labelDoorOpenTitle.Text = "ドア開センサー:";
            this.labelDoorOpenTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // labelDoorOpenStatus
            //
            this.labelDoorOpenStatus.AutoSize = true;
            this.labelDoorOpenStatus.BackColor = System.Drawing.Color.Gray;
            this.labelDoorOpenStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelDoorOpenStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelDoorOpenStatus.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold);
            this.labelDoorOpenStatus.Location = new System.Drawing.Point(153, 0);
            this.labelDoorOpenStatus.Name = "labelDoorOpenStatus";
            this.labelDoorOpenStatus.Size = new System.Drawing.Size(332, 28);
            this.labelDoorOpenStatus.TabIndex = 1;
            this.labelDoorOpenStatus.Text = "OFF";
            this.labelDoorOpenStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // labelDoorCloseTitle
            //
            this.labelDoorCloseTitle.AutoSize = true;
            this.labelDoorCloseTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelDoorCloseTitle.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.labelDoorCloseTitle.Location = new System.Drawing.Point(3, 28);
            this.labelDoorCloseTitle.Name = "labelDoorCloseTitle";
            this.labelDoorCloseTitle.Size = new System.Drawing.Size(144, 28);
            this.labelDoorCloseTitle.TabIndex = 2;
            this.labelDoorCloseTitle.Text = "ドア閉センサー:";
            this.labelDoorCloseTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // labelDoorCloseStatus
            //
            this.labelDoorCloseStatus.AutoSize = true;
            this.labelDoorCloseStatus.BackColor = System.Drawing.Color.Gray;
            this.labelDoorCloseStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelDoorCloseStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelDoorCloseStatus.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold);
            this.labelDoorCloseStatus.Location = new System.Drawing.Point(153, 28);
            this.labelDoorCloseStatus.Name = "labelDoorCloseStatus";
            this.labelDoorCloseStatus.Size = new System.Drawing.Size(332, 28);
            this.labelDoorCloseStatus.TabIndex = 3;
            this.labelDoorCloseStatus.Text = "OFF";
            this.labelDoorCloseStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // labelLeverInTitle
            //
            this.labelLeverInTitle.AutoSize = true;
            this.labelLeverInTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelLeverInTitle.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.labelLeverInTitle.Location = new System.Drawing.Point(3, 56);
            this.labelLeverInTitle.Name = "labelLeverInTitle";
            this.labelLeverInTitle.Size = new System.Drawing.Size(144, 28);
            this.labelLeverInTitle.TabIndex = 4;
            this.labelLeverInTitle.Text = "レバーInセンサー:";
            this.labelLeverInTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // labelLeverInStatus
            //
            this.labelLeverInStatus.AutoSize = true;
            this.labelLeverInStatus.BackColor = System.Drawing.Color.Gray;
            this.labelLeverInStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelLeverInStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelLeverInStatus.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold);
            this.labelLeverInStatus.Location = new System.Drawing.Point(153, 56);
            this.labelLeverInStatus.Name = "labelLeverInStatus";
            this.labelLeverInStatus.Size = new System.Drawing.Size(332, 28);
            this.labelLeverInStatus.TabIndex = 5;
            this.labelLeverInStatus.Text = "OFF";
            this.labelLeverInStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // labelLeverOutTitle
            //
            this.labelLeverOutTitle.AutoSize = true;
            this.labelLeverOutTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelLeverOutTitle.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.labelLeverOutTitle.Location = new System.Drawing.Point(3, 84);
            this.labelLeverOutTitle.Name = "labelLeverOutTitle";
            this.labelLeverOutTitle.Size = new System.Drawing.Size(144, 30);
            this.labelLeverOutTitle.TabIndex = 6;
            this.labelLeverOutTitle.Text = "レバーOutセンサー:";
            this.labelLeverOutTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // labelLeverOutStatus
            //
            this.labelLeverOutStatus.AutoSize = true;
            this.labelLeverOutStatus.BackColor = System.Drawing.Color.Gray;
            this.labelLeverOutStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelLeverOutStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelLeverOutStatus.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold);
            this.labelLeverOutStatus.Location = new System.Drawing.Point(153, 84);
            this.labelLeverOutStatus.Name = "labelLeverOutStatus";
            this.labelLeverOutStatus.Size = new System.Drawing.Size(332, 30);
            this.labelLeverOutStatus.TabIndex = 7;
            this.labelLeverOutStatus.Text = "OFF";
            this.labelLeverOutStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // groupBoxRFID
            //
            this.groupBoxRFID.Controls.Add(this.tableLayoutPanelRFID);
            this.groupBoxRFID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxRFID.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold);
            this.groupBoxRFID.Location = new System.Drawing.Point(3, 563);
            this.groupBoxRFID.Name = "groupBoxRFID";
            this.groupBoxRFID.Size = new System.Drawing.Size(494, 114);
            this.groupBoxRFID.TabIndex = 4;
            this.groupBoxRFID.TabStop = false;
            this.groupBoxRFID.Text = "RFID（手動設定）";
            //
            // tableLayoutPanelRFID
            //
            this.tableLayoutPanelRFID.ColumnCount = 2;
            this.tableLayoutPanelRFID.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanelRFID.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelRFID.Controls.Add(this.labelRFIDTitle, 0, 0);
            this.tableLayoutPanelRFID.Controls.Add(this.textBoxRFID, 1, 0);
            this.tableLayoutPanelRFID.Controls.Add(this.buttonRFIDRandom, 1, 1);
            this.tableLayoutPanelRFID.Controls.Add(this.buttonRFIDSet, 0, 2);
            this.tableLayoutPanelRFID.Controls.Add(this.buttonRFIDClear, 1, 2);
            this.tableLayoutPanelRFID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelRFID.Location = new System.Drawing.Point(3, 17);
            this.tableLayoutPanelRFID.Name = "tableLayoutPanelRFID";
            this.tableLayoutPanelRFID.RowCount = 3;
            this.tableLayoutPanelRFID.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanelRFID.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanelRFID.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanelRFID.Size = new System.Drawing.Size(488, 94);
            this.tableLayoutPanelRFID.TabIndex = 0;
            //
            // labelRFIDTitle
            //
            this.labelRFIDTitle.AutoSize = true;
            this.labelRFIDTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelRFIDTitle.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.labelRFIDTitle.Location = new System.Drawing.Point(3, 0);
            this.labelRFIDTitle.Name = "labelRFIDTitle";
            this.labelRFIDTitle.Size = new System.Drawing.Size(94, 30);
            this.labelRFIDTitle.TabIndex = 0;
            this.labelRFIDTitle.Text = "ID:";
            this.labelRFIDTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // textBoxRFID
            //
            this.textBoxRFID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxRFID.Font = new System.Drawing.Font("MS UI Gothic", 10F);
            this.textBoxRFID.Location = new System.Drawing.Point(103, 3);
            this.textBoxRFID.MaxLength = 16;
            this.textBoxRFID.Name = "textBoxRFID";
            this.textBoxRFID.Size = new System.Drawing.Size(382, 21);
            this.textBoxRFID.TabIndex = 1;
            //
            // buttonRFIDRandom
            //
            this.buttonRFIDRandom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonRFIDRandom.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.buttonRFIDRandom.Location = new System.Drawing.Point(103, 33);
            this.buttonRFIDRandom.Name = "buttonRFIDRandom";
            this.buttonRFIDRandom.Size = new System.Drawing.Size(382, 24);
            this.buttonRFIDRandom.TabIndex = 2;
            this.buttonRFIDRandom.Text = "ランダム生成";
            this.buttonRFIDRandom.UseVisualStyleBackColor = true;
            //
            // buttonRFIDSet
            //
            this.tableLayoutPanelRFID.SetColumnSpan(this.buttonRFIDSet, 1);
            this.buttonRFIDSet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonRFIDSet.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.buttonRFIDSet.Location = new System.Drawing.Point(3, 63);
            this.buttonRFIDSet.Name = "buttonRFIDSet";
            this.buttonRFIDSet.Size = new System.Drawing.Size(94, 28);
            this.buttonRFIDSet.TabIndex = 3;
            this.buttonRFIDSet.Text = "設定";
            this.buttonRFIDSet.UseVisualStyleBackColor = true;
            //
            // buttonRFIDClear
            //
            this.buttonRFIDClear.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonRFIDClear.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.buttonRFIDClear.Location = new System.Drawing.Point(103, 63);
            this.buttonRFIDClear.Name = "buttonRFIDClear";
            this.buttonRFIDClear.Size = new System.Drawing.Size(382, 28);
            this.buttonRFIDClear.TabIndex = 4;
            this.buttonRFIDClear.Text = "クリア";
            this.buttonRFIDClear.UseVisualStyleBackColor = true;
            //
            // groupBoxLog
            //
            this.groupBoxLog.Controls.Add(this.textBoxLog);
            this.groupBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxLog.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold);
            this.groupBoxLog.Location = new System.Drawing.Point(3, 683);
            this.groupBoxLog.Name = "groupBoxLog";
            this.groupBoxLog.Size = new System.Drawing.Size(494, 114);
            this.groupBoxLog.TabIndex = 5;
            this.groupBoxLog.TabStop = false;
            this.groupBoxLog.Text = "ログ";
            //
            // textBoxLog
            //
            this.textBoxLog.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxLog.Font = new System.Drawing.Font("MS UI Gothic", 8F);
            this.textBoxLog.Location = new System.Drawing.Point(3, 17);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLog.Size = new System.Drawing.Size(488, 94);
            this.textBoxLog.TabIndex = 0;
            //
            // UserControlDebugPanel
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanelMain);
            this.Name = "UserControlDebugPanel";
            this.Size = new System.Drawing.Size(500, 800);
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.tableLayoutPanelMain.PerformLayout();
            this.groupBoxSensors.ResumeLayout(false);
            this.tableLayoutPanelSensors.ResumeLayout(false);
            this.tableLayoutPanelSensors.PerformLayout();
            this.groupBoxDeviceControl.ResumeLayout(false);
            this.tableLayoutPanelDeviceControl.ResumeLayout(false);
            this.tableLayoutPanelDeviceControl.PerformLayout();
            this.groupBoxSensorStatus.ResumeLayout(false);
            this.tableLayoutPanelSensorStatus.ResumeLayout(false);
            this.tableLayoutPanelSensorStatus.PerformLayout();
            this.groupBoxRFID.ResumeLayout(false);
            this.tableLayoutPanelRFID.ResumeLayout(false);
            this.tableLayoutPanelRFID.PerformLayout();
            this.groupBoxLog.ResumeLayout(false);
            this.groupBoxLog.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.Label labelTitle;

        // センサー（手動シミュレート）
        private System.Windows.Forms.GroupBox groupBoxSensors;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSensors;
        private System.Windows.Forms.Label labelEntranceName;
        private System.Windows.Forms.Button buttonEntranceOn;
        private System.Windows.Forms.Button buttonEntranceOff;
        private System.Windows.Forms.Label labelEntranceStatus;
        private System.Windows.Forms.Label labelExitName;
        private System.Windows.Forms.Button buttonExitOn;
        private System.Windows.Forms.Button buttonExitOff;
        private System.Windows.Forms.Label labelExitStatus;
        private System.Windows.Forms.Label labelStayName;
        private System.Windows.Forms.Button buttonStayOn;
        private System.Windows.Forms.Button buttonStayOff;
        private System.Windows.Forms.Label labelStayStatus;
        private System.Windows.Forms.Label labelLeverSwName;
        private System.Windows.Forms.Button buttonLeverSwOn;
        private System.Windows.Forms.Button buttonLeverSwOff;
        private System.Windows.Forms.Label labelLeverSwStatus;

        // デバイス制御
        private System.Windows.Forms.GroupBox groupBoxDeviceControl;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelDeviceControl;
        private System.Windows.Forms.Label labelDoorControl;
        private System.Windows.Forms.Button buttonDoorOpen;
        private System.Windows.Forms.Button buttonDoorClose;
        private System.Windows.Forms.Label labelLeverControl;
        private System.Windows.Forms.Button buttonLeverExtend;
        private System.Windows.Forms.Button buttonLeverRetract;
        private System.Windows.Forms.Label labelFeedControl;
        private System.Windows.Forms.Button buttonFeedDispense;
        private System.Windows.Forms.Label labelRoomLampControl;
        private System.Windows.Forms.Button buttonRoomLampOn;
        private System.Windows.Forms.Button buttonRoomLampOff;
        private System.Windows.Forms.Label labelLeverLampControl;
        private System.Windows.Forms.Button buttonLeverLampOn;
        private System.Windows.Forms.Button buttonLeverLampOff;

        // センサー状態表示
        private System.Windows.Forms.GroupBox groupBoxSensorStatus;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSensorStatus;
        private System.Windows.Forms.Label labelDoorOpenTitle;
        private System.Windows.Forms.Label labelDoorOpenStatus;
        private System.Windows.Forms.Label labelDoorCloseTitle;
        private System.Windows.Forms.Label labelDoorCloseStatus;
        private System.Windows.Forms.Label labelLeverInTitle;
        private System.Windows.Forms.Label labelLeverInStatus;
        private System.Windows.Forms.Label labelLeverOutTitle;
        private System.Windows.Forms.Label labelLeverOutStatus;

        // RFID
        private System.Windows.Forms.GroupBox groupBoxRFID;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelRFID;
        private System.Windows.Forms.Label labelRFIDTitle;
        private System.Windows.Forms.TextBox textBoxRFID;
        private System.Windows.Forms.Button buttonRFIDRandom;
        private System.Windows.Forms.Button buttonRFIDSet;
        private System.Windows.Forms.Button buttonRFIDClear;

        // ログ
        private System.Windows.Forms.GroupBox groupBoxLog;
        private System.Windows.Forms.TextBox textBoxLog;
    }
}

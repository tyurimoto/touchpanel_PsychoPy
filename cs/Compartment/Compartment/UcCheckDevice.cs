using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace Compartment
{
    class UcCheckDevice
    {
    }
    public partial class FormMain : Form
    {
        SoundPlayer SoundPlayerCheckDevice = new SoundPlayer();
        NAudio.Wave.WaveOut player;
        private void InitializeComponentOnUcCheckDevice()
        {
            userControlCheckDeviceOnFormMain.groupBoxLeverOnUcCheckDevice.Visible = false;
            userControlCheckDeviceOnFormMain.groupBoxDoorrOnUcCheckDevice.Visible = false;
            userControlCheckDeviceOnFormMain.groupBoxAirPuffOnUcCheckDevice.Visible = false;
            // ユーザ・コントロール: VisibleChanged
            userControlCheckDeviceOnFormMain.VisibleChanged += new System.EventHandler(userControlCheckDeviceOnFormMain_VisibleChanged);

            userControlCheckDeviceOnFormMain.buttonMainScreenOnUcCheckDevice.Click += new System.EventHandler(buttonMainScreenOnUcCheckDevice_Click);
            // Door.Openボタン
            userControlCheckDeviceOnFormMain.buttonOpenOnGpRoomOnUcCheckDevice.Click += (object sender, EventArgs e) =>
            {
                userControlCheckDeviceOnFormMain.textBoxDoutDoorResultOnGpDoorOnUcCheckDevice.Text = "none";
                OpSubOpenDoor();
            };
            // Door.Closeボタン
            userControlCheckDeviceOnFormMain.buttonCloseOnGpDoorOnUcCheckDevice.Click += (object sender, EventArgs e) =>
            {
                userControlCheckDeviceOnFormMain.textBoxDoutDoorResultOnGpDoorOnUcCheckDevice.Text = "none";
                OpSubCloseDoor();
            };
            // Door.Stopボタン
            userControlCheckDeviceOnFormMain.buttonStopOnGpRoomOnUcCheckDevice.Click += (object sender, EventArgs e) =>
            {
                userControlCheckDeviceOnFormMain.textBoxDoutDoorResultOnGpDoorOnUcCheckDevice.Text = "none";
                OpSubStopDoor();
            };
            // Lever.Inボタン
            userControlCheckDeviceOnFormMain.buttonOutputInOnGpLeverOnUcCheckDevice.Click += (object sender, EventArgs e) =>
            {
                userControlCheckDeviceOnFormMain.textBoxDoutLeverResultOnGpLeverOnUcCheckDevice.Text = "none";
                OpSubMoveLeverIn();
            };
            // Lever.Outボタン
            userControlCheckDeviceOnFormMain.buttonOutputOutOnGpLeverOnUcCheckDevice.Click += (object sender, EventArgs e) =>
            {
                userControlCheckDeviceOnFormMain.textBoxDoutLeverResultOnGpLeverOnUcCheckDevice.Text = "none";
                OpSubMoveLeverOut();
            };
            // Lever.Stopボタン
            userControlCheckDeviceOnFormMain.buttonStopOnGpLeverOnUcCheckDevice.Click += (object sender, EventArgs e) =>
            {
                userControlCheckDeviceOnFormMain.textBoxDoutLeverResultOnGpLeverOnUcCheckDevice.Text = "none";
                OpSubStopLever();
            };
            // Lever.Lamp.Onボタン
            userControlCheckDeviceOnFormMain.buttonLampOnGpLeverOnUcCheckDevice.Click += (object sender, EventArgs e) =>
            {
                OpSubSetLeverLampOn();
            };
            // Lever.Lamp.Offボタン
            userControlCheckDeviceOnFormMain.buttonLampOffGpLeverOnUcCheckDevice.Click += (object sender, EventArgs e) =>
            {
                OpSubSetLeverLampOff();
            };
            // AirPuff.Onボタン
            userControlCheckDeviceOnFormMain.buttonAirPuffOnGpAirPuffOnUcCheckDevice.Click += (object sender, EventArgs e) =>
            {
                OpSubAirPuffOn();
            };
            // AirPuff.Offボタン
            userControlCheckDeviceOnFormMain.buttonAirPuffOffGpAirPuffOnUcCheckDevice.Click += (object sender, EventArgs e) =>
            {
                OpSubAirPuffOff();
            };
            // eDoor.Openボタン
            userControlCheckDeviceOnFormMain.buttonEDoorOpen.Click += (object sender, EventArgs e) =>
            {
                int motorTime = int.Parse(userControlCheckDeviceOnFormMain.textBoxEDoorMotorSpeed.Text);
                eDoor.DoorOpen(motorTime);
            };
            // eDoor.Closeボタン
            userControlCheckDeviceOnFormMain.buttonEDoorClose.Click += (object sender, EventArgs e) =>
            {
                int motorTime = int.Parse(userControlCheckDeviceOnFormMain.textBoxEDoorMotorSpeed.Text);
                eDoor.DoorClose(motorTime);
            };
            // eDoor.Middleボタン
            userControlCheckDeviceOnFormMain.buttonEDoorMiddle.Click += (object sender, EventArgs e) =>
            {
                int openTime = int.Parse(userControlCheckDeviceOnFormMain.textBoxEDoorMiddleTime.Text);
                int motorTime = int.Parse(userControlCheckDeviceOnFormMain.textBoxEDoorMotorSpeed.Text);
                eDoor.DoorMiddleOpen(openTime, motorTime);
            };
            // eDoor.Text Middle
            userControlCheckDeviceOnFormMain.textBoxEDoorMiddleTime.KeyDown += (object sender, KeyEventArgs e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (int.TryParse(((TextBox)sender).Text, out int middleTime))
                    {
                        userControlCheckDeviceOnFormMain.trackBarEDoorMiddleTime.Value = middleTime;
                    }
                }
            };

            userControlCheckDeviceOnFormMain.textBoxEDoorMiddleTime.Leave += (object sender, EventArgs e) =>
            {
                if (int.TryParse(((TextBox)sender).Text, out int middleTime))
                {
                    userControlCheckDeviceOnFormMain.trackBarEDoorMiddleTime.Value = middleTime;
                }
            };
            // eDoor.Text MotorSpeed
            userControlCheckDeviceOnFormMain.textBoxEDoorMotorSpeed.KeyDown += (object sender, KeyEventArgs e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (int.TryParse(((TextBox)sender).Text, out int motorSpeed))
                    {
                        if (motorSpeed >= userControlCheckDeviceOnFormMain.trackBarEDoorMotorSpeed.Minimum &&
                        userControlCheckDeviceOnFormMain.trackBarEDoorMotorSpeed.Maximum >= motorSpeed)
                        {
                            userControlCheckDeviceOnFormMain.trackBarEDoorMotorSpeed.Value = motorSpeed;
                        }
                    }
                }
            };
            userControlCheckDeviceOnFormMain.textBoxEDoorMotorSpeed.Leave += (object sender, EventArgs e) =>
            {
                if (int.TryParse(((TextBox)sender).Text, out int motorSpeed))
                {
                    if (motorSpeed >= userControlCheckDeviceOnFormMain.trackBarEDoorMotorSpeed.Minimum &&
                    userControlCheckDeviceOnFormMain.trackBarEDoorMotorSpeed.Maximum >= motorSpeed)
                    {
                        userControlCheckDeviceOnFormMain.trackBarEDoorMotorSpeed.Value = motorSpeed;
                    }
                }
            };

            // eDoor.TrackBarMiddleTime
            userControlCheckDeviceOnFormMain.trackBarEDoorMiddleTime.Scroll += (object sender, EventArgs e) =>
            {
                userControlCheckDeviceOnFormMain.textBoxEDoorMiddleTime.Text = ((TrackBar)sender).Value.ToString();
            };
            // eDoor.TrackBarMotorSpeed
            userControlCheckDeviceOnFormMain.trackBarEDoorMotorSpeed.Scroll += (object sender, EventArgs e) =>
            {
                userControlCheckDeviceOnFormMain.textBoxEDoorMotorSpeed.Text = ((TrackBar)sender).Value.ToString();
            };
            // RoomLamp.Onボタン
            userControlCheckDeviceOnFormMain.buttonLampOnGpRoomOnUcCheckDevice.Click += (object sender, EventArgs e) =>
            {
                OpSubSetRoomLampOn();
            };
            // RoomLamp.Offボタン
            userControlCheckDeviceOnFormMain.buttonLampOffGpRoomOnUcCheckDevice.Click += (object sender, EventArgs e) =>
            {
                OpSubSetRoomLampOff();
            };
            // FeedLamp.Onボタン
            userControlCheckDeviceOnFormMain.buttonFeedLampOnGpFeedOnCheckDevice.Click += (object sender, EventArgs e) =>
            {
                OpSubSetFeedLampOn();
            };
            // FeedLamp.Offボタン
            userControlCheckDeviceOnFormMain.buttonFeedLampOffGpFeedOnCheckDevice.Click += (object sender, EventArgs e) =>
            {
                OpSubSetFeedLampOff();
            };
            // FeedLamp.Onボタン
            userControlCheckDeviceOnFormMain.buttonFeed2LampOnGpFeedOnCheckDevice.Click += (object sender, EventArgs e) =>
            {
                //どちらが押されたか判別する、むしろいらない？
                //OpSubSetFeedLampOn();
            };
            // FeedLamp.Offボタン
            userControlCheckDeviceOnFormMain.buttonFeed2LampOffGpFeedOnCheckDevice.Click += (object sender, EventArgs e) =>
            {
                //OpSubSetFeedLampOff();
            };
            // Feed.Onボタン
            userControlCheckDeviceOnFormMain.buttonOnOnGpFeedOnUcCheckDevice.Click += (object sender, EventArgs e) =>
            {
                DevCmdPkt devCmdPktObj = new DevCmdPkt();

                if (userControlCheckDeviceOnFormMain.radioButtonForwardOnGpFeedOnUcCheckDevice.Checked)
                {
                    devCmdPktObj.DevCmdVal = EDevCmd.FeedForward;
                }
                else
                {
                    devCmdPktObj.DevCmdVal = EDevCmd.FeedReverse;
                }
                if (userControlCheckDeviceOnFormMain.radioButtonContinuousOnGpFeedOnUcCheckDevice.Checked)
                {
                    // 連続動作
                    devCmdPktObj.iParam[0] = -1;
                }
                else
                {
                    // One shot動作
                    devCmdPktObj.iParam[0] = (int)userControlCheckDeviceOnFormMain.numericUpDownOnoShoTimeOnGpFeedOnUcCheckDevice.Value;
                }
                devFeed.CommandEnqueue(devCmdPktObj);
                //concurrentQueueDevCmdPktFeed.Enqueue(devCmdPktObj);
            };
            // Feed.Offボタン
            userControlCheckDeviceOnFormMain.buttonOffOnGpFeedOnUcCheckDevice.Click += (object sender, EventArgs e) =>
            {
                DevCmdPkt devCmdPktObj = new DevCmdPkt();

                devCmdPktObj.DevCmdVal = EDevCmd.FeedStop;
                devFeed.CommandEnqueue(devCmdPktObj);
                //concurrentQueueDevCmdPktFeed.Enqueue(devCmdPktObj);
            };

            // Feed.Onボタン
            userControlCheckDeviceOnFormMain.buttonOnOnGpFeed2OnUcCheckDevice.Click += (object sender, EventArgs e) =>
            {
                DevCmdPkt devCmdPktObj = new DevCmdPkt();

                if (userControlCheckDeviceOnFormMain.radioButtonForwardOnGpFeed2OnUcCheckDevice.Checked)
                {
                    devCmdPktObj.DevCmdVal = EDevCmd.FeedForward;
                }
                else
                {
                    devCmdPktObj.DevCmdVal = EDevCmd.FeedReverse;
                }
                if (userControlCheckDeviceOnFormMain.radioButtonContinuousOnGpFeed2OnUcCheckDevice.Checked)
                {
                    // 連続動作
                    devCmdPktObj.iParam[0] = -1;
                }
                else
                {
                    // One shot動作
                    devCmdPktObj.iParam[0] = (int)userControlCheckDeviceOnFormMain.numericUpDownOnoShoTimeOnGpFeed2OnUcCheckDevice.Value;
                }
                devFeed2.CommandEnqueue(devCmdPktObj);
            };
            // Feed.Offボタン
            userControlCheckDeviceOnFormMain.buttonOffOnGpFeed2OnUcCheckDevice.Click += (object sender, EventArgs e) =>
            {
                DevCmdPkt devCmdPktObj = new DevCmdPkt();

                devCmdPktObj.DevCmdVal = EDevCmd.FeedStop;
                devFeed2.CommandEnqueue(devCmdPktObj);
            };
            userControlCheckDeviceOnFormMain.buttonClearOnGpIdCodeOnUcCheckDevice.Click += (object sender, EventArgs e) =>
            {
                userControlCheckDeviceOnFormMain.textBoxIdCodeOnGpIdCodeOnUcCheckDevice.ResetText();
            };

            userControlCheckDeviceOnFormMain.buttonExecuteOnGpTouchPanelOnUcCheckDevice.Click += new System.EventHandler(this.buttonExecuteOnGpTouchPanelOnUcCheckDevice_Click);

            // FeedOn時間 
            userControlCheckDeviceOnFormMain.numericUpDownOnoShoTimeOnGpFeedOnUcCheckDevice.Leave += (object sender, EventArgs e) =>
            {
                if (userControlCheckDeviceOnFormMain.numericUpDownOnoShoTimeOnGpFeedOnUcCheckDevice.Text == "")
                {
                    userControlCheckDeviceOnFormMain.numericUpDownOnoShoTimeOnGpFeedOnUcCheckDevice.Text = userControlCheckDeviceOnFormMain.numericUpDownOnoShoTimeOnGpFeedOnUcCheckDevice.Value.ToString();
                }
            };
            // FeedOn時間 
            userControlCheckDeviceOnFormMain.numericUpDownOnoShoTimeOnGpFeedOnUcCheckDevice.Leave += (object sender, EventArgs e) =>
            {
                if (userControlCheckDeviceOnFormMain.numericUpDownOnoShoTimeOnGpFeed2OnUcCheckDevice.Text == "")
                {
                    userControlCheckDeviceOnFormMain.numericUpDownOnoShoTimeOnGpFeed2OnUcCheckDevice.Text = userControlCheckDeviceOnFormMain.numericUpDownOnoShoTimeOnGpFeed2OnUcCheckDevice.Value.ToString();
                }
            };
            // SizeOfShapeInPixel
            userControlCheckDeviceOnFormMain.numericUpDownSizeOfShapeInPixelOnGpTouchPanelOnUcCheckDevice.Leave += (object sender, EventArgs e) =>
            {
                if (userControlCheckDeviceOnFormMain.numericUpDownSizeOfShapeInPixelOnGpTouchPanelOnUcCheckDevice.Text == "")
                {
                    userControlCheckDeviceOnFormMain.numericUpDownSizeOfShapeInPixelOnGpTouchPanelOnUcCheckDevice.Text = userControlCheckDeviceOnFormMain.numericUpDownSizeOfShapeInPixelOnGpTouchPanelOnUcCheckDevice.Value.ToString();
                }
            };

            userControlCheckDeviceOnFormMain.buttonPlayOnGpSoundOnUcCheckDevice.Click += (object sender, EventArgs e) =>
            {
                if (userControlCheckDeviceOnFormMain.textBoxFileOnGpSoundOnUcCheckDevice.Text == "")
                {
                    MessageBox.Show("Can not load file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //				OpPlaySoundOfEnd();
                //				SoundPlayerObj= new SoundPlayer(@"C:\user\ono\Work\Jitsuchuuken\WAV_file\se01_01(ピンポン).wav");
                //				SoundPlayerObj= new SoundPlayer(@"C:\user\ono\Work\Jitsuchuuken\WAV_file\a.wav");
                try
                {
                    //					SoundPlayerCheckDevice.SoundLocation = @"C:\user\ono\Work\Jitsuchuuken\WAV_file\se01_01(ピンポン).wav");
                    //SoundPlayerCheckDevice.SoundLocation = userControlCheckDeviceOnFormMain.textBoxFileOnGpSoundOnUcCheckDevice.Text;
                    PlayMedia(userControlCheckDeviceOnFormMain.textBoxFileOnGpSoundOnUcCheckDevice.Text, out player);
                    //SoundPlayerCheckDevice.Load();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    MessageBox.Show("Can not load file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    // 非同期で再生
                    //SoundPlayerCheckDevice.PlayLooping();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    MessageBox.Show("Can not play file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            };
            userControlCheckDeviceOnFormMain.buttonStopOnGpSoundOnUcCheckDevice.Click += (object sender, EventArgs e) =>
            {
                //				OpPlaySoundOfReward();

                player?.Stop();

                if (SoundPlayerCheckDevice != null)
                {
                    SoundPlayerCheckDevice.Stop();
                }
            };
            userControlCheckDeviceOnFormMain.buttonFileOnGpSoundOnUcCheckDevice.Click += (object sender, EventArgs e) =>
            {
                // 最初のファイル名を指定する(最初にに「ファイル名」で表示される文字列を指定)
                userControlCheckDeviceOnFormMain.openFileDialogFileOnGpSoundOnUcCheckDevice.FileName = "";
                // 最初に表示されるフォルダを指定 (指定しない（空の文字列）の時は、現在のディレクトリを表示)
                //	userControlCheckDeviceOnFormMain.openFileDialogFileOnGpSoundOnUcCheckDevice.InitialDirectory = @"C:\";
                userControlCheckDeviceOnFormMain.openFileDialogFileOnGpSoundOnUcCheckDevice.InitialDirectory = "";
                // [ファイルの種類]に表示される選択肢を指定 (指定しないとすべてのファイルを表示)
                userControlCheckDeviceOnFormMain.openFileDialogFileOnGpSoundOnUcCheckDevice.Filter = "Audio Files(*.wav;*.mp3)|*.wav;*.mp3|WAV(*.wav)|*.wav|MP3(*.mp3)|*.mp3";
                // [ファイルの種類]ではじめに選択されるものを指定する (1番目の「WAVファイル」が選択されているようにする)
                userControlCheckDeviceOnFormMain.openFileDialogFileOnGpSoundOnUcCheckDevice.FilterIndex = 1;
                // ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
                userControlCheckDeviceOnFormMain.openFileDialogFileOnGpSoundOnUcCheckDevice.RestoreDirectory = true;
                // 存在しないファイルの名前が指定されたとき警告を表示(デフォルトでTrueなので指定する必要はない)
                userControlCheckDeviceOnFormMain.openFileDialogFileOnGpSoundOnUcCheckDevice.CheckFileExists = true;
                // 存在しないパスが指定されたとき警告を表示する(デフォルトでTrueなので指定する必要はない)
                userControlCheckDeviceOnFormMain.openFileDialogFileOnGpSoundOnUcCheckDevice.CheckPathExists = true;

                if (userControlCheckDeviceOnFormMain.openFileDialogFileOnGpSoundOnUcCheckDevice.ShowDialog() == DialogResult.OK)
                {
                    //OKボタンがクリックされた時、ファイル名を取得
                    userControlCheckDeviceOnFormMain.textBoxFileOnGpSoundOnUcCheckDevice.Text = userControlCheckDeviceOnFormMain.openFileDialogFileOnGpSoundOnUcCheckDevice.FileName;
                }
            };
            userControlCheckDeviceOnFormMain.buttonFileOnGpTouchPanelOnUcCheckDevice.Click += (object sender, EventArgs e) =>
            {
                // 最初のファイル名を指定する(最初にに「ファイル名」で表示される文字列を指定)
                userControlCheckDeviceOnFormMain.openFileDialogFileOnGpTouchPanelOnUcCheckDevice.FileName = "";
                // 最初に表示されるフォルダを指定 (指定しない（空の文字列）の時は、現在のディレクトリを表示)
                //	userControlCheckDeviceOnFormMain.openFileDialogFileOnGpTouchPanelOnUcCheckDevice.InitialDirectory = @"C:\";
                userControlCheckDeviceOnFormMain.openFileDialogFileOnGpTouchPanelOnUcCheckDevice.InitialDirectory = "";
                // [ファイルの種類]に表示される選択肢を指定 (指定しないとすべてのファイルを表示)
                userControlCheckDeviceOnFormMain.openFileDialogFileOnGpTouchPanelOnUcCheckDevice.Filter = "Image Files(*.jpg;*.png;*.bmp)|*.jpg;*.png;*.bmp|JPG(*.jpg)|*.jpg|PNG(*.png)|*.png|BMP(*.bmp)|*.bmp";
                // [ファイルの種類]ではじめに選択されるものを指定する (1番目の「JEGファイル」が選択されているようにする)
                userControlCheckDeviceOnFormMain.openFileDialogFileOnGpTouchPanelOnUcCheckDevice.FilterIndex = 1;
                // ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
                userControlCheckDeviceOnFormMain.openFileDialogFileOnGpTouchPanelOnUcCheckDevice.RestoreDirectory = true;
                // 存在しないファイルの名前が指定されたとき警告を表示(デフォルトでTrueなので指定する必要はない)
                userControlCheckDeviceOnFormMain.openFileDialogFileOnGpTouchPanelOnUcCheckDevice.CheckFileExists = true;
                // 存在しないパスが指定されたとき警告を表示する(デフォルトでTrueなので指定する必要はない)
                userControlCheckDeviceOnFormMain.openFileDialogFileOnGpTouchPanelOnUcCheckDevice.CheckPathExists = true;

                if (userControlCheckDeviceOnFormMain.openFileDialogFileOnGpTouchPanelOnUcCheckDevice.ShowDialog() == DialogResult.OK)
                {
                    //OKボタンがクリックされた時、ファイル名を取得
                    userControlCheckDeviceOnFormMain.textBoxFileOnGpTouchPanelOnUcCheckDevice.Text = userControlCheckDeviceOnFormMain.openFileDialogFileOnGpTouchPanelOnUcCheckDevice.FileName;
                }
            };

            // 表示が変わったとき
            userControlCheckDeviceOnFormMain.VisibleChanged += (sender, e) =>
            {
                const int mergin = 5;
                userControlCheckDeviceOnFormMain.groupBoxTouchPanelImage.Height = 408;
                if ((opImage.WidthOfWholeArea * 1000 / opImage.HeightOfWholeArea * 1000) == (16 * 1000 / 9 * 1000))
                {
                    userControlCheckDeviceOnFormMain.groupBoxTouchPanelImage.Width = userControlCheckDeviceOnFormMain.groupBoxTouchPanel.Width - mergin;
                    userControlCheckDeviceOnFormMain.groupBoxTouchPanelImage.Height = (int)(userControlCheckDeviceOnFormMain.pictureBoxTouchPanelOnGpTouchPanelOnUcCheckDevice.Width * (9.0 / 16.0));
                    //userControlCheckDeviceOnFormMain.pictureBoxTouchPanelOnGpTouchPanelOnUcCheckDevice.Height = userControlCheckDeviceOnFormMain.pictureBoxTouchPanelOnGpTouchPanelOnUcCheckDevice.Width * (3 / 4);
                }
                else
                {
                    userControlCheckDeviceOnFormMain.groupBoxTouchPanelImage.Height = (int)(userControlCheckDeviceOnFormMain.pictureBoxTouchPanelOnGpTouchPanelOnUcCheckDevice.Width * (3.0 / 4.0));
                    //userControlCheckDeviceOnFormMain.pictureBoxTouchPanelOnGpTouchPanelOnUcCheckDevice.Height = userControlCheckDeviceOnFormMain.pictureBoxTouchPanelOnGpTouchPanelOnUcCheckDevice.Width * (9 / 16);

                    //int width = userControlCheckDeviceOnFormMain.splitContainer1.Size.Width;
                    //userControlCheckDeviceOnFormMain.splitContainer1.SplitterDistance = width * 3 / 4;
                }
            };
            userControlCheckDeviceOnFormMain.buttonIpCamTestOnUcCheckDevice.Click += (sender, e) => 
            {
                FormIpCam formIpCam=new FormIpCam(this);
                formIpCam.ShowDialog();
            };
        }

        private void ButtonIpCamTestOnUcCheckDevice_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        //	userControlCheckDeviceOnFormMain
        private void buttonMainScreenOnUcCheckDevice_Click(object sender, EventArgs e)
        {
            EndUcCheckDevice();
            // userControlMainOnFormMain:表示
            VisibleUcMain();
        }
        private Rectangle rectangleShape = new Rectangle(0, 0, 1, 1);

        private void buttonExecuteOnGpTouchPanelOnUcCheckDevice_Click(object sender, EventArgs e)
        {
            ECpShape CpShapeSelected;
            ECpStep CpStepSelected;
            DevCmdPkt devCmdPktObj = new DevCmdPkt();
            int iSizeOfShapeInPixel;
            bool boolDrawWrongShape = true;


            if (userControlCheckDeviceOnFormMain.comboBoxTypeOfCommandOnGpTouchPanelOnUcCheckDevice.SelectedItem.ToString() == ECpCommand.ClearScreen.ToString())
            {
                opImage.DrawBackColor(Color.Black);
                devCmdPktObj.DevCmdVal = EDevCmd.TouchPanelStop;
                concurrentQueueDevCmdPktTouchPanel.Enqueue(devCmdPktObj);
            }
            else if (userControlCheckDeviceOnFormMain.comboBoxTypeOfCommandOnGpTouchPanelOnUcCheckDevice.SelectedItem.ToString() == ECpCommand.DrawBackColor.ToString())
            {
                opImage.DrawBackColor();
                devCmdPktObj.DevCmdVal = EDevCmd.TouchPanelStop;
                concurrentQueueDevCmdPktTouchPanel.Enqueue(devCmdPktObj);
            }
            else if (userControlCheckDeviceOnFormMain.comboBoxTypeOfCommandOnGpTouchPanelOnUcCheckDevice.SelectedItem.ToString() == ECpCommand.DrawImage.ToString())
            {
                if (opImage.DrawImage(userControlCheckDeviceOnFormMain.textBoxFileOnGpTouchPanelOnUcCheckDevice.Text) != true)
                {
                    MessageBox.Show("Error in draw image", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else if (userControlCheckDeviceOnFormMain.comboBoxTypeOfCommandOnGpTouchPanelOnUcCheckDevice.SelectedItem.ToString() == ECpCommand.TouchAny.ToString())
            {
                devCmdPktObj.DevCmdVal = EDevCmd.TouchPanelTouchAny;
                concurrentQueueDevCmdPktTouchPanel.Enqueue(devCmdPktObj);
            }
            else if (userControlCheckDeviceOnFormMain.comboBoxTypeOfCommandOnGpTouchPanelOnUcCheckDevice.SelectedItem.ToString() == ECpCommand.TouchOne.ToString())
            {
                // Size of shape in pixelを取得
                iSizeOfShapeInPixel = (int)userControlCheckDeviceOnFormMain.numericUpDownSizeOfShapeInPixelOnGpTouchPanelOnUcCheckDevice.Value;
                // Stepを取得
                if (opImage.ConvertStringToCpStep(
                        userControlCheckDeviceOnFormMain.comboBoxSizeOfShapeInStepOnGpTouchPanelOnUcCheckDevice.SelectedItem.ToString(),
                        out CpStepSelected) != true)
                {
                    MessageBox.Show("Error in size of shape in step", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // 図形タイプを取得
                if (opImage.ConvertStringToCpShape(
                        userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.SelectedItem.ToString(),
                        out CpShapeSelected) != true)
                {
                    MessageBox.Show("Error in type of Shape", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    opImage.CorrectShape = CpShapeSelected;
                }

                if (opImage.SetParamOfShapeOpeImage(
                    userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.SelectedItem.ToString(),
                    userControlCheckDeviceOnFormMain.comboBoxSizeOfShapeInStepOnGpTouchPanelOnUcCheckDevice.SelectedItem.ToString(),
                    iSizeOfShapeInPixel) != true)
                {
                    MessageBox.Show("Error in setting", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //opImage.MakePointOfCorrectShapeOpeImage();
                //opImage.MakePathOfCorrectShapeOpeImage();
                opImage.MakeCorrectShape();

                if (ECpShape.Random != CpShapeSelected && ECpShape.Image != opImage.ShapeOpeImageCorrectShape.Shape)
                    opImage.ShapeOpeImageCorrectShape.Shape = CpShapeSelected;

                if (opImage.DrawBackColor() != true)
                {
                    MessageBox.Show("Error in draw back color", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (opImage.DrawShapeOpeImage(opImage.ShapeOpeImageCorrectShape, true) != true)
                {
                    MessageBox.Show("Error in draw correct shape", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                OpSubStartToTouchCorrectOnTouchPanel();
            }
            else if (userControlCheckDeviceOnFormMain.comboBoxTypeOfCommandOnGpTouchPanelOnUcCheckDevice.SelectedItem.ToString() == ECpCommand.TouchTwo.ToString())
            {
                // Size of shape in pixelを取得
                iSizeOfShapeInPixel = (int)userControlCheckDeviceOnFormMain.numericUpDownSizeOfShapeInPixelOnGpTouchPanelOnUcCheckDevice.Value;
                // Stepを取得
                if (opImage.ConvertStringToCpStep(
                        userControlCheckDeviceOnFormMain.comboBoxSizeOfShapeInStepOnGpTouchPanelOnUcCheckDevice.SelectedItem.ToString(),
                        out CpStepSelected) != true)
                {
                    MessageBox.Show("Error in size of shape in step", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // 図形タイプを取得
                if (opImage.ConvertStringToCpShape(
                        userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.SelectedItem.ToString(),
                        out CpShapeSelected) != true)
                {
                    MessageBox.Show("Error in type of Shape", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    opImage.CorrectShape = CpShapeSelected;
                }

                if (opImage.SetParamOfShapeOpeImage(
                    userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.SelectedItem.ToString(),
                    userControlCheckDeviceOnFormMain.comboBoxSizeOfShapeInStepOnGpTouchPanelOnUcCheckDevice.SelectedItem.ToString(),
                    iSizeOfShapeInPixel) != true)
                {
                    MessageBox.Show("Error in setting", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                try
                {

                    //opImage.MakePointOfCorrectShapeOpeImage();
                    opImage.MakeCorrectShape();
                    if (ECpShape.Random != CpShapeSelected && ECpShape.Image != opImage.ShapeOpeImageCorrectShape.Shape)
                        opImage.ShapeOpeImageCorrectShape.Shape = CpShapeSelected;

                    //opImage.MakePathOfCorrectShapeOpeImage();

                    opImage.ResetWrongShapePoint();
                    opImage.MakeIncorrectShape();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                    //throw;
                }
                //for (int i = 0; i < preferencesDatOriginal.IncorrectNum; i++)
                //{
                //    if (opImage.MakePointOfWrongShapeOpeImage() != true)
                //    {
                //        opImage.DeletePathOfCorrectShapeOpeImage();
                //        MessageBox.Show("Error in make point of wrong shape", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //        boolDrawWrongShape = false;
                //    }
                //}
                if (opImage.DrawBackColor() != true)
                {
                    MessageBox.Show("Error in draw back color", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (opImage.DrawShapeOpeImage(opImage.ShapeOpeImageCorrectShape, true) != true)
                {
                    MessageBox.Show("Error in draw correct shape", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (boolDrawWrongShape == true)
                {
                    foreach (ShapeObject n in opImage.IncorrectShapes)
                    {
                        _ = opImage.DrawShapeOpeImage(n);
                    }

                    //foreach(Point poicShape in opImage.PointOpeImageCenterOfWrongShapeList)
                    //               {
                    //	opImage.DrawWrongShapeOpeImage(poicShape, true);
                    //}

                    //if (opImage.DrawShapeOpeImage(opImage.PointOpeImageCenterOfWrongShape, true) != true)
                    //{
                    //	MessageBox.Show("Error in draw wrong shape", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //	return;
                    //}
                }
                OpSubStartToTouchCorrectOnTouchPanel();
            }
            else if (userControlCheckDeviceOnFormMain.comboBoxTypeOfCommandOnGpTouchPanelOnUcCheckDevice.SelectedItem.ToString() == ECpCommand.TouchEnd.ToString())
            {
                OpSubEndToTouchCorrectOnTouchPanel();
            }
            else if (userControlCheckDeviceOnFormMain.comboBoxTypeOfCommandOnGpTouchPanelOnUcCheckDevice.SelectedItem.ToString() == ECpCommand.TouchAnyShape.ToString())
            {
                // Size of shape in pixelを取得
                iSizeOfShapeInPixel = (int)userControlCheckDeviceOnFormMain.numericUpDownSizeOfShapeInPixelOnGpTouchPanelOnUcCheckDevice.Value;
                // Stepを取得
                if (opImage.ConvertStringToCpStep(
                        userControlCheckDeviceOnFormMain.comboBoxSizeOfShapeInStepOnGpTouchPanelOnUcCheckDevice.SelectedItem.ToString(),
                        out CpStepSelected) != true)
                {
                    MessageBox.Show("Error in size of shape in step", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // 図形タイプを取得
                if (opImage.ConvertStringToCpShape(
                        userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.SelectedItem.ToString(),
                        out CpShapeSelected) != true)
                {
                    MessageBox.Show("Error in type of Shape", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    opImage.CorrectShape = CpShapeSelected;
                }

                if (opImage.SetParamOfShapeOpeImage(
                    userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.SelectedItem.ToString(),
                    userControlCheckDeviceOnFormMain.comboBoxSizeOfShapeInStepOnGpTouchPanelOnUcCheckDevice.SelectedItem.ToString(),
                    iSizeOfShapeInPixel) != true)
                {
                    MessageBox.Show("Error in setting", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                try
                {
                    opImage.MakeAnyShape();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                if (opImage.DrawBackColor() != true)
                {
                    MessageBox.Show("Error in draw back color", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //if (opImage.DrawShapeOpeImage(opImage.ShapeOpeImageCorrectShape, true) != true)
                //{
                //    MessageBox.Show("Error in draw correct shape", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}
                if (boolDrawWrongShape == true)
                {
                    foreach (ShapeObject n in opImage.EpisodeShapes)
                    {
                        _ = opImage.DrawShapeOpeImage(n);
                    }
                }
                OpSubStartToTouchAnyCorrectOnTouchPanel();
            }
            else if (userControlCheckDeviceOnFormMain.comboBoxTypeOfCommandOnGpTouchPanelOnUcCheckDevice.SelectedItem.ToString() == ECpCommand.DrawIndicator.ToString())
            {
                if (opImage.DrawBackColor() != true)
                {
                    MessageBox.Show("Error in draw back color", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // Size of shape in pixelを取得
                iSizeOfShapeInPixel = (int)userControlCheckDeviceOnFormMain.numericUpDownSizeOfShapeInPixelOnGpTouchPanelOnUcCheckDevice.Value;
                // Stepを取得
                if (opImage.ConvertStringToCpStep(
                        userControlCheckDeviceOnFormMain.comboBoxSizeOfShapeInStepOnGpTouchPanelOnUcCheckDevice.SelectedItem.ToString(),
                        out CpStepSelected) != true)
                {
                    MessageBox.Show("Error in size of shape in step", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (opImage.SetParamOfShapeOpeImage(
                        userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.SelectedItem.ToString(),
                        userControlCheckDeviceOnFormMain.comboBoxSizeOfShapeInStepOnGpTouchPanelOnUcCheckDevice.SelectedItem.ToString(),
                        iSizeOfShapeInPixel) != true)
                {
                    MessageBox.Show("Error in setting", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                ShapeObject so = new ShapeObject();
                so.Shape = ECpShape.SvgCircle;
                so.Point = new Point(100, 100);
                so.ShapeColor = Color.AliceBlue;
                opImage.ShapeOpeImageCorrectShape = so;

                // ShapeSizeの1/3
                int drawWidth = opImage.OpeImageSizeOfShapeInPixel / 3;
                int drawHeight = opImage.OpeImageSizeOfShapeInPixel / 3;
                if (!System.IO.File.Exists(preferencesDatOriginal.TestIndicatorSvgPath))
                {
                    preferencesDatOriginal.TestIndicatorSvgPath = @".\svg\trefoil.svg";
                }
                opImage.DrawShapeOpeImage(so);
                opImage.DrawMarker(preferencesDatOriginal.IndicatorPosition);
                //Bitmap bmp = opImage.LoadSvgImage(preferencesDatOriginal.TestIndicatorSvgPath, drawWidth, drawHeight, Color.YellowGreen);
                //opImage.DrawImage(bmp, new Point(opImage.WidthOfWholeArea - drawWidth - shapeOffset, opImage.HeightOfWholeArea - drawHeight - shapeOffset));
                //opImage.UpdateCanvas();
            }
            else
            {
                MessageBox.Show("Error in Command", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
#if false
		private void buttonExecuteOnGpTouchPanelOnUcCheckDevice_Click(object sender, EventArgs e)
		{
			Point l_pointPoint = new Point();
			Random l_randomValue = new System.Random();
			Color l_colorBackColor = Color.Green;
			Color l_colorShapeColor = Color.White;
			SolidBrush solidbrushBrush = new SolidBrush(l_colorShapeColor);

			// 図形: 円
			cpShapeType = CpShape.Circle;
			l_pointPoint.X = l_randomValue.Next(0, formSub.pictureBoxOnFormSub.Width);
			l_pointPoint.Y = l_randomValue.Next(0, formSub.pictureBoxOnFormSub.Height);
			iSizeofShape = l_randomValue.Next(50, 101);

			//			this.formsubFormSub.pictureBoxOnFormSub.Image = null;
			userControlCheckDeviceOnFormMain.pictureBoxTouchPanelOnGpTouchPanelOnUcCheckDevice.Image = null;
			//ImageオブジェクトのGraphicsオブジェクトを作成する
			Graphics l_graphicsObj = Graphics.FromImage(bitmapCanvas);

			graphicsPathShape= new System.Drawing.Drawing2D.GraphicsPath();
			graphicsPathShape.AddEllipse(l_pointPoint.X - iSizeofShape / 2, l_pointPoint.Y - iSizeofShape / 2,
										iSizeofShape, iSizeofShape);

			l_graphicsObj.Clear(l_colorBackColor);
			l_graphicsObj.FillEllipse(solidbrushBrush, l_pointPoint.X - iSizeofShape / 2, l_pointPoint.Y - iSizeofShape / 2,
										iSizeofShape, iSizeofShape);
			//			l_graphicsObj.DrawEllipse(Pens.White, l_pointPoint.X - l_iSize/2, l_pointPoint.Y - l_iSize/2,
			//										l_iSize, l_iSize);
			l_graphicsObj.Dispose();
			userControlCheckDeviceOnFormMain.pictureBoxTouchPanelOnGpTouchPanelOnUcCheckDevice.SizeMode = PictureBoxSizeMode.StretchImage;
			// PictureBoxSizeMode.StretchImage: PictureBox 内のイメージのサイズは、PictureBox のサイズに合うように調整されます。
			// PictureBoxSizeMode.Zoom: イメージのサイズは、サイズ比率を維持したままで拡大または縮小します。
			this.formSub.pictureBoxOnFormSub.Image = bitmapCanvas;
			userControlCheckDeviceOnFormMain.pictureBoxTouchPanelOnGpTouchPanelOnUcCheckDevice.Image = bitmapCanvas;
		}

#endif
        private void userControlCheckDeviceOnFormMain_VisibleChanged(object sender, EventArgs e)
        {
            if (userControlCheckDeviceOnFormMain.Visible == true)
            {
                // メイン・ステート: 初期化
                mainStateVal = EMainState.Init;
                // 動作モード: CheckDevice
                opeModeTypeVal = EOpeModeType.CheckDevice;
                // タイマ開始
                timerOperation.Start();
                // タッチ・パネル初期化
                // FormSubのMouseClickデリゲートを無効とする
                formSub.boolEnableCallBackTouchPoint.Value = false;
                formSub.callbackTouchPoint = (point) =>
                {
                    // タッチ座標をキューへ入力
                    concurrentQueueFromTouchPanel.Enqueue(point);
                };
                // FormSubのMouseClickデリゲートを有効とする
                formSub.boolEnableCallBackTouchPoint.Value = true;
                // シリアルの受信デリゲートを無効にする
                serialHelperPort.isEnableCallBackReceivedData.Value = false;
                callbackReceivedDataSub = (str) =>
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        // ID code表示
                        //						userControlCheckDeviceOnFormMain.textBoxIdCodeOnGpIdCodeOnUcCheckDevice.Text = str;
                        userControlCheckDeviceOnFormMain.textBoxIdCodeOnGpIdCodeOnUcCheckDevice.AppendText(str + Environment.NewLine);
                        userControlCheckDeviceOnFormMain.textBoxIdCodeOnGpIdCodeOnUcCheckDevice.Focus();
                    }));
                    // ID Code: 重複検査クリア
                    mIdCode0.Value = "";
                };
                // シリアルの受信デリゲートを有効にする
                serialHelperPort.isEnableCallBackReceivedData.Value = true;
                // TypeOfShapeコンボ・ボックス選択候補設定
                userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.Items.Clear();
                userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpShape.Circle.ToString());
                userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpShape.Rectangle.ToString());
                userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpShape.Triangle.ToString());
                userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpShape.Star.ToString());
                //userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpShape.Random.ToString());

                List<ECpShape> nonExistShapeList = OpeImage.CheckExistSvgShape();
                foreach (var n in nonExistShapeList)
                {
                    userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.Items.Add(n.ToString());
                }

                //userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpShape.SvgSquare.ToString());
                //userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpShape.SvgCircle.ToString());
                //userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpShape.SvgRectangle.ToString());
                //userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpShape.SvgTriangle.ToString());
                //userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpShape.SvgStar.ToString());
                //userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpShape.SvgScalene.ToString());
                //userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpShape.SvgPentagon.ToString());
                //userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpShape.SvgRight.ToString());
                //userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpShape.SvgTrapeze.ToString());
                //userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpShape.SvgKite.ToString());
                //userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpShape.SvgPolygon.ToString());
                //userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpShape.SvgParallelogram.ToString());
                //userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpShape.SvgEllipse.ToString());
                //userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpShape.SvgTrefoil.ToString());
                //userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpShape.SvgSemiCircle.ToString());
                //userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpShape.SvgHexagon.ToString());
                //userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpShape.SvgCresent.ToString());
                //userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpShape.SvgOctagon.ToString());
                //userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpShape.SvgCross.ToString());
                //userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpShape.SvgRing.ToString());
                //userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpShape.SvgPic.ToString());
                //userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpShape.SvgHeart.ToString());
                //userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpShape.SvgArrow.ToString());
                //userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpShape.SvgQuatrefoil.ToString());
                //userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpShape.SvgRhombus.ToString());



                userControlCheckDeviceOnFormMain.comboBoxTypeOfShapeOnGpTouchPanelOnUcCheckDevice.SelectedIndex = 0;
                // SizeOfShapeInStepコンボ・ボックス選択候補設定
                userControlCheckDeviceOnFormMain.comboBoxSizeOfShapeInStepOnGpTouchPanelOnUcCheckDevice.Items.Clear();
                userControlCheckDeviceOnFormMain.comboBoxSizeOfShapeInStepOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpStep.Step1.ToString());
                userControlCheckDeviceOnFormMain.comboBoxSizeOfShapeInStepOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpStep.Step2.ToString());
                userControlCheckDeviceOnFormMain.comboBoxSizeOfShapeInStepOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpStep.Step3.ToString());
                userControlCheckDeviceOnFormMain.comboBoxSizeOfShapeInStepOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpStep.Step4.ToString());
                userControlCheckDeviceOnFormMain.comboBoxSizeOfShapeInStepOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpStep.Step5.ToString());
                userControlCheckDeviceOnFormMain.comboBoxSizeOfShapeInStepOnGpTouchPanelOnUcCheckDevice.SelectedIndex = 0;
                // Commandコンボ・ボックス選択候補設定
                userControlCheckDeviceOnFormMain.comboBoxTypeOfCommandOnGpTouchPanelOnUcCheckDevice.Items.Clear();
                userControlCheckDeviceOnFormMain.comboBoxTypeOfCommandOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpCommand.ClearScreen.ToString());
                userControlCheckDeviceOnFormMain.comboBoxTypeOfCommandOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpCommand.DrawBackColor.ToString());
                userControlCheckDeviceOnFormMain.comboBoxTypeOfCommandOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpCommand.DrawImage.ToString());
                userControlCheckDeviceOnFormMain.comboBoxTypeOfCommandOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpCommand.TouchAny.ToString());
                userControlCheckDeviceOnFormMain.comboBoxTypeOfCommandOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpCommand.TouchOne.ToString());
                userControlCheckDeviceOnFormMain.comboBoxTypeOfCommandOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpCommand.TouchTwo.ToString());
                userControlCheckDeviceOnFormMain.comboBoxTypeOfCommandOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpCommand.TouchAnyShape.ToString());
                userControlCheckDeviceOnFormMain.comboBoxTypeOfCommandOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpCommand.DrawIndicator.ToString());
                userControlCheckDeviceOnFormMain.comboBoxTypeOfCommandOnGpTouchPanelOnUcCheckDevice.Items.Add(ECpCommand.TouchEnd.ToString());

                userControlCheckDeviceOnFormMain.comboBoxTypeOfCommandOnGpTouchPanelOnUcCheckDevice.SelectedIndex = 0;

                // FeedOn時間:初期化(最小値を設定) 
                userControlCheckDeviceOnFormMain.numericUpDownOnoShoTimeOnGpFeedOnUcCheckDevice.Value = userControlCheckDeviceOnFormMain.numericUpDownOnoShoTimeOnGpFeedOnUcCheckDevice.Minimum;
                userControlCheckDeviceOnFormMain.numericUpDownOnoShoTimeOnGpFeedOnUcCheckDevice.Text = userControlCheckDeviceOnFormMain.numericUpDownOnoShoTimeOnGpFeedOnUcCheckDevice.Value.ToString();// FeedOn時間:初期化(最小値を設定) 

                userControlCheckDeviceOnFormMain.numericUpDownOnoShoTimeOnGpFeed2OnUcCheckDevice.Value = userControlCheckDeviceOnFormMain.numericUpDownOnoShoTimeOnGpFeed2OnUcCheckDevice.Minimum;
                userControlCheckDeviceOnFormMain.numericUpDownOnoShoTimeOnGpFeed2OnUcCheckDevice.Text = userControlCheckDeviceOnFormMain.numericUpDownOnoShoTimeOnGpFeed2OnUcCheckDevice.Value.ToString();
                // SizeOfShapeInPixel:初期化(最小値を設定) 
                userControlCheckDeviceOnFormMain.numericUpDownSizeOfShapeInPixelOnGpTouchPanelOnUcCheckDevice.Value = userControlCheckDeviceOnFormMain.numericUpDownSizeOfShapeInPixelOnGpTouchPanelOnUcCheckDevice.Minimum;
                userControlCheckDeviceOnFormMain.numericUpDownSizeOfShapeInPixelOnGpTouchPanelOnUcCheckDevice.Text = userControlCheckDeviceOnFormMain.numericUpDownSizeOfShapeInPixelOnGpTouchPanelOnUcCheckDevice.Value.ToString();

                // TextBoxのクリア
                userControlCheckDeviceOnFormMain.textBoxResultOnGpTouchPanelOnUcCheckDevice.ResetText();
                userControlCheckDeviceOnFormMain.textBoxIdCodeOnGpIdCodeOnUcCheckDevice.ResetText();

                // OpeImage初期化
                if (opImage.InitOpeImage(
                    userControlCheckDeviceOnFormMain.pictureBoxTouchPanelOnGpTouchPanelOnUcCheckDevice,
                    formSub.pictureBoxOnFormSub) != true)
                {
                    EndUcCheckDevice();
                    VisibleUcMain();
                    return;
                }
                // 画面:黒
                opImage.DrawBackColor(Color.Black);

                userControlCheckDeviceOnFormMain.labelPicVersion.Text = "PicVersion: " + ioBoardDevice.GetVersionData();
            }
        }

        /// <summary>
        /// userControlCheckDeviceOnFormMain: 表示
        /// </summary>
        private void VisibleUcCheckDevice()
        {
            // シリアル・ポート・オープンしていない時
            if (serialPortOpenFlag != true)
            {
                MessageBox.Show("COM port isn't openned", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // シリアルポート無い場合もCheckDevice開くように
                //if (Properties.Settings.Default.IS_CHECK_SERIAL_OPENED == true)
                //{
                //    EndUcCheckDevice();
                //    VisibleUcMain();
                //    return;
                //}
            }

            // userControlCheckDeviceOnFormMain: 表示
            userControlMainOnFormMain.Visible = false;
            userControlOperationOnFormMain.Visible = false;
            userControlCheckDeviceOnFormMain.Visible = true;
            userControlCheckDeviceOnFormMain.Dock = DockStyle.Fill;
            userControlCheckIoOnFormMain.Visible = false;
            userControlPreferencesTabOnFormMain.Visible = false;
            userControlInputComOnFormMain.Visible = false;
            // Form.Text設定
            this.Text = "Check device";

        }
        private void EndUcCheckDevice()
        {
            // タイマ停止
            timerOperation.Stop();
            // FormSubのMouseClickイベントを無効とする
            formSub.boolEnableCallBackTouchPoint.Value = false;
            formSub.callbackTouchPoint = (point) => { };
            // シリアルの受信デリゲートを無効にする
            serialHelperPort.isEnableCallBackReceivedData.Value = false;
            callbackReceivedDataSub = (str) => { };
            // タッチパネルのタッチ検知を終了する
            OpSubEndToTouchCorrectOnTouchPanel();

        }

        public void UpdateFeedStatus(string str)
        {
            userControlCheckDeviceOnFormMain.textBoxDOutFeedStateOnGpFeedOnUcCheckDevice.Text = str;
        }
        //呼び出し元だけではダメ
        public void UpdateFeed2Status(string str)
        {
            userControlCheckDeviceOnFormMain.textBoxDOutFeedStateOnGpFeed2OnUcCheckDevice.Text = str;
        }

    }

}

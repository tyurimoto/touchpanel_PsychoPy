// #define RANDOM_ENABLE

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Compartment
{
    class UcPreferencesTab
    {
    }
    public partial class FormMain : Form
    {
        private void InitializeComponentOnUcPreferencesTab()
        {
            // ユーザ・コントロール: VisibleChanged
            userControlPreferencesTabOnFormMain.VisibleChanged += new System.EventHandler(userControlPreferencesTabOnFormMain_VisibleChanged);
            // ボタン:
            userControlPreferencesTabOnFormMain.buttonEnd.Click += new System.EventHandler(buttonEndOnUcPreferencesTab_Click);
            userControlPreferencesTabOnFormMain.buttonCancel.Click += new System.EventHandler(buttonCancelOnUcPreferencesTab_Click);
            userControlPreferencesTabOnFormMain.buttonCredit.Click += new System.EventHandler(buttonCredit_Click);

            //エピソード記憶設定不可視
            //userControlPreferencesTabOnFormMain.tabControlTab.TabPages.Remove(userControlPreferencesTabOnFormMain.tabControlTab.TabPages[userControlPreferencesTabOnFormMain.tabControlTab.TabPages.Count - 1]);

            #region  Compartmentカテゴリ----------------------------------------------------------------------------
            {
                //			userControlPreferencesTabOnFormMain.numericUpDownCompartmentNo.TextChanged += (object sender, EventArgs e) =>
                //			{
                //				if (userControlPreferencesTabOnFormMain.numericUpDownCompartmentNo.Text == "")
                //				{
                //					userControlPreferencesTabOnFormMain.numericUpDownCompartmentNo.Text = userControlPreferencesTabOnFormMain.numericUpDownCompartmentNo.Value.ToString();
                //				}
                //			};
                userControlPreferencesTabOnFormMain.numericUpDownCompartmentNo.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownCompartmentNo.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownCompartmentNo.Text = userControlPreferencesTabOnFormMain.numericUpDownCompartmentNo.Value.ToString();
                    }
                };
            }
            #endregion

            #region ID codeカテゴリ--------------------------------------------------------------------------------
            {
            }
            #endregion

            #region Operationカテゴリ------------------------------------------------------------------------------
            {
                userControlPreferencesTabOnFormMain.numericUpDownNumberOfTrial.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownNumberOfTrial.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownNumberOfTrial.Text = userControlPreferencesTabOnFormMain.numericUpDownNumberOfTrial.Value.ToString();
                        userControlPreferencesTabOnFormMain.numericUpDownCompartmentNo.Text = userControlPreferencesTabOnFormMain.numericUpDownCompartmentNo.Value.ToString();
                    }
                };

                userControlPreferencesTabOnFormMain.numericUpDownTimeToDisplayCorrectImage.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownTimeToDisplayCorrectImage.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownTimeToDisplayCorrectImage.Text = userControlPreferencesTabOnFormMain.numericUpDownTimeToDisplayCorrectImage.Value.ToString();
                    }
                };

                userControlPreferencesTabOnFormMain.numericUpDownTimeToDisplayNoImage.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownTimeToDisplayNoImage.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownTimeToDisplayNoImage.Text = userControlPreferencesTabOnFormMain.numericUpDownTimeToDisplayNoImage.Value.ToString();
                    }
                };

                userControlPreferencesTabOnFormMain.numericUpDownIntervalTimeMinimum.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownIntervalTimeMinimum.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownIntervalTimeMinimum.Text = userControlPreferencesTabOnFormMain.numericUpDownIntervalTimeMinimum.Value.ToString();
                    }
                };

                userControlPreferencesTabOnFormMain.numericUpDownIntervalTimeMaximum.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownIntervalTimeMaximum.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownIntervalTimeMaximum.Text = userControlPreferencesTabOnFormMain.numericUpDownIntervalTimeMaximum.Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.checkBoxEnableRandomTime.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.checkBoxEnableRandomTime.Checked)
                    {

                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownRandomTimeMin.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownRandomTimeMin.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownRandomTimeMin.Text = userControlPreferencesTabOnFormMain.numericUpDownRandomTimeMin.Value.ToString();
                    }
                };

                userControlPreferencesTabOnFormMain.numericUpDownRandomTimeMax.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownRandomTimeMax.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownRandomTimeMax.Text = userControlPreferencesTabOnFormMain.numericUpDownRandomTimeMax.Value.ToString();
                    }
                };

                userControlPreferencesTabOnFormMain.numericUpDownBoxFeedingRate.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownBoxFeedingRate.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownBoxFeedingRate.Text = userControlPreferencesTabOnFormMain.numericUpDownBoxFeedingRate.Value.ToString();
                    }
                };

                userControlPreferencesTabOnFormMain.numericUpDownTimeToFeed.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownTimeToFeed.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownTimeToFeed.Text = userControlPreferencesTabOnFormMain.numericUpDownTimeToFeed.Value.ToString();
                    }
                };

                userControlPreferencesTabOnFormMain.numericUpDownDelayFeed.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownDelayFeed.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownDelayFeed.Text = userControlPreferencesTabOnFormMain.numericUpDownDelayFeed.Value.ToString();
                    }
                };

                userControlPreferencesTabOnFormMain.checkBoxEnableFeedLamp.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.checkBoxEnableFeedLamp.Checked)
                    {

                    }
                };

                userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfStart.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfStart.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfStart.Text = userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfStart.Value.ToString();
                    }
                };

                userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfTrial.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfTrial.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfTrial.Text = userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfTrial.Value.ToString();
                    }
                };

                userControlPreferencesTabOnFormMain.numericUpDownMonitorSaveTime.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownMonitorSaveTime.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownMonitorSaveTime.Text = userControlPreferencesTabOnFormMain.numericUpDownMonitorSaveTime.Value.ToString();
                    }
                };

                userControlPreferencesTabOnFormMain.checkBoxEnableMonitorSave.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.checkBoxEnableMonitorSave.Checked)
                    {

                    }
                };

                userControlPreferencesTabOnFormMain.numericUpDownDelayRoomLampOn.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownDelayRoomLampOn.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownDelayRoomLampOn.Text = userControlPreferencesTabOnFormMain.numericUpDownDelayRoomLampOn.Value.ToString();
                    }
                };

                userControlPreferencesTabOnFormMain.checkBoxEnableReEntry.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.checkBoxEnableReEntry.Checked)
                    {

                    }
                };

                userControlPreferencesTabOnFormMain.numericUpDownReEntryTimeout.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownReEntryTimeout.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownReEntryTimeout.Text = userControlPreferencesTabOnFormMain.numericUpDownReEntryTimeout.Value.ToString();
                    }
                };

                userControlPreferencesTabOnFormMain.checkBoxEnableEpisodeMemory.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.checkBoxEnableEpisodeMemory.Checked)
                    {
                        userControlPreferencesTabOnFormMain.checkBoxForceEpisodeMemory.Enabled = true;
                    }
                    else
                    {
                        userControlPreferencesTabOnFormMain.checkBoxForceEpisodeMemory.Enabled = false;
                    }
                };

                userControlPreferencesTabOnFormMain.checkBoxForceEpisodeMemory.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.checkBoxForceEpisodeMemory.Checked)
                    {

                    }
                };

                userControlPreferencesTabOnFormMain.numericUpDownTimeToFeedReward.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownTimeToFeedReward.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownTimeToFeedReward.Text = userControlPreferencesTabOnFormMain.numericUpDownTimeToFeedReward.Value.ToString();
                    }
                };

                userControlPreferencesTabOnFormMain.numericUpDownExpireTime.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownExpireTime.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownExpireTime.Text = userControlPreferencesTabOnFormMain.numericUpDownExpireTime.Value.ToString();
                    }
                };

                userControlPreferencesTabOnFormMain.checkBoxEnableIncorrectRandom.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.checkBoxEnableIncorrectRandom.Checked)
                    {

                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownEpisodeCount.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownEpisodeCount.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownEpisodeCount.Text = userControlPreferencesTabOnFormMain.numericUpDownEpisodeCount.Value.ToString();
                    }
                };

                userControlPreferencesTabOnFormMain.numericUpDownEpisodeTimezoneStart.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownEpisodeTimezoneStart.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownEpisodeTimezoneStart.Text = userControlPreferencesTabOnFormMain.numericUpDownEpisodeTimezoneStart.Value.ToString();
                    }
                    userControlPreferencesTabOnFormMain.numericUpDownEpisodeTimezoneStart.Value =
                    CheckTimeZoneStart(userControlPreferencesTabOnFormMain.numericUpDownEpisodeTimezoneStart.Value, userControlPreferencesTabOnFormMain.numericUpDownEpisodeTimezoneEnd.Value);

                };

                userControlPreferencesTabOnFormMain.numericUpDownEpisodeTimezoneEnd.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownEpisodeTimezoneEnd.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownEpisodeTimezoneEnd.Text = userControlPreferencesTabOnFormMain.numericUpDownEpisodeTimezoneStart.Value.ToString();
                    }
                    userControlPreferencesTabOnFormMain.numericUpDownEpisodeTimezoneEnd.Value =
                    CheckTimeZoneEnd(userControlPreferencesTabOnFormMain.numericUpDownEpisodeTimezoneStart.Value, userControlPreferencesTabOnFormMain.numericUpDownEpisodeTimezoneEnd.Value);
                };

                userControlPreferencesTabOnFormMain.numericUpDownEpSelectTZStartTime.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownEpSelectTZStartTime.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownEpSelectTZStartTime.Text = userControlPreferencesTabOnFormMain.numericUpDownEpSelectTZStartTime.Value.ToString();
                    }
                    userControlPreferencesTabOnFormMain.numericUpDownEpSelectTZStartTime.Value =
                    CheckTimeZoneStart(userControlPreferencesTabOnFormMain.numericUpDownEpSelectTZStartTime.Value, userControlPreferencesTabOnFormMain.numericUpDownEpSelectTZEndTime.Value);

                };

                userControlPreferencesTabOnFormMain.numericUpDownEpSelectTZEndTime.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownEpSelectTZEndTime.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownEpSelectTZEndTime.Text = userControlPreferencesTabOnFormMain.numericUpDownEpSelectTZEndTime.Value.ToString();
                    }
                    userControlPreferencesTabOnFormMain.numericUpDownEpSelectTZEndTime.Value =
                    CheckTimeZoneEnd(userControlPreferencesTabOnFormMain.numericUpDownEpSelectTZStartTime.Value, userControlPreferencesTabOnFormMain.numericUpDownEpSelectTZEndTime.Value);
                };

                userControlPreferencesTabOnFormMain.numericUpDownEpIntervalTime.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownEpIntervalTime.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownEpIntervalTime.Text = userControlPreferencesTabOnFormMain.numericUpDownEpIntervalTime.Value.ToString();
                    }
                };
                // TestIndicator
                userControlPreferencesTabOnFormMain.checkBoxTestIndicator.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (((CheckBox)sender).Checked)
                    {
                    }
                };
                userControlPreferencesTabOnFormMain.selectFileListIndicatorPath.Filter = "SVG(*.svg)|*.svg|All files (*.*)|*.*";
                userControlPreferencesTabOnFormMain.comboBoxIndicatorPosition.SelectedIndexChanged += (object sender, EventArgs e) =>
                    {

                    };
                userControlPreferencesTabOnFormMain.checkBoxNoIDOperation.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (((CheckBox)sender).Checked)
                    {
                    }
                };
                userControlPreferencesTabOnFormMain.checkBoxNoEntryIDOperation.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (((CheckBox)sender).Checked)
                    {
                    }
                };
            }
            #endregion

            #region Imageカテゴリ----------------------------------------------------------------------------------
            {
                userControlPreferencesTabOnFormMain.buttonTriggerImageFile.Click += new System.EventHandler(buttonTriggerImageFileOnUcPreferencesTab_Click);
                userControlPreferencesTabOnFormMain.buttonEpTriggerImageFile.Click += new EventHandler(buttonEpisodeTriggerImageFileOnUcPreferencesTab_Click);

                userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep1.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep1.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep1.Text = userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep1.Value.ToString();
                    }
                };

                userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep2.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep2.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep2.Text = userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep2.Value.ToString();
                    }
                };

                userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep3.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep3.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep3.Text = userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep3.Value.ToString();
                    }
                };

                userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep4.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep4.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep4.Text = userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep4.Value.ToString();
                    }
                };

                userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep5.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep5.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep5.Text = userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep5.Value.ToString();
                    }
                };

                userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixel.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixel.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixel.Text = userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixel.Value.ToString();
                    }
                };

                userControlPreferencesTabOnFormMain.comboBoxSizeOfShapeInStep.SelectedIndexChanged += (object sender, EventArgs e) =>
                {
                    // Step1:80
                    // Step2:120
                    // Step3:160
                    // Step4:200
                    // Step5:250
                    // という前提 簡易対応

                    switch (((ComboBox)sender).SelectedIndex)
                    {
                        case 0:
                            userControlPreferencesTabOnFormMain.numericUpDownIncorrectNum.Maximum = 4;
                            userControlPreferencesTabOnFormMain.labelIncorrectMax.Text = "Max:4";
                            break;
                        case 1:
                            userControlPreferencesTabOnFormMain.numericUpDownIncorrectNum.Maximum = 4;
                            userControlPreferencesTabOnFormMain.labelIncorrectMax.Text = "Max:4";
                            break;
                        case 2:
                            if (userControlPreferencesTabOnFormMain.numericUpDownIncorrectNum.Value > 3)
                            {
                                userControlPreferencesTabOnFormMain.numericUpDownIncorrectNum.Value = 3;
                            }
                            userControlPreferencesTabOnFormMain.numericUpDownIncorrectNum.Maximum = 3;
                            userControlPreferencesTabOnFormMain.labelIncorrectMax.Text = "Max:3";
                            break;
                        case 3:
                            if (userControlPreferencesTabOnFormMain.numericUpDownIncorrectNum.Value > 2)
                            {
                                userControlPreferencesTabOnFormMain.numericUpDownIncorrectNum.Value = 2;
                            }

                            userControlPreferencesTabOnFormMain.numericUpDownIncorrectNum.Maximum = 2;
                            userControlPreferencesTabOnFormMain.labelIncorrectMax.Text = "Max:2";

                            break;
                        case 4:
                            if (userControlPreferencesTabOnFormMain.numericUpDownIncorrectNum.Value > 1)
                            {
                                userControlPreferencesTabOnFormMain.numericUpDownIncorrectNum.Value = 1;
                            }
                            userControlPreferencesTabOnFormMain.numericUpDownIncorrectNum.Maximum = 1;
                            userControlPreferencesTabOnFormMain.labelIncorrectMax.Text = "Max:1";

                            break;
                        default:
                            userControlPreferencesTabOnFormMain.numericUpDownIncorrectNum.Maximum = 4;
                            userControlPreferencesTabOnFormMain.labelIncorrectMax.Text = "Max:4";

                            break;
                    }
                    // Step1:80
                    // Step2:120
                    // Step3:160
                    // Step4:200
                    // Step5:250
                    // という前提 簡易対応

                    switch (((ComboBox)sender).SelectedIndex)
                    {
                        case 0:
                            userControlPreferencesTabOnFormMain.numericUpDownEpisodeTargetNum.Maximum = 5;
                            break;
                        case 1:
                            userControlPreferencesTabOnFormMain.numericUpDownEpisodeTargetNum.Maximum = 5;
                            break;
                        case 2:
                            if (userControlPreferencesTabOnFormMain.numericUpDownEpisodeTargetNum.Value > 4)
                            {
                                userControlPreferencesTabOnFormMain.numericUpDownEpisodeTargetNum.Value = 4;
                            }
                            userControlPreferencesTabOnFormMain.numericUpDownEpisodeTargetNum.Maximum = 4;
                            break;
                        case 3:
                            if (userControlPreferencesTabOnFormMain.numericUpDownEpisodeTargetNum.Value > 3)
                            {
                                userControlPreferencesTabOnFormMain.numericUpDownEpisodeTargetNum.Value = 3;
                            }

                            userControlPreferencesTabOnFormMain.numericUpDownEpisodeTargetNum.Maximum = 3;

                            break;
                        case 4:
                            if (userControlPreferencesTabOnFormMain.numericUpDownEpisodeTargetNum.Value > 2)
                            {
                                userControlPreferencesTabOnFormMain.numericUpDownEpisodeTargetNum.Value = 2;
                            }
                            userControlPreferencesTabOnFormMain.numericUpDownEpisodeTargetNum.Maximum = 2;

                            break;
                        default:
                            userControlPreferencesTabOnFormMain.numericUpDownEpisodeTargetNum.Maximum = 5;

                            break;
                    }
                    userControlPreferencesTabOnFormMain.labelEpisodeTargetNumMax.Text = "Max:" + userControlPreferencesTabOnFormMain.numericUpDownEpisodeTargetNum.Maximum;
                };
            }
            #endregion

            #region CorrectCondition-------------------------------------------------------------------------------
            {
                //userControlPreferencesTabOnFormMain.numericUpDownCorrectFixX.Minimum = opImage.RectOpeImageValidArea.X;
                //userControlPreferencesTabOnFormMain.numericUpDownCorrectFixY.Minimum = opImage.RectOpeImageValidArea.Y;
                //userControlPreferencesTabOnFormMain.numericUpDownCorrectFixX.Maximum = opImage.RectOpeImageValidArea.Width;
                //userControlPreferencesTabOnFormMain.numericUpDownCorrectFixY.Maximum = opImage.RectOpeImageValidArea.Height;
                //仮固定値だが最終的にOpImage側から取得
                userControlPreferencesTabOnFormMain.numericUpDownCorrectFixX.Minimum = 19;
                userControlPreferencesTabOnFormMain.numericUpDownCorrectFixY.Minimum = 147;
                userControlPreferencesTabOnFormMain.numericUpDownCorrectFixX.Maximum = 762;
                userControlPreferencesTabOnFormMain.numericUpDownCorrectFixY.Maximum = 453;

                userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX1.Minimum = 19;
                userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY1.Minimum = 147;
                userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX1.Maximum = 762;
                userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY1.Maximum = 453;

                userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX2.Minimum = 19;
                userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY2.Minimum = 147;
                userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX2.Maximum = 762;
                userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY2.Maximum = 453;

                userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX3.Minimum = 19;
                userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY3.Minimum = 147;
                userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX3.Maximum = 762;
                userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY3.Maximum = 453;

                userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX4.Minimum = 19;
                userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY4.Minimum = 147;
                userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX4.Maximum = 762;
                userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY4.Maximum = 453;

                userControlPreferencesTabOnFormMain.buttonSelectImageFolder.Click += new System.EventHandler(buttonCorrectImageFolder_Click);
                userControlPreferencesTabOnFormMain.buttonSelectCorrectImage.Click += new System.EventHandler(buttonCorrectImage_Click);

                userControlPreferencesTabOnFormMain.radioButtonCoordinate.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (((RadioButton)sender).Checked)
                    {

                    }
                };

                userControlPreferencesTabOnFormMain.radioButtonShape.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (((RadioButton)sender).Checked)
                    {

                    }
                };
                userControlPreferencesTabOnFormMain.radioButtonColor.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (((RadioButton)sender).Checked)
                    {

                    }
                };
                userControlPreferencesTabOnFormMain.checkBoxCoordinateRandom.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (((CheckBox)sender).Checked)
                    {

                    }
                };
                userControlPreferencesTabOnFormMain.checkBoxColorRandom.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (((CheckBox)sender).Checked)
                    {

                    }
                };
                userControlPreferencesTabOnFormMain.checkBoxShapeRandom.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (((CheckBox)sender).Checked)
                    {

                    }
                };
                userControlPreferencesTabOnFormMain.checkBoxIncorrectCoordinateRandom.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (((CheckBox)sender).Checked)
                    {

                    }
                };
                userControlPreferencesTabOnFormMain.checkBoxIncorrectColorRandom.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (((CheckBox)sender).Checked)
                    {

                    }
                };
                userControlPreferencesTabOnFormMain.checkBoxIncorrectShapeRandom.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (((CheckBox)sender).Checked)
                    {

                    }
                };

                userControlPreferencesTabOnFormMain.numericUpDownCorrectFixX.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownCorrectFixY.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownIncorrectNum.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX1.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX2.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX3.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX4.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY1.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY2.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY3.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY4.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.checkBoxEnableImageShape.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (((CheckBox)sender).Checked)
                    {
                        if (userControlPreferencesTabOnFormMain.radioButtonColor.Checked)
                        {
                            userControlPreferencesTabOnFormMain.radioButtonCoordinate.Checked = true;
                        }
                        userControlPreferencesTabOnFormMain.radioButtonColor.Enabled = false;

                    }
                    else
                    {
                        userControlPreferencesTabOnFormMain.radioButtonColor.Enabled = true;
                    }
                };
                userControlPreferencesTabOnFormMain.checkBoxRandomCorrectImage.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (((CheckBox)sender).Checked)
                    {
                        userControlPreferencesTabOnFormMain.textBoxCorrectImage.Enabled = false;
                        userControlPreferencesTabOnFormMain.buttonSelectCorrectImage.Enabled = false;
                    }
                    else
                    {
                        userControlPreferencesTabOnFormMain.textBoxCorrectImage.Enabled = true;
                        userControlPreferencesTabOnFormMain.buttonSelectCorrectImage.Enabled = true;
                    }

                };
                // Incorrect Cancel
                userControlPreferencesTabOnFormMain.checkBoxEnableIncorrectCancel.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (((CheckBox)sender).Checked)
                    {
                    }
                };
                // Incorrect Cancel count
                userControlPreferencesTabOnFormMain.numericUpDownIncorrectCount.Visible = true;
                userControlPreferencesTabOnFormMain.labelIncorrectCancel.Visible = true;
                userControlPreferencesTabOnFormMain.numericUpDownIncorrectCount.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };

                userControlPreferencesTabOnFormMain.numericUpDownIncorrectPenaltyTime.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
            }

            #endregion

            #region Soundカテゴリ----------------------------------------------------------------------------------
            {
                userControlPreferencesTabOnFormMain.buttonSoundFileOfEnd.Click += new System.EventHandler(buttonSoundFileOfEndOnUcPreferencesTab_Click);
                userControlPreferencesTabOnFormMain.buttonSoundFileOfReward.Click += new System.EventHandler(buttonSoundFileOfRewardOnUcPreferencesTab_Click);
                userControlPreferencesTabOnFormMain.buttonSoundFileOfCorrect.Click += new System.EventHandler(buttonSoundFileOfCorrectOnUcPreferencesTab_Click);
                userControlPreferencesTabOnFormMain.buttonSoundFileOfIncorrect.Click += new System.EventHandler(buttonSoundFileOfIncorrectOnUcPreferencesTab_Click);
                userControlPreferencesTabOnFormMain.buttonSoundFileOfIncorrectReward.Click += new System.EventHandler(buttonSoundFileOfIncorrectRewardOnUcPreferencesTab_Click);

                userControlPreferencesTabOnFormMain.numericUpDownTimeToOutputSoundOfEnd.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownTimeToOutputSoundOfEnd.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownTimeToOutputSoundOfEnd.Text = userControlPreferencesTabOnFormMain.numericUpDownTimeToOutputSoundOfEnd.Value.ToString();
                    }
                };

                userControlPreferencesTabOnFormMain.numericUpDownTimeToOutputSoundOfReward.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownTimeToOutputSoundOfReward.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownTimeToOutputSoundOfReward.Text = userControlPreferencesTabOnFormMain.numericUpDownTimeToOutputSoundOfReward.Value.ToString();
                    }
                };

                //userControlPreferencesTabOnFormMain.numericUpDownTimeToOutputSoundOfCorrect.Leave += (object sender, EventArgs e) =>
                //{
                //    if (userControlPreferencesTabOnFormMain.numericUpDownTimeToOutputSoundOfCorrect.Text == "")
                //    {
                //        userControlPreferencesTabOnFormMain.numericUpDownTimeToOutputSoundOfCorrect.Text = userControlPreferencesTabOnFormMain.numericUpDownTimeToOutputSoundOfCorrect.Value.ToString();
                //    }
                //};
            }
            #endregion

            #region Mechanical thingカテゴリ:----------------------------------------------------------------------
            {
                userControlPreferencesTabOnFormMain.toolTip1.SetToolTip(userControlPreferencesTabOnFormMain.labelEDoorOpenSpeed, "開きスピードを指定します");
                userControlPreferencesTabOnFormMain.toolTip1.SetToolTip(userControlPreferencesTabOnFormMain.labelEDoorCloseSpeed, "閉じスピードを指定します");
                userControlPreferencesTabOnFormMain.toolTip1.SetToolTip(userControlPreferencesTabOnFormMain.labelEDoorMiddleSpeed, "中間扉スピードを指定します");
                userControlPreferencesTabOnFormMain.toolTip1.SetToolTip(userControlPreferencesTabOnFormMain.labelEDoorMiddleTime, "開き→中間扉時間を指定します[ms]");
                userControlPreferencesTabOnFormMain.toolTip1.SetToolTip(userControlPreferencesTabOnFormMain.labelEDoorCloseToMiddleTime, "閉じ→中間扉時間を指定します[ms]");
                userControlPreferencesTabOnFormMain.toolTip1.SetToolTip(userControlPreferencesTabOnFormMain.labelEDoorOpenIntervalTime, "開き動作後条件判別するまでのInterval時間");
                userControlPreferencesTabOnFormMain.toolTip1.SetToolTip(userControlPreferencesTabOnFormMain.labelEDoorCloseIntervalTime, "閉じ動作後条件判別するまでのInterval時間");
                userControlPreferencesTabOnFormMain.toolTip1.SetToolTip(userControlPreferencesTabOnFormMain.labelEDoorMiddleIntervalTime, "中間扉動作後条件判別するまでのInterval時間");
                userControlPreferencesTabOnFormMain.toolTip1.SetToolTip(userControlPreferencesTabOnFormMain.labelEDoorCloseTimeout, "閉じ状態で開き状態に戻る時間を指定します");
                userControlPreferencesTabOnFormMain.toolTip1.SetToolTip(userControlPreferencesTabOnFormMain.labelEDoorMiddleTimeout, "開き閉じ状態で中間扉状態が続いた場合、開きに戻る時間を指定します");
                userControlPreferencesTabOnFormMain.toolTip1.SetToolTip(userControlPreferencesTabOnFormMain.labelEDoorMonitorStartTime, "扉条件モニタスタート時間 Interval時間より短いと無視されます");
                userControlPreferencesTabOnFormMain.toolTip1.SetToolTip(userControlPreferencesTabOnFormMain.labelEDoorCloseDoorMonitorStartTime, "扉閉時、半開き閉じを開始する時間を指定します");
                userControlPreferencesTabOnFormMain.toolTip1.SetToolTip(userControlPreferencesTabOnFormMain.labelEDoorInsideDetectCloseTime, "開き後入室センサ検知で閉じるまでの時間を指定します");
                userControlPreferencesTabOnFormMain.toolTip1.SetToolTip(userControlPreferencesTabOnFormMain.labelEDoorPresenceOpenTime, "入室センサ未検時、扉を開くまでの時間を指定します");
                userControlPreferencesTabOnFormMain.toolTip1.SetToolTip(userControlPreferencesTabOnFormMain.labelEDoorReEntryTime, "退室後、入室検知するまでのインターバル時間を指定します");
                userControlPreferencesTabOnFormMain.toolTip1.SetToolTip(userControlPreferencesTabOnFormMain.labelEDoorPresenceMiddleDetectTime, "中間扉状態から入室検知するまでのインターバル時間を指定します");
                userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfLeverIn.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfLeverIn.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfLeverIn.Text = userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfLeverIn.Value.ToString();
                    }
                };

                userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfLeverOut.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfLeverOut.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfLeverOut.Text = userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfLeverOut.Value.ToString();
                    }
                };

                userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfDoorOpen.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfDoorOpen.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfDoorOpen.Text = userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfDoorOpen.Value.ToString();
                    }
                };

                userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfDoorClose.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfDoorClose.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfDoorClose.Text = userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfDoorClose.Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownCageEntryTime.Leave += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.numericUpDownCageEntryTime.Text == "")
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownCageEntryTime.Text = userControlPreferencesTabOnFormMain.numericUpDownCageEntryTime.Value.ToString();
                    }
                };

                userControlPreferencesTabOnFormMain.checkBoxDisableDoor.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.checkBoxDisableDoor.Checked)
                    {

                    }
                    userControlPreferencesTabOnFormMain.numericUpDownTimeoutLeaveCage.Enabled = userControlPreferencesTabOnFormMain.checkBoxDisableDoor.Checked;
                };
                userControlPreferencesTabOnFormMain.checkBoxIgnoreDoorError.CheckedChanged += (object sender, EventArgs e) =>
                 {
                     if (userControlPreferencesTabOnFormMain.checkBoxIgnoreDoorError.Checked)
                     {


                     }
                 };
                userControlPreferencesTabOnFormMain.checkBoxDisableLever.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.checkBoxDisableLever.Checked)
                    {

                    }
                };
                userControlPreferencesTabOnFormMain.checkBoxConveyorFeed.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (userControlPreferencesTabOnFormMain.checkBoxConveyorFeed.Checked)
                    {

                    }
                };
                userControlPreferencesTabOnFormMain.checkBoxExtraFeeder.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (((CheckBox)sender).Checked)
                    {

                    }
                    else
                    {
                        if (userControlPreferencesTabOnFormMain.checkBoxMultiExtraFeeder.Checked)
                        {
                            userControlPreferencesTabOnFormMain.checkBoxMultiExtraFeeder.Checked = ((CheckBox)sender).Checked;
                        }
                    }
                    userControlPreferencesTabOnFormMain.checkBoxMultiExtraFeeder.Enabled = ((CheckBox)sender).Checked;
                };
                userControlPreferencesTabOnFormMain.checkBoxMultiExtraFeeder.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (((CheckBox)sender).Checked)
                    {

                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownEDoorOpenSpeed.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownEDoorCloseSpeed.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownEDoorMiddleSpeed.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownEDoorMiddleTime.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownEDoorCloseToMiddleTime.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownEDoorOpenIntervalTime.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownEDoorCloseIntervalTime.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownEDoorMiddleIntervalTime.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownEDoorCloseTimeout.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownEDoorMiddleTimeout.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownEDoorMonitorStartTime.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownEDoorCloseDoorMonitorStartTime.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownEDoorInsideDetectCloseTime.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownEDoorPresenceOpenTime.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownEDoorReEntryTime.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownEDoorPresenceMiddleDetectTime.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
            }
            #endregion

            #region Outputカテゴリ---------------------------------------------------------------------------------
            {
                userControlPreferencesTabOnFormMain.buttonOutputResultFile.Click += new System.EventHandler(buttonOutputResultFileOnUcPreferencesTab_Click);
                userControlPreferencesTabOnFormMain.buttonOpenDebugLogFolder.Click += new System.EventHandler(buttonOpenDebugLogFolder_Click);
                userControlPreferencesTabOnFormMain.buttonOpenExecuteFolder.Click += new System.EventHandler(buttonOpenExecuteFolder_Click);
                userControlPreferencesTabOnFormMain.buttonOpenResultLogFolder.Click += new System.EventHandler(buttonOpenResultLogFolder_Click);
            }
            #endregion

            #region EpisodeMemoryカテゴリ--------------------------------------------------------------------------
            {
                userControlPreferencesTabOnFormMain.numericUpDownEpisodeTargetNum.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.buttonEpisodeSelectImageFolder.Click += new System.EventHandler(buttonEpisodeImageFolder_Click);

                userControlPreferencesTabOnFormMain.numericUpDownEpTarget1FixX.Minimum = 19;
                userControlPreferencesTabOnFormMain.numericUpDownEpTarget1FixY.Minimum = 147;
                userControlPreferencesTabOnFormMain.numericUpDownEpTarget1FixX.Maximum = 762;
                userControlPreferencesTabOnFormMain.numericUpDownEpTarget1FixY.Maximum = 453;

                userControlPreferencesTabOnFormMain.numericUpDownEpTarget2FixX.Minimum = 19;
                userControlPreferencesTabOnFormMain.numericUpDownEpTarget2FixY.Minimum = 147;
                userControlPreferencesTabOnFormMain.numericUpDownEpTarget2FixX.Maximum = 762;
                userControlPreferencesTabOnFormMain.numericUpDownEpTarget2FixY.Maximum = 453;

                userControlPreferencesTabOnFormMain.numericUpDownEpTarget3FixX.Minimum = 19;
                userControlPreferencesTabOnFormMain.numericUpDownEpTarget3FixY.Minimum = 147;
                userControlPreferencesTabOnFormMain.numericUpDownEpTarget3FixX.Maximum = 762;
                userControlPreferencesTabOnFormMain.numericUpDownEpTarget3FixY.Maximum = 453;

                userControlPreferencesTabOnFormMain.numericUpDownEpTarget4FixX.Minimum = 19;
                userControlPreferencesTabOnFormMain.numericUpDownEpTarget4FixY.Minimum = 147;
                userControlPreferencesTabOnFormMain.numericUpDownEpTarget4FixX.Maximum = 762;
                userControlPreferencesTabOnFormMain.numericUpDownEpTarget4FixY.Maximum = 453;

                userControlPreferencesTabOnFormMain.numericUpDownEpTarget5FixX.Minimum = 19;
                userControlPreferencesTabOnFormMain.numericUpDownEpTarget5FixY.Minimum = 147;
                userControlPreferencesTabOnFormMain.numericUpDownEpTarget5FixX.Maximum = 762;
                userControlPreferencesTabOnFormMain.numericUpDownEpTarget5FixY.Maximum = 453;

                userControlPreferencesTabOnFormMain.checkBoxEpisodeCoordinateRandom.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (((CheckBox)sender).Checked)
                    {

                    }
                };
                userControlPreferencesTabOnFormMain.checkBoxEpisodeShapeRandom.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (((CheckBox)sender).Checked)
                    {

                    }
                };
                userControlPreferencesTabOnFormMain.checkBoxEpisodeColorRandom.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (((CheckBox)sender).Checked)
                    {

                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownEpTarget1FixX.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownEpTarget1FixY.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownEpTarget2FixX.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownEpTarget2FixY.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownEpTarget3FixX.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownEpTarget3FixY.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownEpTarget4FixX.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownEpTarget4FixY.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownEpTarget5FixX.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.numericUpDownEpTarget5FixY.Leave += (object sender, EventArgs e) =>
                {
                    if (((NumericUpDown)sender).Text == "")
                    {
                        ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                    }
                };
                userControlPreferencesTabOnFormMain.checkBoxEnableEpImageShape.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (((CheckBox)sender).Checked)
                    {

                    }
                    else
                    {

                    }

                };

                userControlPreferencesTabOnFormMain.checkBoxRandomCorrectImage.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (((CheckBox)sender).Checked)
                    {
                        userControlPreferencesTabOnFormMain.textBoxCorrectImage.Enabled = false;
                        userControlPreferencesTabOnFormMain.buttonSelectCorrectImage.Enabled = false;
                    }
                    else
                    {
                        userControlPreferencesTabOnFormMain.textBoxCorrectImage.Enabled = true;
                        userControlPreferencesTabOnFormMain.buttonSelectCorrectImage.Enabled = true;
                    }

                };
            }
            #endregion

            #region MarmosetDetectionカテゴリ
            //
            userControlPreferencesTabOnFormMain.buttonModelFile.Click += new System.EventHandler(buttonModelFile_Click);
            userControlPreferencesTabOnFormMain.buttonLearningImageFolder.Click += new System.EventHandler(buttonLearningImageFolder_Click);
            userControlPreferencesTabOnFormMain.numericUpDownMarmoDetectionThreshold.Leave += (object sender, EventArgs e) =>
            {
                if (((NumericUpDown)sender).Text == "")
                {
                    ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                }
            };
            userControlPreferencesTabOnFormMain.numericUpDownShotCamInterval.Leave += (object sender, EventArgs e) =>
            {
                if (((NumericUpDown)sender).Text == "")
                {
                    ((NumericUpDown)sender).Text = ((NumericUpDown)sender).Value.ToString();
                }
            };
            userControlPreferencesTabOnFormMain.checkBoxMarmosetDetection.CheckedChanged += (object sender, EventArgs e) =>
            {
                //userControlPreferencesTabOnFormMain.checkBoxMarmosetDetection.Checked = true;
                userControlPreferencesTabOnFormMain.checkBoxSaveOnlyImage.Checked = false;
                userControlPreferencesTabOnFormMain.checkBoxDetectionSaveImageMarmosetDetection.Checked = false;
            };
            userControlPreferencesTabOnFormMain.checkBoxSaveOnlyImage.CheckedChanged += (object sender, EventArgs e) =>
            {
                userControlPreferencesTabOnFormMain.checkBoxMarmosetDetection.Checked = false;
                //userControlPreferencesTabOnFormMain.checkBoxSaveOnlyImage.Checked = true;
                userControlPreferencesTabOnFormMain.checkBoxDetectionSaveImageMarmosetDetection.Checked = false;
            };
            userControlPreferencesTabOnFormMain.checkBoxDetectionSaveImageMarmosetDetection.CheckedChanged += (object sender, EventArgs e) =>
            {
                userControlPreferencesTabOnFormMain.checkBoxMarmosetDetection.Checked = false;
                userControlPreferencesTabOnFormMain.checkBoxSaveOnlyImage.Checked = false;
                //userControlPreferencesTabOnFormMain.checkBoxDetectionSaveImageMarmosetDetection.Checked = true;
            };
            #endregion
        }

        public decimal CheckTimeZoneStart(decimal aTime, decimal bTime)
        {

            if (aTime == 0 && bTime == 0)
            {
                return 0;
            }
            if (aTime == bTime)
            {
                return (aTime - 1);
            }
            //else if (aTime > bTime)
            //{
            //    if (bTime == 0)
            //    {
            //        return aTime;
            //    }
            //    else
            //    {
            //        return (bTime - 1);
            //    }
            //}
            else
            {
                return aTime;
            }
        }
        public decimal CheckTimeZoneEnd(decimal aTime, decimal bTime)
        {

            if (aTime == 0 && bTime == 0)
            {
                return 1;
            }
            if (aTime == bTime)
            {
                if (bTime + 1 > 23)
                {
                    return 0;
                }
                else
                {
                    return (bTime + 1);
                }
            }
            //else if (aTime > bTime)
            //{
            //    if (aTime + 1 > 23)
            //    {
            //        return 0;
            //    }
            //    else
            //    {
            //        return (aTime + 1);
            //    }
            //}
            else
            {
                return bTime;
            }
        }

        private void buttonEndOnUcPreferencesTab_Click(object sender, EventArgs e)
        {
            // 入力を取得
            {
                #region Compartmentカテゴリ
                // Compartmentタブ
                {
                    preferencesDatTemp.CompartmentNo = (int)userControlPreferencesTabOnFormMain.numericUpDownCompartmentNo.Value;
                }
                #endregion

                #region ID codeカテゴリ
                {
                    // ID codeタブ
                    preferencesDatTemp.ComPort = userControlPreferencesTabOnFormMain.comboBoxComPort.Text;
                    preferencesDatTemp.ComBaudRate = userControlPreferencesTabOnFormMain.comboBoxComBaurate.Text;
                    preferencesDatTemp.ComDataBitLength = userControlPreferencesTabOnFormMain.comboBoxComDataBitLength.Text;
                    preferencesDatTemp.ComStopBitLength = userControlPreferencesTabOnFormMain.comboBoxComStopBitLength.Text;
                    preferencesDatTemp.ComParity = userControlPreferencesTabOnFormMain.comboBoxComParity.Text;
                }
                #endregion

                #region Operationカテゴリ
                // Operationタブ
                {
                    preferencesDatTemp.OpeTypeOfTask = userControlPreferencesTabOnFormMain.comboBoxTypeOfTask.SelectedItem.ToString();

                    preferencesDatTemp.OpeNumberOfTrial = (int)userControlPreferencesTabOnFormMain.numericUpDownNumberOfTrial.Value;
                    preferencesDatTemp.OpeTimeToDisplayCorrectImage = (int)userControlPreferencesTabOnFormMain.numericUpDownTimeToDisplayCorrectImage.Value;
                    preferencesDatTemp.OpeTimeToDisplayNoImage = (int)userControlPreferencesTabOnFormMain.numericUpDownTimeToDisplayNoImage.Value;
                    preferencesDatTemp.OpeIntervalTimeMinimum = (int)userControlPreferencesTabOnFormMain.numericUpDownIntervalTimeMinimum.Value;
                    preferencesDatTemp.OpeIntervalTimeMaximum = (int)userControlPreferencesTabOnFormMain.numericUpDownIntervalTimeMaximum.Value;
                    preferencesDatTemp.EnableRandomTime = (bool)userControlPreferencesTabOnFormMain.checkBoxEnableRandomTime.Checked;
                    preferencesDatTemp.OpeRandomTimeMinimum = (int)userControlPreferencesTabOnFormMain.numericUpDownRandomTimeMin.Value;
                    preferencesDatTemp.OpeRandomTimeMaximum = (int)userControlPreferencesTabOnFormMain.numericUpDownRandomTimeMax.Value;
                    preferencesDatTemp.OpeFeedingRate = (int)userControlPreferencesTabOnFormMain.numericUpDownBoxFeedingRate.Value;
                    preferencesDatTemp.OpeTimeToFeed = (int)userControlPreferencesTabOnFormMain.numericUpDownTimeToFeed.Value;
                    preferencesDatTemp.OpeDelayFeed = (int)userControlPreferencesTabOnFormMain.numericUpDownDelayFeed.Value;
                    preferencesDatTemp.EnableFeedLamp = (bool)userControlPreferencesTabOnFormMain.checkBoxEnableFeedLamp.Checked;
                    preferencesDatTemp.OpeDelayFeedLamp = (int)userControlPreferencesTabOnFormMain.numericUpDownDelayFeedLamp.Value;
                    preferencesDatTemp.OpeTimeoutOfStart = (int)userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfStart.Value;
                    preferencesDatTemp.OpeTimeoutOfTrial = (int)userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfTrial.Value;
                    preferencesDatTemp.EnableMonitorSave = (bool)userControlPreferencesTabOnFormMain.checkBoxEnableMonitorSave.Checked;
                    preferencesDatTemp.MonitorSaveTime = (int)userControlPreferencesTabOnFormMain.numericUpDownMonitorSaveTime.Value;
                    preferencesDatTemp.OpeDelayRoomLampOnTime = (int)userControlPreferencesTabOnFormMain.numericUpDownDelayRoomLampOn.Value;
                    preferencesDatTemp.OpeEnableReEntry = userControlPreferencesTabOnFormMain.checkBoxEnableReEntry.Checked;
                    preferencesDatTemp.OpeReEntryTimeout = (int)userControlPreferencesTabOnFormMain.numericUpDownReEntryTimeout.Value;
                    preferencesDatTemp.EnableEpisodeMemory = userControlPreferencesTabOnFormMain.checkBoxEnableEpisodeMemory.Checked;
                    preferencesDatTemp.ForceEpisodeMemory = userControlPreferencesTabOnFormMain.checkBoxForceEpisodeMemory.Checked;
                    preferencesDatTemp.EnableEpisodeIncorrectRandom = userControlPreferencesTabOnFormMain.checkBoxEnableIncorrectRandom.Checked;
                    preferencesDatTemp.OpeTimeToFeedReward = (int)userControlPreferencesTabOnFormMain.numericUpDownTimeToFeedReward.Value;
                    preferencesDatTemp.OpeTimeToFeedRewardFirstTime = (int)userControlPreferencesTabOnFormMain.numericUpDownTimeToFeedRewardFirstTime.Value;
                    preferencesDatTemp.EpisodeExpireTime = (int)userControlPreferencesTabOnFormMain.numericUpDownExpireTime.Value;
                    preferencesDatTemp.EpisodeCount = (int)userControlPreferencesTabOnFormMain.numericUpDownEpisodeCount.Value;
                    preferencesDatTemp.EpisodeTimezoneStartTime = (int)userControlPreferencesTabOnFormMain.numericUpDownEpisodeTimezoneStart.Value;
                    preferencesDatTemp.EpisodeTimezoneEndTime = (int)userControlPreferencesTabOnFormMain.numericUpDownEpisodeTimezoneEnd.Value;
                    preferencesDatTemp.EpisodeSelectShapeTimezoneStartTime = (int)userControlPreferencesTabOnFormMain.numericUpDownEpSelectTZStartTime.Value;
                    preferencesDatTemp.EpisodeSelectShapeTimezoneEndTime = (int)userControlPreferencesTabOnFormMain.numericUpDownEpSelectTZEndTime.Value;
                    preferencesDatTemp.EpisodeIntervalTime = (int)userControlPreferencesTabOnFormMain.numericUpDownEpIntervalTime.Value;
                    preferencesDatTemp.TestIndicatorSvgPath = userControlPreferencesTabOnFormMain.selectFileListIndicatorPath.FileName;
                    preferencesDatTemp.IndicatorPosition = userControlPreferencesTabOnFormMain.comboBoxIndicatorPosition.SelectedIndex;
                    preferencesDatTemp.EnableNoIDOperation = userControlPreferencesTabOnFormMain.checkBoxNoIDOperation.Checked;
                    preferencesDatTemp.EnableNoEntryIDOperation = userControlPreferencesTabOnFormMain.checkBoxNoEntryIDOperation.Checked;

                    //チェック切り替えされてもエピソードリセット
                    if (!preferencesDatTemp.EnableEpisodeMemory)
                    {
                        idControlHelper.ClearEntry();
                        episodeMemory.ClearEntry();
                    }

                }
                #endregion

                #region Imageカテゴリ
                // Imageタブ
                {
                    preferencesDatTemp.TriggerImageFile = userControlPreferencesTabOnFormMain.textBoxTriggerImage.Text;
                    preferencesDatTemp.EpisodeTriggerImageFile = userControlPreferencesTabOnFormMain.textBoxEpTriggerImage.Text;

                    preferencesDatTemp.BackColor = userControlPreferencesTabOnFormMain.comboBoxBackColor.SelectedItem.ToString();
                    preferencesDatTemp.EpisodeFirstBackColor = new System.Drawing.ColorConverter().ConvertToString(userControlPreferencesTabOnFormMain.comboBoxEpFirstBackColor.SelectedItem);
                    preferencesDatTemp.EpisodeTestBackColor = new System.Drawing.ColorConverter().ConvertToString(userControlPreferencesTabOnFormMain.comboBoxEpTestBackColor.SelectedItem);

                    preferencesDatTemp.DelayBackColor = userControlPreferencesTabOnFormMain.comboBoxDelayBackColor.SelectedItem.ToString();
                    preferencesDatTemp.ShapeColor = userControlPreferencesTabOnFormMain.comboBoxShapeColor.SelectedItem.ToString();
                    preferencesDatTemp.TypeOfShape = userControlPreferencesTabOnFormMain.comboBoxTypeOfShape.SelectedItem.ToString();
                    preferencesDatTemp.SizeOfShapeInStep = userControlPreferencesTabOnFormMain.comboBoxSizeOfShapeInStep.SelectedItem.ToString();

                    preferencesDatTemp.SizeOfShapeInPixelForStep1 = (int)userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep1.Value;
                    preferencesDatTemp.SizeOfShapeInPixelForStep2 = (int)userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep2.Value;
                    preferencesDatTemp.SizeOfShapeInPixelForStep3 = (int)userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep3.Value;
                    preferencesDatTemp.SizeOfShapeInPixelForStep4 = (int)userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep4.Value;
                    preferencesDatTemp.SizeOfShapeInPixelForStep5 = (int)userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep5.Value;
                    preferencesDatTemp.SizeOfShapeInPixel = (int)userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixel.Value;
                }
                #endregion

                #region CorrectCondition
                {
                    {
                        if (userControlPreferencesTabOnFormMain.radioButtonCoordinate.Checked)
                            preferencesDatTemp.CorrectCondition = ECpCorrectCondition.Coordinate.ToString();
                        else if (userControlPreferencesTabOnFormMain.radioButtonColor.Checked)
                            preferencesDatTemp.CorrectCondition = ECpCorrectCondition.Color.ToString();
                        else if (userControlPreferencesTabOnFormMain.radioButtonShape.Checked)
                            preferencesDatTemp.CorrectCondition = ECpCorrectCondition.Shape.ToString();
                        else
                        {
                            preferencesDatTemp.CorrectCondition = ECpCorrectCondition.Coordinate.ToString();
                        }
                    }

                    preferencesDatTemp.RandomCoordinate = userControlPreferencesTabOnFormMain.checkBoxCoordinateRandom.Checked;
                    preferencesDatTemp.RandomColor = userControlPreferencesTabOnFormMain.checkBoxColorRandom.Checked;
                    preferencesDatTemp.RandomShape = userControlPreferencesTabOnFormMain.checkBoxShapeRandom.Checked;
                    preferencesDatTemp.CoordinateFixX = (int)userControlPreferencesTabOnFormMain.numericUpDownCorrectFixX.Value;
                    preferencesDatTemp.CoordinateFixY = (int)userControlPreferencesTabOnFormMain.numericUpDownCorrectFixY.Value;
                    preferencesDatTemp.ColorFix = userControlPreferencesTabOnFormMain.comboBoxCorrectFixColor.SelectedItem.ToString();
                    preferencesDatTemp.ShapeFix = userControlPreferencesTabOnFormMain.comboBoxCorrectFixShape.SelectedItem.ToString();

                    preferencesDatTemp.IncorrectNum = (int)userControlPreferencesTabOnFormMain.numericUpDownIncorrectNum.Value;
                    preferencesDatTemp.IncorrectCoordinateRandom = (bool)userControlPreferencesTabOnFormMain.checkBoxIncorrectCoordinateRandom.Checked;
                    preferencesDatTemp.IncorrectCoordinateFixX1 = (int)userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX1.Value;
                    preferencesDatTemp.IncorrectCoordinateFixX2 = (int)userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX2.Value;
                    preferencesDatTemp.IncorrectCoordinateFixX3 = (int)userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX3.Value;
                    preferencesDatTemp.IncorrectCoordinateFixX4 = (int)userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX4.Value;
                    preferencesDatTemp.IncorrectCoordinateFixY1 = (int)userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY1.Value;
                    preferencesDatTemp.IncorrectCoordinateFixY2 = (int)userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY2.Value;
                    preferencesDatTemp.IncorrectCoordinateFixY3 = (int)userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY3.Value;
                    preferencesDatTemp.IncorrectCoordinateFixY4 = (int)userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY4.Value;
                    preferencesDatTemp.IncorrectColorRandom = (bool)userControlPreferencesTabOnFormMain.checkBoxIncorrectColorRandom.Checked;
                    preferencesDatTemp.IncorrectColorFix1 = userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix1.SelectedItem.ToString();
                    preferencesDatTemp.IncorrectColorFix2 = userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix2.SelectedItem.ToString();
                    preferencesDatTemp.IncorrectColorFix3 = userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix3.SelectedItem.ToString();
                    preferencesDatTemp.IncorrectColorFix4 = userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix4.SelectedItem.ToString();
                    preferencesDatTemp.IncorrectShapeRandom = (bool)userControlPreferencesTabOnFormMain.checkBoxIncorrectShapeRandom.Checked;
                    preferencesDatTemp.IncorrectShapeFix1 = userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix1.SelectedItem.ToString();
                    preferencesDatTemp.IncorrectShapeFix2 = userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix2.SelectedItem.ToString();
                    preferencesDatTemp.IncorrectShapeFix3 = userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix3.SelectedItem.ToString();
                    preferencesDatTemp.IncorrectShapeFix4 = userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix4.SelectedItem.ToString();

                    preferencesDatTemp.EnableImageShape = userControlPreferencesTabOnFormMain.checkBoxEnableImageShape.Checked;
                    preferencesDatTemp.ImageFileFolder = userControlPreferencesTabOnFormMain.textBoxImageFileFolder.Text;
                    preferencesDatTemp.CorrectImageFile = userControlPreferencesTabOnFormMain.textBoxCorrectImage.Text;

                    preferencesDatTemp.RandomCorrectImage = userControlPreferencesTabOnFormMain.checkBoxRandomCorrectImage.Checked;

                    preferencesDatTemp.EnableIncorrectCancel = userControlPreferencesTabOnFormMain.checkBoxEnableIncorrectCancel.Checked;
                    preferencesDatTemp.IncorrectCount = (int)userControlPreferencesTabOnFormMain.numericUpDownIncorrectCount.Value;

                    preferencesDatTemp.EnableTestIndicator = userControlPreferencesTabOnFormMain.checkBoxTestIndicator.Checked;

                    preferencesDatTemp.IncorrectPenaltyTime = (int)userControlPreferencesTabOnFormMain.numericUpDownIncorrectPenaltyTime.Value;

                }
                #endregion

                #region Soundカテゴリ
                // Soundタブ
                {
                    preferencesDatTemp.SoundFileOfEnd = userControlPreferencesTabOnFormMain.textBoxSoundOfEnd.Text;
                    preferencesDatTemp.SoundFileOfReward = userControlPreferencesTabOnFormMain.textBoxSoundOfReward.Text;
                    preferencesDatTemp.SoundFileOfCorrect = userControlPreferencesTabOnFormMain.textBoxSoundOfCorrect.Text;
                    preferencesDatTemp.SoundFileOfIncorrect = userControlPreferencesTabOnFormMain.textBoxSoundOfIncorrect.Text;
                    preferencesDatTemp.SoundFileOfIncorrectReward = userControlPreferencesTabOnFormMain.textBoxSoundOfIncorrectReward.Text;

                    preferencesDatTemp.TimeToOutputSoundOfEnd = (int)userControlPreferencesTabOnFormMain.numericUpDownTimeToOutputSoundOfEnd.Value;
                    preferencesDatTemp.TimeToOutputSoundOfReward = (int)userControlPreferencesTabOnFormMain.numericUpDownTimeToOutputSoundOfReward.Value;
                    //preferencesDatTemp.TimeToOutputSoundOfCorrect = (int)userControlPreferencesTabOnFormMain.numericUpDownTimeToOutputSoundOfCorrect.Value;
                }
                #endregion

                #region Mechanical thingカテゴリ
                // Mechanical thingタブ
                {
                    preferencesDatTemp.TimeoutOfLeverIn = (int)userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfLeverIn.Value;
                    preferencesDatTemp.TimeoutOfLeverOut = (int)userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfLeverOut.Value;
                    preferencesDatTemp.TimeoutOfDoorOpen = (int)userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfDoorOpen.Value;
                    preferencesDatTemp.TimeoutOfDoorClose = (int)userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfDoorClose.Value;

                    preferencesDatTemp.OpeTimeoutOfLeaveCage= (int)userControlPreferencesTabOnFormMain.numericUpDownTimeoutLeaveCage.Value;
                    
                    preferencesDatTemp.CageEntryTime = (int)userControlPreferencesTabOnFormMain.numericUpDownCageEntryTime.Value;
                    preferencesDatTemp.DisableDoor = (bool)userControlPreferencesTabOnFormMain.checkBoxDisableDoor.Checked;
                    preferencesDatTemp.IgnoreDoorError = (bool)userControlPreferencesTabOnFormMain.checkBoxIgnoreDoorError.Checked;
                    preferencesDatTemp.DisableLever = (bool)userControlPreferencesTabOnFormMain.checkBoxDisableLever.Checked;
                    preferencesDatTemp.EnableConveyor = (bool)userControlPreferencesTabOnFormMain.checkBoxConveyorFeed.Checked;
                    preferencesDatTemp.EnableExtraFeeder = (bool)userControlPreferencesTabOnFormMain.checkBoxExtraFeeder.Checked;
                    preferencesDatTemp.EnableMultiFeeder = (bool)userControlPreferencesTabOnFormMain.checkBoxMultiExtraFeeder.Checked;

                    //EDoor
                    preferencesDatTemp.EDoorOpenSpeed = (int)userControlPreferencesTabOnFormMain.numericUpDownEDoorOpenSpeed.Value;
                    preferencesDatTemp.EDoorCloseSpeed = (int)userControlPreferencesTabOnFormMain.numericUpDownEDoorCloseSpeed.Value;
                    preferencesDatTemp.EDoorMiddleSpeed = (int)userControlPreferencesTabOnFormMain.numericUpDownEDoorMiddleSpeed.Value;
                    preferencesDatTemp.EDoorOpenToMiddleTime = (int)userControlPreferencesTabOnFormMain.numericUpDownEDoorMiddleTime.Value;
                    preferencesDatTemp.EDoorCloseToMiddleTime = (int)userControlPreferencesTabOnFormMain.numericUpDownEDoorCloseToMiddleTime.Value;
                    preferencesDatTemp.EDoorOpenIntervalTime = (int)userControlPreferencesTabOnFormMain.numericUpDownEDoorOpenIntervalTime.Value;
                    preferencesDatTemp.EDoorCloseIntervalTime = (int)userControlPreferencesTabOnFormMain.numericUpDownEDoorCloseIntervalTime.Value;
                    preferencesDatTemp.EDoorMiddleIntervalTime = (int)userControlPreferencesTabOnFormMain.numericUpDownEDoorMiddleIntervalTime.Value;
                    preferencesDatTemp.EDoorCloseTimeout = (int)userControlPreferencesTabOnFormMain.numericUpDownEDoorCloseTimeout.Value;
                    preferencesDatTemp.EDoorMiddleTimeout = (int)userControlPreferencesTabOnFormMain.numericUpDownEDoorMiddleTimeout.Value;
                    preferencesDatTemp.EDoorMonitorStartTime = (int)userControlPreferencesTabOnFormMain.numericUpDownEDoorMonitorStartTime.Value;
                    preferencesDatTemp.EDoorCloseDoorMonitorStartTime = (int)userControlPreferencesTabOnFormMain.numericUpDownEDoorCloseDoorMonitorStartTime.Value;
                    preferencesDatTemp.EDoorInsideDetectCloseTime = (int)userControlPreferencesTabOnFormMain.numericUpDownEDoorInsideDetectCloseTime.Value;
                    preferencesDatTemp.EDoorPresenceOpenTime = (int)userControlPreferencesTabOnFormMain.numericUpDownEDoorPresenceOpenTime.Value;
                    preferencesDatTemp.EDoorReEntryTime = (int)userControlPreferencesTabOnFormMain.numericUpDownEDoorReEntryTime.Value;
                }
                #endregion

                #region Output dataカテゴリ
                // Output dataタブ
                {
                    preferencesDatTemp.OutputResultFile = userControlPreferencesTabOnFormMain.textBoxOutputResultFile.Text;
                }
                #endregion

                #region EpisodeMemoryカテゴリ---------------------------------------------------------------------------------
                {
                    preferencesDatTemp.EpisodeTargetNum = (int)userControlPreferencesTabOnFormMain.numericUpDownEpisodeTargetNum.Value;

                    preferencesDatTemp.EpisodeRandomCoordinate = userControlPreferencesTabOnFormMain.checkBoxEpisodeCoordinateRandom.Checked;
                    preferencesDatTemp.EpisodeRandomShape = userControlPreferencesTabOnFormMain.checkBoxEpisodeShapeRandom.Checked;
                    preferencesDatTemp.EpisodeRandomColor = userControlPreferencesTabOnFormMain.checkBoxEpisodeColorRandom.Checked;
                    preferencesDatTemp.EpTarget1FixX = (int)userControlPreferencesTabOnFormMain.numericUpDownEpTarget1FixX.Value;
                    preferencesDatTemp.EpTarget2FixX = (int)userControlPreferencesTabOnFormMain.numericUpDownEpTarget2FixX.Value;
                    preferencesDatTemp.EpTarget3FixX = (int)userControlPreferencesTabOnFormMain.numericUpDownEpTarget3FixX.Value;
                    preferencesDatTemp.EpTarget4FixX = (int)userControlPreferencesTabOnFormMain.numericUpDownEpTarget4FixX.Value;
                    preferencesDatTemp.EpTarget5FixX = (int)userControlPreferencesTabOnFormMain.numericUpDownEpTarget5FixX.Value;
                    preferencesDatTemp.EpTarget1FixY = (int)userControlPreferencesTabOnFormMain.numericUpDownEpTarget1FixY.Value;
                    preferencesDatTemp.EpTarget2FixY = (int)userControlPreferencesTabOnFormMain.numericUpDownEpTarget2FixY.Value;
                    preferencesDatTemp.EpTarget3FixY = (int)userControlPreferencesTabOnFormMain.numericUpDownEpTarget3FixY.Value;
                    preferencesDatTemp.EpTarget4FixY = (int)userControlPreferencesTabOnFormMain.numericUpDownEpTarget4FixY.Value;
                    preferencesDatTemp.EpTarget5FixY = (int)userControlPreferencesTabOnFormMain.numericUpDownEpTarget5FixY.Value;

                    preferencesDatTemp.EpShapeFix1 = userControlPreferencesTabOnFormMain.comboBoxEpFixShape1.SelectedItem.ToString();
                    preferencesDatTemp.EpShapeFix2 = userControlPreferencesTabOnFormMain.comboBoxEpFixShape2.SelectedItem.ToString();
                    preferencesDatTemp.EpShapeFix3 = userControlPreferencesTabOnFormMain.comboBoxEpFixShape3.SelectedItem.ToString();
                    preferencesDatTemp.EpShapeFix4 = userControlPreferencesTabOnFormMain.comboBoxEpFixShape4.SelectedItem.ToString();
                    preferencesDatTemp.EpShapeFix5 = userControlPreferencesTabOnFormMain.comboBoxEpFixShape5.SelectedItem.ToString();

                    preferencesDatTemp.EpColorFix1 = userControlPreferencesTabOnFormMain.comboBoxEpFixColor1.SelectedItem.ToString();
                    preferencesDatTemp.EpColorFix2 = userControlPreferencesTabOnFormMain.comboBoxEpFixColor2.SelectedItem.ToString();
                    preferencesDatTemp.EpColorFix3 = userControlPreferencesTabOnFormMain.comboBoxEpFixColor3.SelectedItem.ToString();
                    preferencesDatTemp.EpColorFix4 = userControlPreferencesTabOnFormMain.comboBoxEpFixColor4.SelectedItem.ToString();
                    preferencesDatTemp.EpColorFix5 = userControlPreferencesTabOnFormMain.comboBoxEpFixColor5.SelectedItem.ToString();

                    preferencesDatTemp.EnableEpImageShape = userControlPreferencesTabOnFormMain.checkBoxEnableEpImageShape.Checked;
                    preferencesDatTemp.EpImageFileFolder = userControlPreferencesTabOnFormMain.textBoxEpImageFileFolder.Text;
                }
                #endregion

                #region MarmosetDetectionカテゴリ

                preferencesDatTemp.EnableMamosetDetection = userControlPreferencesTabOnFormMain.checkBoxMarmosetDetection.Checked;
                preferencesDatTemp.EnableMamosetDetectionSaveImageMode = userControlPreferencesTabOnFormMain.checkBoxDetectionSaveImageMarmosetDetection.Checked;
                preferencesDatTemp.EnableMamosetDetectionSaveOnlyMode = userControlPreferencesTabOnFormMain.checkBoxSaveOnlyImage.Checked;
                preferencesDatTemp.LearningSaveImageFolder = userControlPreferencesTabOnFormMain.textBoxLearningSaveImageFolder.Text;
                preferencesDatTemp.LearningModelPath = userControlPreferencesTabOnFormMain.textBoxModelFile.Text;
                // URIによる指定だが表示はCAM名指定
                if (userControlPreferencesTabOnFormMain.comboBoxCamUri.SelectedIndex >= 0)
                {
                    preferencesDatTemp.SelectCamUri = camImage.DeviceUris.ElementAt(userControlPreferencesTabOnFormMain.comboBoxCamUri.SelectedIndex).Uri.ToString();
                }
                preferencesDatTemp.ShotCamInterval = (int)userControlPreferencesTabOnFormMain.numericUpDownShotCamInterval.Value;
                preferencesDatTemp.DetectThreshold = (double)userControlPreferencesTabOnFormMain.numericUpDownMarmoDetectionThreshold.Value;
                #endregion

            }
            // 画像切り替えされた場合エピソード状態リセット
            if (preferencesDatOriginal.EnableEpImageShape != preferencesDatTemp.EnableEpImageShape ||
                preferencesDatOriginal.EpisodeRandomCoordinate != preferencesDatTemp.EpisodeRandomCoordinate ||
                preferencesDatOriginal.EpisodeRandomShape != preferencesDatTemp.EpisodeRandomShape ||
                preferencesDatOriginal.EpisodeRandomColor != preferencesDatTemp.EpisodeRandomColor ||
                preferencesDatTemp.EpisodeTargetNum != preferencesDatOriginal.EpisodeTargetNum)
            {
                idControlHelper.ClearEntry();
                episodeMemory.ClearEntry();
            }


            // 一時領域をオリジナルへコピー
            preferencesDatOriginal = preferencesDatTemp.Clone();
            // 設定をファイルへ保存
            if (SavePreference(preferencesDatOriginal) != true)
            {
                MessageBox.Show("Preference file save error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // シリアル・ポート・オープン FIX BaudRate StopBits
            serialHelperPort.ComPort = preferencesDatOriginal.ComPort;
            //serialHelperPort.BaudRate = preferencesDatOriginal.ComBaudRate;
            serialHelperPort.BaudRate = "9600";
            serialHelperPort.DataBits = preferencesDatOriginal.ComDataBitLength;
            //serialHelperPort.StopBits = preferencesDatOriginal.ComStopBitLength;
            serialHelperPort.StopBits = "Two";
            serialHelperPort.Handshake = System.IO.Ports.Handshake.None.ToString();
            serialHelperPort.Parity = preferencesDatOriginal.ComParity;
            if (serialHelperPort.Open() != true)
            {
                serialPortOpenFlag = false;
                MessageBox.Show("COM port open error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                serialPortOpenFlag = true;
            }

            // 設定値反映クラス化弊害
            opImage.UpdatePreferences(preferencesDatOriginal);

            idControlHelper.ExpireTime = preferencesDatOriginal.EpisodeExpireTime;

            // CamImage 設定反映 簡易
            camImage.SetCamIntervalToFrame();

            // userControlMainOnFormMain表示
            VisibleUcMain();
        }
        private void buttonCancelOnUcPreferencesTab_Click(object sender, EventArgs e)
        {
            // シリアル・ポート・オープン
            serialHelperPort.ComPort = preferencesDatOriginal.ComPort;
            serialHelperPort.BaudRate = preferencesDatOriginal.ComBaudRate;
            serialHelperPort.DataBits = preferencesDatOriginal.ComDataBitLength;
            serialHelperPort.StopBits = preferencesDatOriginal.ComStopBitLength;
            serialHelperPort.Handshake = System.IO.Ports.Handshake.None.ToString();
            serialHelperPort.Parity = preferencesDatOriginal.ComParity;
            if (serialHelperPort.Open() != true)
            {
                serialPortOpenFlag = false;
                MessageBox.Show("COM port open error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                serialPortOpenFlag = true;
            }
            // userControlMainOnFormMain表示
            VisibleUcMain();
        }
        //----------------------------------------------------------------------------
        // Imageカテゴリ
        private void buttonTriggerImageFileOnUcPreferencesTab_Click(object sender, EventArgs e)
        {
            // 最初のファイル名を指定する(最初にに「ファイル名」で表示される文字列を指定)
            userControlPreferencesTabOnFormMain.openFileDialogTriggerImageFile.FileName = "";
            // 最初に表示されるフォルダを指定 (指定しない（空の文字列）の時は、現在のディレクトリを表示)
            //			userControlPreferencesTabOnFormMain.openFileDialogTriggerImageFile.InitialDirectory = @"C:\";
            userControlPreferencesTabOnFormMain.openFileDialogTriggerImageFile.InitialDirectory = "";
            // [ファイルの種類]に表示される選択肢を指定 (指定しないとすべてのファイルを表示)
            userControlPreferencesTabOnFormMain.openFileDialogTriggerImageFile.Filter = "JPG(*.jpg)|*.jpg|PNG(*.png)|*.png|BMP(*.bmp)|*.bmp";
            // [ファイルの種類]ではじめに選択されるものを指定する (1番目の「JPGファイル」が選択されているようにする)
            userControlPreferencesTabOnFormMain.openFileDialogTriggerImageFile.FilterIndex = 1;
            // ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            userControlPreferencesTabOnFormMain.openFileDialogTriggerImageFile.RestoreDirectory = true;
            // 存在しないファイルの名前が指定されたとき警告を表示(デフォルトでTrueなので指定する必要はない)
            userControlPreferencesTabOnFormMain.openFileDialogTriggerImageFile.CheckFileExists = true;
            // 存在しないパスが指定されたとき警告を表示する(デフォルトでTrueなので指定する必要はない)
            userControlPreferencesTabOnFormMain.openFileDialogTriggerImageFile.CheckPathExists = true;

            if (userControlPreferencesTabOnFormMain.openFileDialogTriggerImageFile.ShowDialog() == DialogResult.OK)
            {
                String stringExtension = Path.GetExtension(userControlPreferencesTabOnFormMain.openFileDialogTriggerImageFile.FileName);
                if (stringExtension == "jpg" ||
                    stringExtension == "png" ||
                    stringExtension == "bmp")
                {
                }

                //OKボタンがクリックされた時、ファイル名を取得
                userControlPreferencesTabOnFormMain.textBoxTriggerImage.Text = userControlPreferencesTabOnFormMain.openFileDialogTriggerImageFile.FileName;
            }
        }

        private void buttonEpisodeTriggerImageFileOnUcPreferencesTab_Click(object sender, EventArgs e)
        {
            // 最初のファイル名を指定する(最初にに「ファイル名」で表示される文字列を指定)
            userControlPreferencesTabOnFormMain.openFileDialogEpisodeTriggerImageFile.FileName = "";
            // 最初に表示されるフォルダを指定 (指定しない（空の文字列）の時は、現在のディレクトリを表示)
            //			userControlPreferencesTabOnFormMain.openFileDialogTriggerImageFile.InitialDirectory = @"C:\";
            userControlPreferencesTabOnFormMain.openFileDialogEpisodeTriggerImageFile.InitialDirectory = "";
            // [ファイルの種類]に表示される選択肢を指定 (指定しないとすべてのファイルを表示)
            userControlPreferencesTabOnFormMain.openFileDialogEpisodeTriggerImageFile.Filter = "JPG(*.jpg)|*.jpg|PNG(*.png)|*.png|BMP(*.bmp)|*.bmp";
            // [ファイルの種類]ではじめに選択されるものを指定する (1番目の「JPGファイル」が選択されているようにする)
            userControlPreferencesTabOnFormMain.openFileDialogEpisodeTriggerImageFile.FilterIndex = 1;
            // ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            userControlPreferencesTabOnFormMain.openFileDialogEpisodeTriggerImageFile.RestoreDirectory = true;
            // 存在しないファイルの名前が指定されたとき警告を表示(デフォルトでTrueなので指定する必要はない)
            userControlPreferencesTabOnFormMain.openFileDialogEpisodeTriggerImageFile.CheckFileExists = true;
            // 存在しないパスが指定されたとき警告を表示する(デフォルトでTrueなので指定する必要はない)
            userControlPreferencesTabOnFormMain.openFileDialogEpisodeTriggerImageFile.CheckPathExists = true;

            if (userControlPreferencesTabOnFormMain.openFileDialogEpisodeTriggerImageFile.ShowDialog() == DialogResult.OK)
            {
                String stringExtension = Path.GetExtension(userControlPreferencesTabOnFormMain.openFileDialogEpisodeTriggerImageFile.FileName);
                if (stringExtension == "jpg" ||
                    stringExtension == "png" ||
                    stringExtension == "bmp")
                {
                }

                //OKボタンがクリックされた時、ファイル名を取得
                userControlPreferencesTabOnFormMain.textBoxEpTriggerImage.Text = userControlPreferencesTabOnFormMain.openFileDialogEpisodeTriggerImageFile.FileName;
            }
        }

        //----------------------------------------------------------------------------
        // CorrectCondition
        private void buttonCorrectImageFolder_Click(object sender, EventArgs e)
        {
            // 最初に表示されるフォルダを指定 (指定しない（空の文字列）の時は、現在のディレクトリを表示)
            //			userControlPreferencesTabOnFormMain.openFileDialogTriggerImageFile.InitialDirectory = @"C:\";
            if (userControlPreferencesTabOnFormMain.textBoxImageFileFolder.Text != "" && preferencesDatOriginal.ImageFileFolder != "")
            {
                userControlPreferencesTabOnFormMain.folderBrowserDialogCorrectImage.SelectedPath = preferencesDatOriginal.ImageFileFolder;
            }

            userControlPreferencesTabOnFormMain.folderBrowserDialogCorrectImage.ShowNewFolderButton = false;
            try
            {

                if (userControlPreferencesTabOnFormMain.folderBrowserDialogCorrectImage.ShowDialog() == DialogResult.OK)
                {
                    //OKボタンがクリックされた時、ファイル名を取得
                    userControlPreferencesTabOnFormMain.textBoxImageFileFolder.Text = userControlPreferencesTabOnFormMain.folderBrowserDialogCorrectImage.SelectedPath;

                    //フォルダ内にイメージファイルがない場合は警告
                    //大きいサイズ読み込まないようにもする
                    string[] patterns = ImageLoader.SupportExtension;
                    var folder = userControlPreferencesTabOnFormMain.folderBrowserDialogCorrectImage.SelectedPath;
                    var fileList = Directory.GetFiles(folder).Where(f =>
                    {
                        const int fileMaxSize = 40000000;   //最大サイズ40MB
                        return (new FileInfo(f).Length < fileMaxSize) && patterns.Any(pattern => f.ToLower().EndsWith(pattern));
                    }).ToList();
                    if (fileList?.Count == 0)
                    {
                        throw new Exception("Not found image files or found 40MB larger size file only.");
                    }

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
                // 設定画面キャンセルする
                buttonCancelOnUcPreferencesTab_Click(sender, e);
            }
        }
        private void buttonEpisodeImageFolder_Click(object sender, EventArgs e)
        {
            // 最初に表示されるフォルダを指定 (指定しない（空の文字列）の時は、現在のディレクトリを表示)
            //			userControlPreferencesTabOnFormMain.openFileDialogTriggerImageFile.InitialDirectory = @"C:\";
            if (userControlPreferencesTabOnFormMain.textBoxEpImageFileFolder.Text != "" && preferencesDatOriginal.EpImageFileFolder != "")
            {
                userControlPreferencesTabOnFormMain.folderBrowserDialogEpisodeImage.SelectedPath = preferencesDatOriginal.EpImageFileFolder;
            }

            userControlPreferencesTabOnFormMain.folderBrowserDialogEpisodeImage.ShowNewFolderButton = false;
            try
            {

                if (userControlPreferencesTabOnFormMain.folderBrowserDialogEpisodeImage.ShowDialog() == DialogResult.OK)
                {
                    //OKボタンがクリックされた時、ファイル名を取得
                    userControlPreferencesTabOnFormMain.textBoxEpImageFileFolder.Text = userControlPreferencesTabOnFormMain.folderBrowserDialogEpisodeImage.SelectedPath;

                    //フォルダ内にイメージファイルがない場合は警告
                    //大きいサイズ読み込まないようにもする
                    string[] patterns = ImageLoader.SupportExtension;
                    var folder = userControlPreferencesTabOnFormMain.folderBrowserDialogEpisodeImage.SelectedPath;
                    var fileList = Directory.GetFiles(folder).Where(f =>
                    {
                        const int fileMaxSize = 40000000;   //最大サイズ40MB
                        return (new FileInfo(f).Length < fileMaxSize) && patterns.Any(pattern => f.ToLower().EndsWith(pattern));
                    }).ToList();
                    if (fileList?.Count == 0)
                    {
                        throw new Exception("Not found image files or found 40MB larger size file only.");
                    }

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
                // 設定画面キャンセルする
                buttonCancelOnUcPreferencesTab_Click(sender, e);
            }
        }
        private void buttonCorrectImage_Click(object sender, EventArgs e)
        {
            if (preferencesDatOriginal.CorrectImageFile == "")
            {
                userControlPreferencesTabOnFormMain.openFileDialogCorrectImage.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            }
            else
            {
                userControlPreferencesTabOnFormMain.openFileDialogCorrectImage.InitialDirectory = Path.GetDirectoryName(preferencesDatOriginal.CorrectImageFile);
                userControlPreferencesTabOnFormMain.openFileDialogCorrectImage.FileName = preferencesDatOriginal.CorrectImageFile;
            }
            // [ファイルの種類]に表示される選択肢を指定 (指定しないとすべてのファイルを表示)
            userControlPreferencesTabOnFormMain.openFileDialogCorrectImage.Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.PNG|All files (*.*)|*.*";
            // [ファイルの種類]ではじめに選択されるものを指定する (1番目の「JPGファイル」が選択されているようにする)
            userControlPreferencesTabOnFormMain.openFileDialogCorrectImage.FilterIndex = 1;
            // ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            userControlPreferencesTabOnFormMain.openFileDialogCorrectImage.RestoreDirectory = true;
            // 存在しないファイルの名前が指定されたとき警告を表示(デフォルトでTrueなので指定する必要はない)
            userControlPreferencesTabOnFormMain.openFileDialogCorrectImage.CheckFileExists = true;
            // 存在しないパスが指定されたとき警告を表示する(デフォルトでTrueなので指定する必要はない)
            userControlPreferencesTabOnFormMain.openFileDialogCorrectImage.CheckPathExists = true;

            if (userControlPreferencesTabOnFormMain.openFileDialogCorrectImage.ShowDialog() == DialogResult.OK)
            {
                String stringExtension = Path.GetExtension(userControlPreferencesTabOnFormMain.openFileDialogCorrectImage.FileName);
                if (stringExtension == "jpg" ||
                    stringExtension == "png" ||
                    stringExtension == "bmp")
                {
                }

                //OKボタンがクリックされた時、ファイル名を取得
                userControlPreferencesTabOnFormMain.textBoxCorrectImage.Text = userControlPreferencesTabOnFormMain.openFileDialogCorrectImage.FileName;
            }
        }
        //----------------------------------------------------------------------------
        // Soundカテゴリ
        private void buttonSoundFileOfEndOnUcPreferencesTab_Click(object sender, EventArgs e)
        {
            // 最初のファイル名を指定する(最初にに「ファイル名」で表示される文字列を指定)
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfEnd.FileName = "";
            // 最初に表示されるフォルダを指定 (指定しない（空の文字列）の時は、現在のディレクトリを表示)
            //			userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfEnd.InitialDirectory = @"C:\";
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfEnd.InitialDirectory = "";
            // [ファイルの種類]に表示される選択肢を指定 (指定しないとすべてのファイルを表示)
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfEnd.Filter = "WAV(*.wav)|*.wav";
            // [ファイルの種類]ではじめに選択されるものを指定する (1番目の「WAVファイル」が選択されているようにする)
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfEnd.FilterIndex = 1;
            // ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfEnd.RestoreDirectory = true;
            // 存在しないファイルの名前が指定されたとき警告を表示(デフォルトでTrueなので指定する必要はない)
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfEnd.CheckFileExists = true;
            // 存在しないパスが指定されたとき警告を表示する(デフォルトでTrueなので指定する必要はない)
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfEnd.CheckPathExists = true;

            string path = Path.GetDirectoryName(userControlPreferencesTabOnFormMain.textBoxSoundOfEnd.Text);
            if (path != "")
            {
                userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfEnd.InitialDirectory = path;
            }

            if (userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfEnd.ShowDialog() == DialogResult.OK)
            {
                //OKボタンがクリックされた時、ファイル名を取得
                userControlPreferencesTabOnFormMain.textBoxSoundOfEnd.Text = userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfEnd.FileName;
            }
        }
        private void buttonSoundFileOfRewardOnUcPreferencesTab_Click(object sender, EventArgs e)
        {
            // 最初のファイル名を指定する(最初にに「ファイル名」で表示される文字列を指定)
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfReward.FileName = "";
            // 最初に表示されるフォルダを指定 (指定しない（空の文字列）の時は、現在のディレクトリを表示)
            //			userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfReward.InitialDirectory = @"C:\";
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfReward.InitialDirectory = "";
            // [ファイルの種類]に表示される選択肢を指定 (指定しないとすべてのファイルを表示)
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfReward.Filter = "WAV(*.wav)|*.wav";
            // [ファイルの種類]ではじめに選択されるものを指定する (1番目の「WAVファイル」が選択されているようにする)
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfReward.FilterIndex = 1;
            // ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfReward.RestoreDirectory = true;
            // 存在しないファイルの名前が指定されたとき警告を表示(デフォルトでTrueなので指定する必要はない)
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfReward.CheckFileExists = true;
            // 存在しないパスが指定されたとき警告を表示する(デフォルトでTrueなので指定する必要はない)
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfReward.CheckPathExists = true;

            string path = Path.GetDirectoryName(userControlPreferencesTabOnFormMain.textBoxSoundOfReward.Text);
            if (path != "")
            {
                userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfReward.InitialDirectory = path;
            }

            if (userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfReward.ShowDialog() == DialogResult.OK)
            {
                //OKボタンがクリックされた時、ファイル名を取得
                userControlPreferencesTabOnFormMain.textBoxSoundOfReward.Text = userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfReward.FileName;
            }
        }
        private void buttonSoundFileOfCorrectOnUcPreferencesTab_Click(object sender, EventArgs e)
        {
            string path = "";
            // 最初のファイル名を指定する(最初にに「ファイル名」で表示される文字列を指定)
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfCorrect.FileName = "";
            // 最初に表示されるフォルダを指定 (指定しない（空の文字列）の時は、現在のディレクトリを表示)
            //			userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfReward.InitialDirectory = @"C:\";
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfCorrect.InitialDirectory = "";
            // [ファイルの種類]に表示される選択肢を指定 (指定しないとすべてのファイルを表示)
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfCorrect.Filter = "WAV(*.wav)|*.wav";
            // [ファイルの種類]ではじめに選択されるものを指定する (1番目の「WAVファイル」が選択されているようにする)
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfCorrect.FilterIndex = 1;
            // ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfCorrect.RestoreDirectory = true;
            // 存在しないファイルの名前が指定されたとき警告を表示(デフォルトでTrueなので指定する必要はない)
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfCorrect.CheckFileExists = true;
            // 存在しないパスが指定されたとき警告を表示する(デフォルトでTrueなので指定する必要はない)
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfCorrect.CheckPathExists = true;
            if (userControlPreferencesTabOnFormMain.textBoxSoundOfCorrect.Text == "")
            {
                path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
            else
            {
                path = Path.GetDirectoryName(userControlPreferencesTabOnFormMain.textBoxSoundOfCorrect.Text);
            }
            if (path != "")
            {
                userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfCorrect.InitialDirectory = path;
            }

            if (userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfCorrect.ShowDialog() == DialogResult.OK)
            {
                //OKボタンがクリックされた時、ファイル名を取得
                userControlPreferencesTabOnFormMain.textBoxSoundOfCorrect.Text = userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfCorrect.FileName;
            }
        }
        private void buttonSoundFileOfIncorrectOnUcPreferencesTab_Click(object sender, EventArgs e)
        {
            string path = "";
            // 最初のファイル名を指定する(最初にに「ファイル名」で表示される文字列を指定)
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfIncorrect.FileName = "";
            // 最初に表示されるフォルダを指定 (指定しない（空の文字列）の時は、現在のディレクトリを表示)
            //			userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfReward.InitialDirectory = @"C:\";
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfIncorrect.InitialDirectory = "";
            // [ファイルの種類]に表示される選択肢を指定 (指定しないとすべてのファイルを表示)
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfIncorrect.Filter = "WAV(*.wav)|*.wav";
            // [ファイルの種類]ではじめに選択されるものを指定する (1番目の「WAVファイル」が選択されているようにする)
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfIncorrect.FilterIndex = 1;
            // ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfIncorrect.RestoreDirectory = true;
            // 存在しないファイルの名前が指定されたとき警告を表示(デフォルトでTrueなので指定する必要はない)
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfIncorrect.CheckFileExists = true;
            // 存在しないパスが指定されたとき警告を表示する(デフォルトでTrueなので指定する必要はない)
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfIncorrect.CheckPathExists = true;
            if (userControlPreferencesTabOnFormMain.textBoxSoundOfIncorrect.Text == "")
            {
                path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
            else
            {
                path = Path.GetDirectoryName(userControlPreferencesTabOnFormMain.textBoxSoundOfIncorrect.Text);
            }
            if (path != "")
            {
                userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfIncorrect.InitialDirectory = path;
            }

            if (userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfIncorrect.ShowDialog() == DialogResult.OK)
            {
                //OKボタンがクリックされた時、ファイル名を取得
                userControlPreferencesTabOnFormMain.textBoxSoundOfIncorrect.Text = userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfIncorrect.FileName;
            }
        }
        private void buttonSoundFileOfIncorrectRewardOnUcPreferencesTab_Click(object sender, EventArgs e)
        {
            string path = "";
            // 最初のファイル名を指定する(最初にに「ファイル名」で表示される文字列を指定)
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfIncorrectReward.FileName = "";
            // 最初に表示されるフォルダを指定 (指定しない（空の文字列）の時は、現在のディレクトリを表示)
            //			userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfReward.InitialDirectory = @"C:\";
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfIncorrectReward.InitialDirectory = "";
            // [ファイルの種類]に表示される選択肢を指定 (指定しないとすべてのファイルを表示)
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfIncorrectReward.Filter = "WAV(*.wav)|*.wav";
            // [ファイルの種類]ではじめに選択されるものを指定する (1番目の「WAVファイル」が選択されているようにする)
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfIncorrectReward.FilterIndex = 1;
            // ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfIncorrectReward.RestoreDirectory = true;
            // 存在しないファイルの名前が指定されたとき警告を表示(デフォルトでTrueなので指定する必要はない)
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfIncorrectReward.CheckFileExists = true;
            // 存在しないパスが指定されたとき警告を表示する(デフォルトでTrueなので指定する必要はない)
            userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfIncorrectReward.CheckPathExists = true;
            if (userControlPreferencesTabOnFormMain.textBoxSoundOfIncorrectReward.Text == "")
            {
                path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
            else
            {
                path = Path.GetDirectoryName(userControlPreferencesTabOnFormMain.textBoxSoundOfIncorrectReward.Text);
            }
            if (path != "")
            {
                userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfIncorrectReward.InitialDirectory = path;
            }

            if (userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfIncorrectReward.ShowDialog() == DialogResult.OK)
            {
                //OKボタンがクリックされた時、ファイル名を取得
                userControlPreferencesTabOnFormMain.textBoxSoundOfIncorrectReward.Text = userControlPreferencesTabOnFormMain.openFileDialogSoundFileOfIncorrectReward.FileName;
            }
        }
        //----------------------------------------------------------------------------
        // Outputカテゴリ
        private void buttonOutputResultFileOnUcPreferencesTab_Click(object sender, EventArgs e)
        {
            // 最初のファイル名を指定する(最初にに「ファイル名」で表示される文字列を指定)
            userControlPreferencesTabOnFormMain.openFileDialogOutputResultFile.FileName = "";
            // 最初に表示されるフォルダを指定 (指定しない（空の文字列）の時は、現在のディレクトリを表示)
            if (File.Exists(preferencesDatOriginal.OutputResultFile))
                userControlPreferencesTabOnFormMain.openFileDialogOutputResultFile.InitialDirectory = Path.GetDirectoryName(preferencesDatOriginal.OutputResultFile);
            else
                userControlPreferencesTabOnFormMain.openFileDialogOutputResultFile.InitialDirectory = "";
            // [ファイルの種類]に表示される選択肢を指定 (指定しないとすべてのファイルを表示)
            userControlPreferencesTabOnFormMain.openFileDialogOutputResultFile.Filter = "CSVファイル(*.csv)|*.csv|すべてのファイル(*.*)|*.*";
            // [ファイルの種類]ではじめに選択されるものを指定する (1番目の「CSVファイル」が選択されているようにする)
            userControlPreferencesTabOnFormMain.openFileDialogOutputResultFile.FilterIndex = 1;
            // ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            userControlPreferencesTabOnFormMain.openFileDialogOutputResultFile.RestoreDirectory = true;
            // 存在しないファイルの名前が指定されたとき警告しない
            userControlPreferencesTabOnFormMain.openFileDialogOutputResultFile.CheckFileExists = false;
            // 存在しないパスが指定されたとき警告を表示する(デフォルトでTrueなので指定する必要はない)
            userControlPreferencesTabOnFormMain.openFileDialogOutputResultFile.CheckPathExists = true;

            if (userControlPreferencesTabOnFormMain.openFileDialogOutputResultFile.ShowDialog() == DialogResult.OK)
            {
                //OKボタンがクリックされた時、ファイル名を取得
                userControlPreferencesTabOnFormMain.textBoxOutputResultFile.Text = userControlPreferencesTabOnFormMain.openFileDialogOutputResultFile.FileName;
            }
        }

        private void buttonOpenDebugLogFolder_Click(object sender, EventArgs e)
        {
            var configFile = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.PerUserRoaming).FilePath;
            System.Diagnostics.Process.Start("explorer", Path.GetDirectoryName(Application.UserAppDataPath));
        }
        private void buttonOpenExecuteFolder_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer", Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
        }

        private void buttonOpenResultLogFolder_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer", Path.GetDirectoryName(preferencesDatOriginal.OutputResultFile));
        }

        private void buttonCredit_Click(object sender, EventArgs e)
        {
            using (FormCredit fc = new FormCredit())
            {
                fc.ShowDialog();
            }
        }
        //----------------------------------------------------------------------------
        // MamosetDetectカテゴリ
        private void buttonLearningImageFolder_Click(object sender, EventArgs e)
        {
            if (userControlPreferencesTabOnFormMain.textBoxLearningSaveImageFolder.Text != "" && preferencesDatOriginal.LearningSaveImageFolder != "")
            {
                userControlPreferencesTabOnFormMain.folderBrowserDialogLearningSaveImageFolder.SelectedPath = preferencesDatOriginal.LearningSaveImageFolder;
            }
            if (userControlPreferencesTabOnFormMain.folderBrowserDialogLearningSaveImageFolder.ShowDialog() == DialogResult.OK)
            {
                //OKボタンがクリックされた時、ファイル名を取得
                userControlPreferencesTabOnFormMain.textBoxLearningSaveImageFolder.Text = userControlPreferencesTabOnFormMain.folderBrowserDialogLearningSaveImageFolder.SelectedPath;

            }
        }
        private void buttonModelFile_Click(object sender, EventArgs e)
        {
            if (preferencesDatOriginal.LearningModelPath == "")
            {
                userControlPreferencesTabOnFormMain.openFileDialogCorrectImage.InitialDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
            else
            {
                userControlPreferencesTabOnFormMain.openFileDialogCorrectImage.InitialDirectory = Path.GetDirectoryName(preferencesDatOriginal.LearningModelPath);
                userControlPreferencesTabOnFormMain.openFileDialogCorrectImage.FileName = preferencesDatOriginal.LearningModelPath;
            }
            // [ファイルの種類]に表示される選択肢を指定 (指定しないとすべてのファイルを表示)
            userControlPreferencesTabOnFormMain.openFileDialogModelFile.Filter = "Model Files(*.weights)|*.weights|All files (*.*)|*.*";
            // [ファイルの種類]ではじめに選択されるものを指定する (1番目のファイルが選択されているようにする)
            userControlPreferencesTabOnFormMain.openFileDialogModelFile.FilterIndex = 1;
            // ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            userControlPreferencesTabOnFormMain.openFileDialogModelFile.RestoreDirectory = true;
            // 存在しないファイルの名前が指定されたとき警告を表示(デフォルトでTrueなので指定する必要はない)
            userControlPreferencesTabOnFormMain.openFileDialogModelFile.CheckFileExists = true;
            // 存在しないパスが指定されたとき警告を表示する(デフォルトでTrueなので指定する必要はない)
            userControlPreferencesTabOnFormMain.openFileDialogModelFile.CheckPathExists = true;

            if (userControlPreferencesTabOnFormMain.openFileDialogModelFile.ShowDialog() == DialogResult.OK)
            {
                String stringExtension = Path.GetExtension(userControlPreferencesTabOnFormMain.openFileDialogModelFile.FileName);
                if (stringExtension == "weights")
                {
                }
                //OKボタンがクリックされた時、ファイル名を取得
                userControlPreferencesTabOnFormMain.textBoxModelFile.Text = userControlPreferencesTabOnFormMain.openFileDialogModelFile.FileName;
            }

        }


        // userControlPreferencesOnFormMainがVisibleとなった時、設定を一時領域へコピーし

        private void userControlPreferencesTabOnFormMain_VisibleChanged(object sender, EventArgs e)
        {
            if (userControlPreferencesTabOnFormMain.Visible == true)
            {
                // 一時変数へクローン
                preferencesDatTemp = preferencesDatOriginal.Clone();
                //=============================================================================================
                // 初期値設定
                #region Compartmentカテゴリ------------------------------------------------------------------------
                {
                    userControlPreferencesTabOnFormMain.numericUpDownCompartmentNo.Value = (decimal)preferencesDatTemp.CompartmentNo;
                    userControlPreferencesTabOnFormMain.numericUpDownCompartmentNo.Text = preferencesDatTemp.CompartmentNo.ToString();
                }
                #endregion

                #region ID codeカテゴリ----------------------------------------------------------------------------
                {
                    // COM portコンボ・ボックス選択候補設定
                    {
                        // 項目クリア
                        userControlPreferencesTabOnFormMain.comboBoxComPort.Items.Clear();
                        String[] stringComPort = GetSerialDeviceNames();

                        if (stringComPort != null && stringComPort[0] != String.Empty)
                        {
                            foreach (String l_stringComPort in stringComPort)
                            {
                                userControlPreferencesTabOnFormMain.comboBoxComPort.Items.Add(GetSerialPortName(l_stringComPort));
                            }
                        }
                        // 初期値:未設定状態
                        userControlPreferencesTabOnFormMain.comboBoxComPort.SelectedIndex = -1;
                    }


                    // Baudrateコンボ・ボックス選択候補設定
                    userControlPreferencesTabOnFormMain.comboBoxComBaurate.Items.Clear();
                    userControlPreferencesTabOnFormMain.comboBoxComBaurate.Items.Add("1200");
                    userControlPreferencesTabOnFormMain.comboBoxComBaurate.Items.Add("2400");
                    userControlPreferencesTabOnFormMain.comboBoxComBaurate.Items.Add("4800");
                    userControlPreferencesTabOnFormMain.comboBoxComBaurate.Items.Add("9600");
                    userControlPreferencesTabOnFormMain.comboBoxComBaurate.Items.Add("19200");
                    userControlPreferencesTabOnFormMain.comboBoxComBaurate.Items.Add("38400");
                    userControlPreferencesTabOnFormMain.comboBoxComBaurate.Items.Add("57600");
                    // ComDataBitLengthコンボ・ボックス選択候補設定
                    userControlPreferencesTabOnFormMain.comboBoxComDataBitLength.Items.Clear();
                    userControlPreferencesTabOnFormMain.comboBoxComDataBitLength.Items.Add("7");
                    userControlPreferencesTabOnFormMain.comboBoxComDataBitLength.Items.Add("8");
                    // ComStopBitLengthコンボ・ボックス選択候補設定
                    userControlPreferencesTabOnFormMain.comboBoxComStopBitLength.Items.Clear();
                    userControlPreferencesTabOnFormMain.comboBoxComStopBitLength.Items.Add(System.IO.Ports.StopBits.One.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxComStopBitLength.Items.Add(System.IO.Ports.StopBits.OnePointFive.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxComStopBitLength.Items.Add(System.IO.Ports.StopBits.One.ToString());
                    // ComStopBitLengthコンボ・ボックス選択候補設定
                    userControlPreferencesTabOnFormMain.comboBoxComParity.Items.Clear();
                    userControlPreferencesTabOnFormMain.comboBoxComParity.Items.Add(System.IO.Ports.Parity.None.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxComParity.Items.Add(System.IO.Ports.Parity.Odd.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxComParity.Items.Add(System.IO.Ports.Parity.Even.ToString());
                    // 初期値設定
                    userControlPreferencesTabOnFormMain.comboBoxComPort.SelectedItem = preferencesDatTemp.ComPort;
                    userControlPreferencesTabOnFormMain.comboBoxComBaurate.SelectedItem = preferencesDatTemp.ComBaudRate;
                    userControlPreferencesTabOnFormMain.comboBoxComDataBitLength.SelectedItem = preferencesDatTemp.ComDataBitLength;
                    userControlPreferencesTabOnFormMain.comboBoxComStopBitLength.SelectedItem = preferencesDatTemp.ComStopBitLength;
                    userControlPreferencesTabOnFormMain.comboBoxComParity.SelectedItem = preferencesDatTemp.ComParity;
                }
                #endregion

                //----------------------------------------------------------------------------
                #region Operationカテゴリ
                {
                    {
                        // TypeOfTask設定
                        ECpTask eCpTaskTypeOfTask;

                        // TypeOfTaskコンボ・ボックス選択候補設定
                        userControlPreferencesTabOnFormMain.comboBoxTypeOfTask.Items.Clear();
                        userControlPreferencesTabOnFormMain.comboBoxTypeOfTask.Items.Add(ECpTask.Training.ToString());
                        userControlPreferencesTabOnFormMain.comboBoxTypeOfTask.Items.Add(ECpTask.DelayMatch.ToString());
                        userControlPreferencesTabOnFormMain.comboBoxTypeOfTask.Items.Add(ECpTask.TrainingEasy.ToString());
                        userControlPreferencesTabOnFormMain.comboBoxTypeOfTask.Items.Add(ECpTask.UnConditionalFeeding.ToString());
                        // 設定が異常の時、Trainingとする
                        if (opImage.ConvertStringToCpTask(preferencesDatTemp.OpeTypeOfTask, out eCpTaskTypeOfTask) != true)
                        {
                            eCpTaskTypeOfTask = ECpTask.Training;
                            preferencesDatTemp.OpeTypeOfTask = eCpTaskTypeOfTask.ToString();
                        }
                        userControlPreferencesTabOnFormMain.comboBoxTypeOfTask.SelectedItem = preferencesDatTemp.OpeTypeOfTask;
                    }
                    userControlPreferencesTabOnFormMain.numericUpDownNumberOfTrial.Value = (decimal)preferencesDatTemp.OpeNumberOfTrial;
                    userControlPreferencesTabOnFormMain.numericUpDownNumberOfTrial.Text = preferencesDatTemp.OpeNumberOfTrial.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownTimeToDisplayCorrectImage.Value = (decimal)preferencesDatTemp.OpeTimeToDisplayCorrectImage;
                    userControlPreferencesTabOnFormMain.numericUpDownTimeToDisplayCorrectImage.Text = preferencesDatTemp.OpeTimeToDisplayCorrectImage.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownTimeToDisplayNoImage.Value = (decimal)preferencesDatTemp.OpeTimeToDisplayNoImage;
                    userControlPreferencesTabOnFormMain.numericUpDownTimeToDisplayNoImage.Text = preferencesDatTemp.OpeTimeToDisplayNoImage.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownIntervalTimeMinimum.Value = (decimal)preferencesDatTemp.OpeIntervalTimeMinimum;
                    userControlPreferencesTabOnFormMain.numericUpDownIntervalTimeMinimum.Text = preferencesDatTemp.OpeIntervalTimeMinimum.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownIntervalTimeMaximum.Value = (decimal)preferencesDatTemp.OpeIntervalTimeMaximum;
                    userControlPreferencesTabOnFormMain.numericUpDownIntervalTimeMaximum.Text = preferencesDatTemp.OpeIntervalTimeMaximum.ToString();

                    userControlPreferencesTabOnFormMain.checkBoxEnableRandomTime.Checked = (bool)preferencesDatTemp.EnableRandomTime;

                    userControlPreferencesTabOnFormMain.numericUpDownRandomTimeMin.Value = (decimal)preferencesDatTemp.OpeRandomTimeMinimum;
                    userControlPreferencesTabOnFormMain.numericUpDownRandomTimeMin.Text = preferencesDatTemp.OpeRandomTimeMinimum.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownRandomTimeMax.Value = (decimal)preferencesDatTemp.OpeRandomTimeMaximum;
                    userControlPreferencesTabOnFormMain.numericUpDownRandomTimeMax.Text = preferencesDatTemp.OpeRandomTimeMaximum.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownBoxFeedingRate.Value = (decimal)preferencesDatTemp.OpeFeedingRate;
                    userControlPreferencesTabOnFormMain.numericUpDownBoxFeedingRate.Text = preferencesDatTemp.OpeFeedingRate.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownTimeToFeed.Value = (decimal)preferencesDatTemp.OpeTimeToFeed;
                    userControlPreferencesTabOnFormMain.numericUpDownTimeToFeed.Text = preferencesDatTemp.OpeTimeToFeed.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownDelayFeed.Value = (decimal)preferencesDatTemp.OpeDelayFeed;
                    userControlPreferencesTabOnFormMain.numericUpDownDelayFeed.Text = preferencesDatTemp.OpeDelayFeed.ToString();

                    userControlPreferencesTabOnFormMain.checkBoxEnableFeedLamp.Checked = (bool)preferencesDatTemp.EnableFeedLamp;

                    userControlPreferencesTabOnFormMain.numericUpDownDelayFeedLamp.Value = (decimal)preferencesDatTemp.OpeDelayFeedLamp;
                    userControlPreferencesTabOnFormMain.numericUpDownDelayFeedLamp.Text = preferencesDatTemp.OpeDelayFeedLamp.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfStart.Value = (decimal)preferencesDatTemp.OpeTimeoutOfStart;
                    userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfStart.Text = preferencesDatTemp.OpeTimeoutOfStart.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfTrial.Value = (decimal)preferencesDatTemp.OpeTimeoutOfTrial;
                    userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfTrial.Text = preferencesDatTemp.OpeTimeoutOfTrial.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownMonitorSaveTime.Value = (decimal)preferencesDatTemp.MonitorSaveTime;
                    userControlPreferencesTabOnFormMain.numericUpDownMonitorSaveTime.Text = preferencesDatTemp.MonitorSaveTime.ToString();

                    userControlPreferencesTabOnFormMain.checkBoxEnableMonitorSave.Checked = (bool)preferencesDatTemp.EnableMonitorSave;

                    userControlPreferencesTabOnFormMain.numericUpDownDelayRoomLampOn.Value = (decimal)preferencesDatTemp.OpeDelayRoomLampOnTime;
                    userControlPreferencesTabOnFormMain.numericUpDownDelayRoomLampOn.Text = preferencesDatTemp.OpeDelayRoomLampOnTime.ToString();

                    userControlPreferencesTabOnFormMain.checkBoxEnableReEntry.Checked = preferencesDatTemp.OpeEnableReEntry;

                    userControlPreferencesTabOnFormMain.numericUpDownReEntryTimeout.Value = (decimal)preferencesDatTemp.OpeReEntryTimeout;
                    userControlPreferencesTabOnFormMain.numericUpDownReEntryTimeout.Text = preferencesDatTemp.OpeReEntryTimeout.ToString();

                    userControlPreferencesTabOnFormMain.checkBoxEnableEpisodeMemory.Checked = preferencesDatTemp.EnableEpisodeMemory;

                    userControlPreferencesTabOnFormMain.checkBoxForceEpisodeMemory.Checked = preferencesDatTemp.ForceEpisodeMemory;

                    userControlPreferencesTabOnFormMain.numericUpDownTimeToFeedReward.Value = (decimal)preferencesDatTemp.OpeTimeToFeedReward;
                    userControlPreferencesTabOnFormMain.numericUpDownTimeToFeedReward.Text = preferencesDatTemp.OpeTimeToFeedReward.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownTimeToFeedRewardFirstTime.Value = (decimal)preferencesDatTemp.OpeTimeToFeedRewardFirstTime;
                    userControlPreferencesTabOnFormMain.numericUpDownTimeToFeedRewardFirstTime.Text = preferencesDatTemp.OpeTimeToFeedRewardFirstTime.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownExpireTime.Value = (decimal)preferencesDatTemp.EpisodeExpireTime;
                    userControlPreferencesTabOnFormMain.numericUpDownExpireTime.Text = preferencesDatTemp.EpisodeExpireTime.ToString();

                    userControlPreferencesTabOnFormMain.checkBoxEnableIncorrectRandom.Checked = preferencesDatTemp.EnableEpisodeIncorrectRandom;

                    userControlPreferencesTabOnFormMain.numericUpDownEpisodeCount.Value = preferencesDatTemp.EpisodeCount;
                    userControlPreferencesTabOnFormMain.numericUpDownEpisodeCount.Text = preferencesDatTemp.EpisodeCount.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownEpisodeTimezoneStart.Value = preferencesDatTemp.EpisodeTimezoneStartTime;
                    userControlPreferencesTabOnFormMain.numericUpDownEpisodeTimezoneStart.Text = preferencesDatTemp.EpisodeTimezoneStartTime.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownEpisodeTimezoneEnd.Value = preferencesDatTemp.EpisodeTimezoneEndTime;
                    userControlPreferencesTabOnFormMain.numericUpDownEpisodeTimezoneEnd.Text = preferencesDatTemp.EpisodeTimezoneEndTime.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownEpSelectTZStartTime.Value = preferencesDatTemp.EpisodeSelectShapeTimezoneStartTime;
                    userControlPreferencesTabOnFormMain.numericUpDownEpSelectTZStartTime.Text = preferencesDatTemp.EpisodeSelectShapeTimezoneStartTime.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownEpSelectTZEndTime.Value = preferencesDatTemp.EpisodeSelectShapeTimezoneEndTime;
                    userControlPreferencesTabOnFormMain.numericUpDownEpSelectTZEndTime.Text = preferencesDatTemp.EpisodeSelectShapeTimezoneEndTime.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownEpIntervalTime.Value = preferencesDatTemp.EpisodeIntervalTime;
                    userControlPreferencesTabOnFormMain.numericUpDownEpIntervalTime.Text = preferencesDatTemp.EpisodeIntervalTime.ToString();

                    userControlPreferencesTabOnFormMain.checkBoxTestIndicator.Checked = preferencesDatTemp.EnableTestIndicator;
                    userControlPreferencesTabOnFormMain.selectFileListIndicatorPath.FileName = preferencesDatTemp.TestIndicatorSvgPath;
                    {
                        // IndicatorPositionコンボ・ボックス選択候補設定
                        userControlPreferencesTabOnFormMain.comboBoxIndicatorPosition.Items.Clear();
                        userControlPreferencesTabOnFormMain.comboBoxIndicatorPosition.Items.Add(ECpMarkerPosion.RightBottom.ToString());
                        userControlPreferencesTabOnFormMain.comboBoxIndicatorPosition.Items.Add(ECpMarkerPosion.RightTop.ToString());
                        userControlPreferencesTabOnFormMain.comboBoxIndicatorPosition.Items.Add(ECpMarkerPosion.LeftBottom.ToString());
                        userControlPreferencesTabOnFormMain.comboBoxIndicatorPosition.Items.Add(ECpMarkerPosion.LeftTop.ToString());
                        userControlPreferencesTabOnFormMain.comboBoxIndicatorPosition.SelectedIndex = preferencesDatTemp.IndicatorPosition;

                    }
                    {
                        userControlPreferencesTabOnFormMain.checkBoxNoIDOperation.Checked = preferencesDatTemp.EnableNoIDOperation;
                        userControlPreferencesTabOnFormMain.checkBoxNoEntryIDOperation.Checked = preferencesDatTemp.EnableNoEntryIDOperation;
                    }
                }
                #endregion

                #region Imageカテゴリ:-----------------------------------------------------------------------------
                {
                    {
                        // BackColor設定
                        // BackColorコンボ・ボックス選択候補設定
                        userControlPreferencesTabOnFormMain.comboBoxBackColor.Items.Clear();
                        userControlPreferencesTabOnFormMain.comboBoxBackColor.Items.Add(ECpColor.Black.ToString());
                        userControlPreferencesTabOnFormMain.comboBoxBackColor.Items.Add(ECpColor.White.ToString());
                        userControlPreferencesTabOnFormMain.comboBoxBackColor.Items.Add(ECpColor.Red.ToString());
                        userControlPreferencesTabOnFormMain.comboBoxBackColor.Items.Add(ECpColor.Green.ToString());
                        userControlPreferencesTabOnFormMain.comboBoxBackColor.Items.Add(ECpColor.Blue.ToString());
                        userControlPreferencesTabOnFormMain.comboBoxBackColor.Items.Add(ECpColor.Yellow.ToString());
#if RANDOM_ENABLE
						userControlPreferencesTabOnFormMain.comboBoxBackColor.Items.Add(ECpColor.Random.ToString());
#endif
                        // 設定が異常の時、Greenとする
                        //if (opImage.ConvertStringToCpColor(preferencesDatTemp.BackColor, out eCpColorBackColor) != true)
                        //{
                        //    eCpColorBackColor = ECpColor.Green;
                        //    preferencesDatTemp.BackColor = eCpColorBackColor.ToString();
                        //}
                        userControlPreferencesTabOnFormMain.comboBoxBackColor.SelectedItem = preferencesDatTemp.BackColor;
                    }

                    {
                        // EpisodeFirst用BackColor設定
                        // BackColorコンボ・ボックス選択候補設定
                        //userControlPreferencesTabOnFormMain.comboBoxEpFirstBackColor.Items.Clear();
                        //userControlPreferencesTabOnFormMain.comboBoxEpFirstBackColor.Items.Add(ECpColor.Black.ToString());
                        //userControlPreferencesTabOnFormMain.comboBoxEpFirstBackColor.Items.Add(ECpColor.White.ToString());
                        //userControlPreferencesTabOnFormMain.comboBoxEpFirstBackColor.Items.Add(ECpColor.Red.ToString());
                        //userControlPreferencesTabOnFormMain.comboBoxEpFirstBackColor.Items.Add(ECpColor.Green.ToString());
                        //userControlPreferencesTabOnFormMain.comboBoxEpFirstBackColor.Items.Add(ECpColor.Blue.ToString());
                        //userControlPreferencesTabOnFormMain.comboBoxEpFirstBackColor.Items.Add(ECpColor.Yellow.ToString());
#if RANDOM_ENABLE
						userControlPreferencesTabOnFormMain.comboBoxBackColor.Items.Add(ECpColor.Random.ToString());
#endif
                        // 設定が異常の時、Greenとする
                        //if (opImage.ConvertStringToCpColor(preferencesDatTemp.EpisodeFirstBackColor, out eCpColorEpBackColor) != true)
                        //{
                        //    eCpColorEpBackColor = ECpColor.Green;
                        //    preferencesDatTemp.EpisodeFirstBackColor = eCpColorEpBackColor.ToString();
                        //}
                        userControlPreferencesTabOnFormMain.comboBoxEpFirstBackColor.SelectedItem = Color.FromName(preferencesDatTemp.EpisodeFirstBackColor);
                    }

                    {
                        // EpisodeTest用BackColor設定
                        // BackColorコンボ・ボックス選択候補設定
                        //userControlPreferencesTabOnFormMain.comboBoxEpTestBackColor.Items.Clear();
                        //userControlPreferencesTabOnFormMain.comboBoxEpTestBackColor.Items.Add(ECpColor.Black.ToString());
                        //userControlPreferencesTabOnFormMain.comboBoxEpTestBackColor.Items.Add(ECpColor.White.ToString());
                        //userControlPreferencesTabOnFormMain.comboBoxEpTestBackColor.Items.Add(ECpColor.Red.ToString());
                        //userControlPreferencesTabOnFormMain.comboBoxEpTestBackColor.Items.Add(ECpColor.Green.ToString());
                        //userControlPreferencesTabOnFormMain.comboBoxEpTestBackColor.Items.Add(ECpColor.Blue.ToString());
                        //userControlPreferencesTabOnFormMain.comboBoxEpTestBackColor.Items.Add(ECpColor.Yellow.ToString());
#if RANDOM_ENABLE
						userControlPreferencesTabOnFormMain.comboBoxBackColor.Items.Add(ECpColor.Random.ToString());
#endif
                        // 設定が異常の時、Greenとする
                        //if (opImage.ConvertStringToCpColor(preferencesDatTemp.EpisodeTestBackColor, out eCpColorEpBackColor) != true)
                        //{
                        //    eCpColorEpBackColor = ECpColor.Green;
                        //    preferencesDatTemp.EpisodeTestBackColor = eCpColorEpBackColor.ToString();
                        //}
                        userControlPreferencesTabOnFormMain.comboBoxEpTestBackColor.SelectedItem = Color.FromName(preferencesDatTemp.EpisodeTestBackColor);
                    }

                    {
                        // DelayBackColor設定
                        // BackColorコンボ・ボックス選択候補設定
                        userControlPreferencesTabOnFormMain.comboBoxDelayBackColor.Items.Clear();
                        userControlPreferencesTabOnFormMain.comboBoxDelayBackColor.Items.Add(ECpColor.Black.ToString());
                        userControlPreferencesTabOnFormMain.comboBoxDelayBackColor.Items.Add(ECpColor.White.ToString());
                        userControlPreferencesTabOnFormMain.comboBoxDelayBackColor.Items.Add(ECpColor.Red.ToString());
                        userControlPreferencesTabOnFormMain.comboBoxDelayBackColor.Items.Add(ECpColor.Green.ToString());
                        userControlPreferencesTabOnFormMain.comboBoxDelayBackColor.Items.Add(ECpColor.Blue.ToString());
                        userControlPreferencesTabOnFormMain.comboBoxDelayBackColor.Items.Add(ECpColor.Yellow.ToString());
#if RANDOM_ENABLE
						userControlPreferencesTabOnFormMain.comboBoxDelayBackColor.Items.Add(ECpColor.Random.ToString());
#endif
                        // 設定が異常の時、Blackとする
                        //if (opImage.ConvertStringToCpColor(preferencesDatTemp.DelayBackColor, out eCpColorDelayBackColor) != true)
                        //{
                        //    eCpColorDelayBackColor = ECpColor.Black;
                        //    preferencesDatTemp.DelayBackColor = eCpColorDelayBackColor.ToString();
                        //}
                        userControlPreferencesTabOnFormMain.comboBoxDelayBackColor.SelectedItem = preferencesDatTemp.DelayBackColor;
                    }
                    {
                        // ShapeColor設定
                        // ShapeColorコンボ・ボックス選択候補設定
                        userControlPreferencesTabOnFormMain.comboBoxShapeColor.Items.Clear();
                        userControlPreferencesTabOnFormMain.comboBoxShapeColor.Items.Add(ECpColor.Black.ToString());
                        userControlPreferencesTabOnFormMain.comboBoxShapeColor.Items.Add(ECpColor.White.ToString());
                        userControlPreferencesTabOnFormMain.comboBoxShapeColor.Items.Add(ECpColor.Red.ToString());
                        userControlPreferencesTabOnFormMain.comboBoxShapeColor.Items.Add(ECpColor.Green.ToString());
                        userControlPreferencesTabOnFormMain.comboBoxShapeColor.Items.Add(ECpColor.Blue.ToString());
                        userControlPreferencesTabOnFormMain.comboBoxShapeColor.Items.Add(ECpColor.Yellow.ToString());
#if RANDOM_ENABLE
						userControlPreferencesTabOnFormMain.comboBoxShapeColor.Items.Add(ECpColor.Random.ToString());
#endif
                        // 設定が異常の時、Whiteとする
                        //if (opImage.ConvertStringToCpColor(preferencesDatTemp.ShapeColor, out eCpColorShapeColor) != true)
                        //{
                        //    eCpColorShapeColor = ECpColor.White;
                        //    preferencesDatTemp.ShapeColor = eCpColorShapeColor.ToString();
                        //}
                        userControlPreferencesTabOnFormMain.comboBoxShapeColor.SelectedItem = preferencesDatTemp.ShapeColor;
                    }
                    {
                        // TypeOfShape設定
                        ECpShape eCpShapeTypeOfShape;

                        // TypeOfShapeコンボ・ボックス選択候補設定
                        userControlPreferencesTabOnFormMain.comboBoxTypeOfShape.Items.Clear();
                        userControlPreferencesTabOnFormMain.comboBoxTypeOfShape.Items.Add(ECpShape.Circle.ToString());
                        userControlPreferencesTabOnFormMain.comboBoxTypeOfShape.Items.Add(ECpShape.Rectangle.ToString());
                        userControlPreferencesTabOnFormMain.comboBoxTypeOfShape.Items.Add(ECpShape.Triangle.ToString());
                        userControlPreferencesTabOnFormMain.comboBoxTypeOfShape.Items.Add(ECpShape.Star.ToString());
#if RANDOM_ENABLE
						userControlPreferencesTabOnFormMain.comboBoxTypeOfShape.Items.Add(ECpShape.Random.ToString());
#endif
                        // 設定が異常の時、Circleとする
                        if (opImage.ConvertStringToCpShape(preferencesDatTemp.TypeOfShape, out eCpShapeTypeOfShape) != true)
                        {
                            eCpShapeTypeOfShape = ECpShape.Circle;
                            preferencesDatTemp.TypeOfShape = eCpShapeTypeOfShape.ToString();
                        }
                        userControlPreferencesTabOnFormMain.comboBoxTypeOfShape.SelectedItem = preferencesDatTemp.TypeOfShape;
                    }
                    {
                        // SizeOfShapeInStep設定
                        ECpStep eCpStepSizeOfShapeInStep;

                        // SizeOfShapeInStepコンボ・ボックス選択候補設定
                        userControlPreferencesTabOnFormMain.comboBoxSizeOfShapeInStep.Items.Clear();
                        userControlPreferencesTabOnFormMain.comboBoxSizeOfShapeInStep.Items.Add(ECpStep.Step1.ToString());
                        userControlPreferencesTabOnFormMain.comboBoxSizeOfShapeInStep.Items.Add(ECpStep.Step2.ToString());
                        userControlPreferencesTabOnFormMain.comboBoxSizeOfShapeInStep.Items.Add(ECpStep.Step3.ToString());
                        userControlPreferencesTabOnFormMain.comboBoxSizeOfShapeInStep.Items.Add(ECpStep.Step4.ToString());
                        userControlPreferencesTabOnFormMain.comboBoxSizeOfShapeInStep.Items.Add(ECpStep.Step5.ToString());
                        // 設定が異常の時、Step1とする
                        if (opImage.ConvertStringToCpStep(preferencesDatTemp.SizeOfShapeInStep, out eCpStepSizeOfShapeInStep) != true)
                        {
                            eCpStepSizeOfShapeInStep = ECpStep.Step1;
                            preferencesDatTemp.SizeOfShapeInStep = eCpStepSizeOfShapeInStep.ToString();
                        }
                        userControlPreferencesTabOnFormMain.comboBoxSizeOfShapeInStep.SelectedItem = preferencesDatTemp.SizeOfShapeInStep;
                    }
                    // 初期値設定
                    userControlPreferencesTabOnFormMain.textBoxTriggerImage.Text = preferencesDatTemp.TriggerImageFile;
                    userControlPreferencesTabOnFormMain.textBoxEpTriggerImage.Text = preferencesDatTemp.EpisodeTriggerImageFile;

                    userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep1.Value = (decimal)preferencesDatTemp.SizeOfShapeInPixelForStep1;
                    userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep1.Text = preferencesDatTemp.SizeOfShapeInPixelForStep1.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep2.Value = (decimal)preferencesDatTemp.SizeOfShapeInPixelForStep2;
                    userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep2.Text = preferencesDatTemp.SizeOfShapeInPixelForStep2.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep3.Value = (decimal)preferencesDatTemp.SizeOfShapeInPixelForStep3;
                    userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep3.Text = preferencesDatTemp.SizeOfShapeInPixelForStep3.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep4.Value = (decimal)preferencesDatTemp.SizeOfShapeInPixelForStep4;
                    userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep4.Text = preferencesDatTemp.SizeOfShapeInPixelForStep4.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep5.Value = (decimal)preferencesDatTemp.SizeOfShapeInPixelForStep5;
                    userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixcelForStep5.Text = preferencesDatTemp.SizeOfShapeInPixelForStep5.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixel.Value = (decimal)preferencesDatTemp.SizeOfShapeInPixel;
                    userControlPreferencesTabOnFormMain.numericUpDownSizeOfShapeInPixel.Text = preferencesDatTemp.SizeOfShapeInPixel.ToString();

                }
                #endregion

                #region CorrectCondtionカテゴリ

                {
                    if (opImage != null)
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownCorrectFixX.Minimum = opImage.RectOpeImageValidArea.X;
                        userControlPreferencesTabOnFormMain.numericUpDownCorrectFixY.Minimum = opImage.RectOpeImageValidArea.Y;
                        userControlPreferencesTabOnFormMain.numericUpDownCorrectFixX.Maximum = opImage.WidthOfWholeArea;
                        userControlPreferencesTabOnFormMain.numericUpDownCorrectFixY.Maximum = opImage.HeightOfWholeArea;

                        userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX1.Minimum = opImage.RectOpeImageValidArea.X;
                        userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY1.Minimum = opImage.RectOpeImageValidArea.Y;
                        userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX1.Maximum = opImage.WidthOfWholeArea;
                        userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY1.Maximum = opImage.HeightOfWholeArea;

                        userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX2.Minimum = opImage.RectOpeImageValidArea.X;
                        userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY2.Minimum = opImage.RectOpeImageValidArea.Y;
                        userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX2.Maximum = opImage.WidthOfWholeArea;
                        userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY2.Maximum = opImage.HeightOfWholeArea;

                        userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX3.Minimum = opImage.RectOpeImageValidArea.X;
                        userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY3.Minimum = opImage.RectOpeImageValidArea.Y;
                        userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX3.Maximum = opImage.WidthOfWholeArea;
                        userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY3.Maximum = opImage.HeightOfWholeArea;

                        userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX4.Minimum = opImage.RectOpeImageValidArea.X;
                        userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY4.Minimum = opImage.RectOpeImageValidArea.Y;
                        userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX4.Maximum = opImage.WidthOfWholeArea;
                        userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY4.Maximum = opImage.HeightOfWholeArea;
                    }


                    if (!CheckRenderSize(opImage.OpeImageSizeOfShapeInPixel, preferencesDatOriginal.IncorrectNum))
                    {
                        while (!CheckRenderSize(opImage.OpeImageSizeOfShapeInPixel, preferencesDatOriginal.IncorrectNum--))
                        {

                        }
                    }

                    // CorrectCondition設定
                    ECpCorrectCondition eccc;
                    opImage.ConvertStringToCpCorrectCondtion(preferencesDatTemp.CorrectCondition.ToString(), out eccc);

                    if (eccc == ECpCorrectCondition.Coordinate)
                    {
                        userControlPreferencesTabOnFormMain.radioButtonCoordinate.Checked = true;
                    }
                    else if (eccc == ECpCorrectCondition.Color)
                    {
                        userControlPreferencesTabOnFormMain.radioButtonColor.Checked = true;
                    }
                    else if (eccc == ECpCorrectCondition.Shape)
                    {
                        userControlPreferencesTabOnFormMain.radioButtonShape.Checked = true;
                    }
                    else
                    {
                        userControlPreferencesTabOnFormMain.radioButtonCoordinate.Checked = true;
                    }
                }

                {
                    userControlPreferencesTabOnFormMain.checkBoxCoordinateRandom.Checked = preferencesDatTemp.RandomCoordinate;
                    userControlPreferencesTabOnFormMain.checkBoxColorRandom.Checked = preferencesDatTemp.RandomColor;
                    userControlPreferencesTabOnFormMain.checkBoxShapeRandom.Checked = preferencesDatTemp.RandomShape;

                    userControlPreferencesTabOnFormMain.checkBoxIncorrectCoordinateRandom.Checked = preferencesDatTemp.IncorrectCoordinateRandom;
                    userControlPreferencesTabOnFormMain.checkBoxIncorrectColorRandom.Checked = preferencesDatTemp.IncorrectColorRandom;
                    userControlPreferencesTabOnFormMain.checkBoxIncorrectShapeRandom.Checked = preferencesDatTemp.IncorrectShapeRandom;
                }

                {
                    // ShapeColor設定
                    // ShapeColorコンボ・ボックス選択候補設定
                    userControlPreferencesTabOnFormMain.comboBoxCorrectFixColor.Items.Clear();
                    userControlPreferencesTabOnFormMain.comboBoxCorrectFixColor.Items.Add(ECpColor.Black.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxCorrectFixColor.Items.Add(ECpColor.White.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxCorrectFixColor.Items.Add(ECpColor.Red.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxCorrectFixColor.Items.Add(ECpColor.Green.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxCorrectFixColor.Items.Add(ECpColor.Blue.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxCorrectFixColor.Items.Add(ECpColor.Yellow.ToString());
#if RANDOM_ENABLE
						userControlPreferencesTabOnFormMain.comboBoxCorrectFixColor.Items.Add(ECpColor.Random.ToString());
#endif
                    // 設定が異常の時、Whiteとする
                    //if (opImage.ConvertStringToCpColor(preferencesDatTemp.ColorFix, out eCpColorCorrectShapeColor) != true)
                    //{
                    //    eCpColorCorrectShapeColor = ECpColor.White;
                    //    preferencesDatTemp.ColorFix = eCpColorCorrectShapeColor.ToString();
                    //}
                    userControlPreferencesTabOnFormMain.comboBoxCorrectFixColor.SelectedItem = preferencesDatTemp.ColorFix;
                }

                {
                    // ShapeColor設定
                    ECpShape ecpFixCorrectShape;

                    // TypeOfShapeコンボ・ボックス選択候補設定
                    userControlPreferencesTabOnFormMain.comboBoxCorrectFixShape.Items.Clear();
                    userControlPreferencesTabOnFormMain.comboBoxCorrectFixShape.Items.Add(ECpShape.Circle.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxCorrectFixShape.Items.Add(ECpShape.Rectangle.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxCorrectFixShape.Items.Add(ECpShape.Triangle.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxCorrectFixShape.Items.Add(ECpShape.Star.ToString());

                    //SVG
                    List<ECpShape> nonExistShapeList = OpeImage.CheckExistSvgShape();
                    foreach (var n in nonExistShapeList)
                    {
                        userControlPreferencesTabOnFormMain.comboBoxCorrectFixShape.Items.Add(n.ToString());
                    }

#if RANDOM_ENABLE
						userControlPreferencesTabOnFormMain.comboBoxCorrectFixShape.Items.Add(ECpShape.Random.ToString());
#endif
                    // 設定が異常の時、Whiteとする
                    if (opImage.ConvertStringToCpShape(preferencesDatTemp.ShapeFix, out ecpFixCorrectShape) != true)
                    {
                        ecpFixCorrectShape = ECpShape.Circle;
                        preferencesDatTemp.ShapeFix = ecpFixCorrectShape.ToString();
                    }
                    userControlPreferencesTabOnFormMain.comboBoxCorrectFixShape.SelectedItem = preferencesDatTemp.ShapeFix;
                }
                {
                    // ShapeColor設定
                    // ShapeColorコンボ・ボックス1選択候補設定
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix1.Items.Clear();
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix1.Items.Add(ECpColor.Black.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix1.Items.Add(ECpColor.White.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix1.Items.Add(ECpColor.Red.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix1.Items.Add(ECpColor.Green.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix1.Items.Add(ECpColor.Blue.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix1.Items.Add(ECpColor.Yellow.ToString());
#if RANDOM_ENABLE
						userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix1.Items.Add(ECpColor.Random.ToString());
#endif
                    // 設定が異常の時、Whiteとする
                    //if (opImage.ConvertStringToCpColor(preferencesDatTemp.IncorrectColorFix1, out eCpColorIncorrectFixColor) != true)
                    //{
                    //    eCpColorIncorrectFixColor = ECpColor.White;
                    //    preferencesDatTemp.IncorrectColorFix1 = eCpColorIncorrectFixColor.ToString();
                    //}
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix1.SelectedItem = preferencesDatTemp.IncorrectColorFix1;
                }
                {
                    // ShapeColor設定
                    // ShapeColorコンボ・ボックス2選択候補設定
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix2.Items.Clear();
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix2.Items.Add(ECpColor.Black.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix2.Items.Add(ECpColor.White.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix2.Items.Add(ECpColor.Red.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix2.Items.Add(ECpColor.Green.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix2.Items.Add(ECpColor.Blue.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix2.Items.Add(ECpColor.Yellow.ToString());
#if RANDOM_ENABLE
						userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix2.Items.Add(ECpColor.Random.ToString());
#endif
                    // 設定が異常の時、Whiteとする
                    //if (opImage.ConvertStringToCpColor(preferencesDatTemp.IncorrectColorFix2, out eCpColorIncorrectFixColor) != true)
                    //{
                    //    eCpColorIncorrectFixColor = ECpColor.White;
                    //    preferencesDatTemp.IncorrectColorFix2 = eCpColorIncorrectFixColor.ToString();
                    //}
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix2.SelectedItem = preferencesDatTemp.IncorrectColorFix2;
                }
                {
                    // ShapeColor設定
                    // ShapeColorコンボ・ボックス2選択候補設定
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix3.Items.Clear();
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix3.Items.Add(ECpColor.Black.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix3.Items.Add(ECpColor.White.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix3.Items.Add(ECpColor.Red.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix3.Items.Add(ECpColor.Green.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix3.Items.Add(ECpColor.Blue.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix3.Items.Add(ECpColor.Yellow.ToString());
#if RANDOM_ENABLE
						userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix3.Items.Add(ECpColor.Random.ToString());
#endif
                    // 設定が異常の時、Whiteとする
                    //if (opImage.ConvertStringToCpColor(preferencesDatTemp.IncorrectColorFix3, out eCpColorIncorrectFixColor) != true)
                    //{
                    //    eCpColorIncorrectFixColor = ECpColor.White;
                    //    preferencesDatTemp.IncorrectColorFix3 = eCpColorIncorrectFixColor.ToString();
                    //}
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix3.SelectedItem = preferencesDatTemp.IncorrectColorFix3;
                }
                {
                    // ShapeColor設定
                    // ShapeColorコンボ・ボックス2選択候補設定
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix4.Items.Clear();
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix4.Items.Add(ECpColor.Black.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix4.Items.Add(ECpColor.White.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix4.Items.Add(ECpColor.Red.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix4.Items.Add(ECpColor.Green.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix4.Items.Add(ECpColor.Blue.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix4.Items.Add(ECpColor.Yellow.ToString());
#if RANDOM_ENABLE
						userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix4.Items.Add(ECpColor.Random.ToString());
#endif
                    // 設定が異常の時、Whiteとする
                    //if (opImage.ConvertStringToCpColor(preferencesDatTemp.IncorrectColorFix4, out eCpColorIncorrectFixColor) != true)
                    //{
                    //    eCpColorIncorrectFixColor = ECpColor.White;
                    //    preferencesDatTemp.IncorrectColorFix4 = eCpColorIncorrectFixColor.ToString();
                    //}
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectColorFix4.SelectedItem = preferencesDatTemp.IncorrectColorFix4;
                }

                {
                    // ShapeColor設定
                    ECpShape ecpFixIncorrectShape;

                    // TypeOfShapeコンボ・ボックス選択候補設定
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix1.Items.Clear();
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix1.Items.Add(ECpShape.Circle.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix1.Items.Add(ECpShape.Rectangle.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix1.Items.Add(ECpShape.Triangle.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix1.Items.Add(ECpShape.Star.ToString());

                    //SVG
                    List<ECpShape> nonExistShapeList = OpeImage.CheckExistSvgShape();
                    foreach (var n in nonExistShapeList)
                    {
                        userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix1.Items.Add(n.ToString());
                    }
#if RANDOM_ENABLE
						userControlPreferencesTabOnFormMain.comboBoxCorrectFixShape.Items.Add(ECpShape.Random.ToString());
#endif
                    // 設定が異常の時、Whiteとする
                    if (opImage.ConvertStringToCpShape(preferencesDatTemp.IncorrectShapeFix1, out ecpFixIncorrectShape) != true)
                    {
                        ecpFixIncorrectShape = ECpShape.Circle;
                        preferencesDatTemp.IncorrectShapeFix1 = ecpFixIncorrectShape.ToString();
                    }
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix1.SelectedItem = preferencesDatTemp.IncorrectShapeFix1;
                }
                {
                    // ShapeColor設定
                    ECpShape ecpFixIncorrectShape;

                    // TypeOfShapeコンボ・ボックス選択候補設定
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix2.Items.Clear();
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix2.Items.Add(ECpShape.Circle.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix2.Items.Add(ECpShape.Rectangle.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix2.Items.Add(ECpShape.Triangle.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix2.Items.Add(ECpShape.Star.ToString());

                    //SVG
                    List<ECpShape> nonExistShapeList = OpeImage.CheckExistSvgShape();
                    foreach (var n in nonExistShapeList)
                    {
                        userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix2.Items.Add(n.ToString());
                    }
#if RANDOM_ENABLE
						userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix2.Items.Add(ECpShape.Random.ToString());
#endif
                    // 設定が異常の時、Whiteとする
                    if (opImage.ConvertStringToCpShape(preferencesDatTemp.IncorrectShapeFix2, out ecpFixIncorrectShape) != true)
                    {
                        ecpFixIncorrectShape = ECpShape.Circle;
                        preferencesDatTemp.IncorrectShapeFix2 = ecpFixIncorrectShape.ToString();
                    }
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix2.SelectedItem = preferencesDatTemp.IncorrectShapeFix2;
                }
                {
                    // ShapeColor設定
                    ECpShape ecpFixIncorrectShape;

                    // TypeOfShapeコンボ・ボックス選択候補設定
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix3.Items.Clear();
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix3.Items.Add(ECpShape.Circle.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix3.Items.Add(ECpShape.Rectangle.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix3.Items.Add(ECpShape.Triangle.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix3.Items.Add(ECpShape.Star.ToString());

                    //SVG
                    List<ECpShape> nonExistShapeList = OpeImage.CheckExistSvgShape();
                    foreach (var n in nonExistShapeList)
                    {
                        userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix3.Items.Add(n.ToString());
                    }
#if RANDOM_ENABLE
						userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix3.Items.Add(ECpShape.Random.ToString());
#endif
                    // 設定が異常の時、Whiteとする
                    if (opImage.ConvertStringToCpShape(preferencesDatTemp.IncorrectShapeFix3, out ecpFixIncorrectShape) != true)
                    {
                        ecpFixIncorrectShape = ECpShape.Circle;
                        preferencesDatTemp.IncorrectShapeFix3 = ecpFixIncorrectShape.ToString();
                    }
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix3.SelectedItem = preferencesDatTemp.IncorrectShapeFix3;
                }
                {
                    // ShapeColor設定
                    ECpShape ecpFixIncorrectShape;

                    // TypeOfShapeコンボ・ボックス選択候補設定
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix4.Items.Clear();
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix4.Items.Add(ECpShape.Circle.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix4.Items.Add(ECpShape.Rectangle.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix4.Items.Add(ECpShape.Triangle.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix4.Items.Add(ECpShape.Star.ToString());
                    //SVG
                    List<ECpShape> nonExistShapeList = OpeImage.CheckExistSvgShape();
                    foreach (var n in nonExistShapeList)
                    {
                        userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix4.Items.Add(n.ToString());
                    }
#if RANDOM_ENABLE
						userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix4.Items.Add(ECpShape.Random.ToString());
#endif
                    // 設定が異常の時、Whiteとする
                    if (opImage.ConvertStringToCpShape(preferencesDatTemp.IncorrectShapeFix4, out ecpFixIncorrectShape) != true)
                    {
                        ecpFixIncorrectShape = ECpShape.Circle;
                        preferencesDatTemp.IncorrectShapeFix4 = ecpFixIncorrectShape.ToString();
                    }
                    userControlPreferencesTabOnFormMain.comboBoxIncorrectShapeFix4.SelectedItem = preferencesDatTemp.IncorrectShapeFix4;

                    if (userControlPreferencesTabOnFormMain.numericUpDownCorrectFixX.Maximum > (decimal)preferencesDatTemp.CoordinateFixX)
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownCorrectFixX.Value = (decimal)preferencesDatTemp.CoordinateFixX;
                    }
                    else
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownCorrectFixX.Value = userControlPreferencesTabOnFormMain.numericUpDownCorrectFixX.Maximum;
                    }
                    userControlPreferencesTabOnFormMain.numericUpDownCorrectFixX.Text = preferencesDatTemp.CoordinateFixX.ToString();

                    if (userControlPreferencesTabOnFormMain.numericUpDownCorrectFixY.Maximum > (decimal)preferencesDatTemp.CoordinateFixY)
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownCorrectFixY.Value = (decimal)preferencesDatTemp.CoordinateFixY;
                    }
                    else
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownCorrectFixY.Value = userControlPreferencesTabOnFormMain.numericUpDownCorrectFixY.Maximum;
                    }
                    userControlPreferencesTabOnFormMain.numericUpDownCorrectFixY.Text = preferencesDatTemp.CoordinateFixY.ToString();

                    if (userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX1.Maximum > (decimal)preferencesDatTemp.IncorrectCoordinateFixX1)
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX1.Value = (decimal)preferencesDatTemp.IncorrectCoordinateFixX1;
                    }
                    else
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX1.Value = userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX1.Maximum;
                    }
                    userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX1.Text = preferencesDatTemp.IncorrectCoordinateFixX1.ToString();

                    if (userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX2.Maximum > (decimal)preferencesDatTemp.IncorrectCoordinateFixX2)
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX2.Value = (decimal)preferencesDatTemp.IncorrectCoordinateFixX2;
                    }
                    else
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX2.Value = userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX2.Maximum;
                    }
                    userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX2.Text = preferencesDatTemp.IncorrectCoordinateFixX2.ToString();

                    if (userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX3.Maximum > (decimal)preferencesDatTemp.IncorrectCoordinateFixX3)
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX3.Value = (decimal)preferencesDatTemp.IncorrectCoordinateFixX3;
                    }
                    else
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX3.Value = userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX3.Maximum;
                    }
                    userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX3.Text = preferencesDatTemp.IncorrectCoordinateFixX3.ToString();

                    if (userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX4.Maximum > (decimal)preferencesDatTemp.IncorrectCoordinateFixX4)
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX4.Value = (decimal)preferencesDatTemp.IncorrectCoordinateFixX4;
                    }
                    else
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX4.Value = userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX4.Maximum;
                    }
                    userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixX4.Text = preferencesDatTemp.IncorrectCoordinateFixX4.ToString();

                    if (userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY1.Maximum > (decimal)preferencesDatTemp.IncorrectCoordinateFixY1)
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY1.Value = (decimal)preferencesDatTemp.IncorrectCoordinateFixY1;
                    }
                    else
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY1.Value = userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY1.Maximum;
                    }
                    userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY1.Text = preferencesDatTemp.IncorrectCoordinateFixY1.ToString();

                    if (userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY2.Maximum > (decimal)preferencesDatTemp.IncorrectCoordinateFixY2)
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY2.Value = (decimal)preferencesDatTemp.IncorrectCoordinateFixY2;
                    }
                    else
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY2.Value = userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY2.Maximum;
                    }
                    userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY2.Text = preferencesDatTemp.IncorrectCoordinateFixY2.ToString();

                    if (userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY3.Maximum > (decimal)preferencesDatTemp.IncorrectCoordinateFixY3)
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY3.Value = (decimal)preferencesDatTemp.IncorrectCoordinateFixY3;
                    }
                    else
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY3.Value = userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY3.Maximum;
                    }
                    userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY3.Text = preferencesDatTemp.IncorrectCoordinateFixY3.ToString();

                    if (userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY4.Maximum > (decimal)preferencesDatTemp.IncorrectCoordinateFixY4)
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY4.Value = (decimal)preferencesDatTemp.IncorrectCoordinateFixY4;
                    }
                    else
                    {
                        userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY4.Value = userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY4.Maximum;
                    }
                    userControlPreferencesTabOnFormMain.numericUpDownIncorrectFixY4.Text = preferencesDatTemp.IncorrectCoordinateFixY4.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownIncorrectNum.Value = (decimal)preferencesDatTemp.IncorrectNum;
                    userControlPreferencesTabOnFormMain.numericUpDownIncorrectNum.Text = preferencesDatTemp.IncorrectNum.ToString();

                    userControlPreferencesTabOnFormMain.checkBoxEnableImageShape.Checked = preferencesDatTemp.EnableImageShape;
                    userControlPreferencesTabOnFormMain.textBoxImageFileFolder.Text = preferencesDatTemp.ImageFileFolder;
                    userControlPreferencesTabOnFormMain.textBoxCorrectImage.Text = preferencesDatTemp.CorrectImageFile;

                    userControlPreferencesTabOnFormMain.checkBoxRandomCorrectImage.Checked = preferencesDatTemp.RandomCorrectImage;

                    userControlPreferencesTabOnFormMain.checkBoxEnableIncorrectCancel.Checked = preferencesDatTemp.EnableIncorrectCancel;
                    userControlPreferencesTabOnFormMain.numericUpDownIncorrectCount.Value = (decimal)preferencesDatTemp.IncorrectCount;
                    userControlPreferencesTabOnFormMain.numericUpDownIncorrectCount.Text = preferencesDatTemp.IncorrectCount.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownIncorrectPenaltyTime.Value = (decimal)preferencesDatTemp.IncorrectPenaltyTime;
                    userControlPreferencesTabOnFormMain.numericUpDownIncorrectPenaltyTime.Text = preferencesDatTemp.IncorrectPenaltyTime.ToString();

                }
                #endregion

                #region Soundカテゴリ:-----------------------------------------------------------------------------
                {
                    // 初期値設定
                    userControlPreferencesTabOnFormMain.textBoxSoundOfEnd.Text = preferencesDatTemp.SoundFileOfEnd;
                    userControlPreferencesTabOnFormMain.textBoxSoundOfReward.Text = preferencesDatTemp.SoundFileOfReward;
                    userControlPreferencesTabOnFormMain.textBoxSoundOfCorrect.Text = preferencesDatTemp.SoundFileOfCorrect;
                    userControlPreferencesTabOnFormMain.textBoxSoundOfIncorrect.Text = preferencesDatTemp.SoundFileOfIncorrect;
                    userControlPreferencesTabOnFormMain.textBoxSoundOfIncorrectReward.Text = preferencesDatTemp.SoundFileOfIncorrectReward;

                    userControlPreferencesTabOnFormMain.numericUpDownTimeToOutputSoundOfEnd.Value = (decimal)preferencesDatTemp.TimeToOutputSoundOfEnd;
                    userControlPreferencesTabOnFormMain.numericUpDownTimeToOutputSoundOfEnd.Text = preferencesDatTemp.TimeToOutputSoundOfEnd.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownTimeToOutputSoundOfReward.Value = (decimal)preferencesDatTemp.TimeToOutputSoundOfReward;
                    userControlPreferencesTabOnFormMain.numericUpDownTimeToOutputSoundOfReward.Text = preferencesDatTemp.TimeToOutputSoundOfReward.ToString();

                    //userControlPreferencesTabOnFormMain.numericUpDownTimeToOutputSoundOfCorrect.Value = (decimal)preferencesDatTemp.TimeToOutputSoundOfCorrect;
                    //userControlPreferencesTabOnFormMain.numericUpDownTimeToOutputSoundOfCorrect.Text = preferencesDatTemp.TimeToOutputSoundOfCorrect.ToString();
                }
                #endregion

                #region Mechanical thingカテゴリ:------------------------------------------------------------------
                {
                    // 初期値設定
                    userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfLeverIn.Value = (decimal)preferencesDatTemp.TimeoutOfLeverIn;
                    userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfLeverIn.Text = preferencesDatTemp.TimeoutOfLeverIn.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfLeverOut.Value = (decimal)preferencesDatTemp.TimeoutOfLeverOut;
                    userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfLeverOut.Text = preferencesDatTemp.TimeoutOfLeverOut.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfDoorOpen.Value = (decimal)preferencesDatTemp.TimeoutOfDoorOpen;
                    userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfDoorOpen.Text = preferencesDatTemp.TimeoutOfDoorOpen.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfDoorClose.Value = (decimal)preferencesDatTemp.TimeoutOfDoorClose;
                    userControlPreferencesTabOnFormMain.numericUpDownTimeoutOfDoorClose.Text = preferencesDatTemp.TimeoutOfDoorClose.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownTimeoutLeaveCage.Value = (decimal)preferencesDatTemp.OpeTimeoutOfLeaveCage;
                    userControlPreferencesTabOnFormMain.numericUpDownTimeoutLeaveCage.Text = preferencesDatTemp.OpeTimeoutOfLeaveCage.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownCageEntryTime.Value = (decimal)preferencesDatTemp.CageEntryTime;
                    userControlPreferencesTabOnFormMain.numericUpDownCageEntryTime.Text = preferencesDatTemp.CageEntryTime.ToString();

                    userControlPreferencesTabOnFormMain.checkBoxDisableDoor.Checked = (bool)preferencesDatTemp.DisableDoor;
                    userControlPreferencesTabOnFormMain.checkBoxIgnoreDoorError.Checked = (bool)preferencesDatTemp.IgnoreDoorError;
                    userControlPreferencesTabOnFormMain.checkBoxDisableLever.Checked = (bool)preferencesDatTemp.DisableLever;
                    userControlPreferencesTabOnFormMain.checkBoxConveyorFeed.Checked = (bool)preferencesDatTemp.EnableConveyor;
                    userControlPreferencesTabOnFormMain.checkBoxExtraFeeder.Checked = (bool)preferencesDatTemp.EnableExtraFeeder;
                    userControlPreferencesTabOnFormMain.checkBoxMultiExtraFeeder.Checked = (bool)preferencesDatTemp.EnableMultiFeeder;

                    //EDoor

                    userControlPreferencesTabOnFormMain.numericUpDownEDoorOpenSpeed.Value = preferencesDatTemp.EDoorOpenSpeed;
                    userControlPreferencesTabOnFormMain.numericUpDownEDoorOpenSpeed.Text = preferencesDatTemp.EDoorOpenSpeed.ToString();
                    userControlPreferencesTabOnFormMain.numericUpDownEDoorCloseSpeed.Value = preferencesDatTemp.EDoorCloseSpeed;
                    userControlPreferencesTabOnFormMain.numericUpDownEDoorCloseSpeed.Text = preferencesDatTemp.EDoorCloseSpeed.ToString();
                    userControlPreferencesTabOnFormMain.numericUpDownEDoorMiddleSpeed.Value = preferencesDatTemp.EDoorMiddleSpeed;
                    userControlPreferencesTabOnFormMain.numericUpDownEDoorMiddleSpeed.Text = preferencesDatTemp.EDoorMiddleSpeed.ToString();
                    userControlPreferencesTabOnFormMain.numericUpDownEDoorMiddleTime.Value = preferencesDatTemp.EDoorOpenToMiddleTime;
                    userControlPreferencesTabOnFormMain.numericUpDownEDoorMiddleTime.Text = preferencesDatTemp.EDoorOpenToMiddleTime.ToString();
                    userControlPreferencesTabOnFormMain.numericUpDownEDoorCloseToMiddleTime.Text = preferencesDatTemp.EDoorCloseToMiddleTime.ToString();
                    userControlPreferencesTabOnFormMain.numericUpDownEDoorOpenIntervalTime.Text = preferencesDatTemp.EDoorOpenIntervalTime.ToString();
                    userControlPreferencesTabOnFormMain.numericUpDownEDoorCloseIntervalTime.Text = preferencesDatTemp.EDoorCloseIntervalTime.ToString();
                    userControlPreferencesTabOnFormMain.numericUpDownEDoorMiddleIntervalTime.Text = preferencesDatTemp.EDoorMiddleIntervalTime.ToString();
                    userControlPreferencesTabOnFormMain.numericUpDownEDoorCloseTimeout.Text = preferencesDatTemp.EDoorCloseTimeout.ToString();
                    userControlPreferencesTabOnFormMain.numericUpDownEDoorMiddleTimeout.Text = preferencesDatTemp.EDoorMiddleTimeout.ToString();
                    userControlPreferencesTabOnFormMain.numericUpDownEDoorMonitorStartTime.Text = preferencesDatTemp.EDoorMonitorStartTime.ToString();
                    userControlPreferencesTabOnFormMain.numericUpDownEDoorCloseDoorMonitorStartTime.Text = preferencesDatTemp.EDoorCloseDoorMonitorStartTime.ToString();
                    userControlPreferencesTabOnFormMain.numericUpDownEDoorInsideDetectCloseTime.Text = preferencesDatTemp.EDoorInsideDetectCloseTime.ToString();
                    userControlPreferencesTabOnFormMain.numericUpDownEDoorPresenceOpenTime.Text = preferencesDatTemp.EDoorPresenceOpenTime.ToString();
                    userControlPreferencesTabOnFormMain.numericUpDownEDoorReEntryTime.Text = preferencesDatTemp.EDoorReEntryTime.ToString();
                    userControlPreferencesTabOnFormMain.numericUpDownEDoorPresenceMiddleDetectTime.Text = preferencesDatTemp.EDoorPresenceMiddleDetectTime.ToString();

                }
                #endregion

                #region Outputカテゴリ:----------------------------------------------------------------------------
                {
                    // 初期値設定
                    userControlPreferencesTabOnFormMain.textBoxOutputResultFile.Text = preferencesDatTemp.OutputResultFile;
                }
                #endregion

                #region EpisodMemoryカテゴリ:----------------------------------------------------------------------


                //EpisodeCount調整
                {
                    if (!CheckRenderSize(opImage.OpeImageSizeOfShapeInPixel, preferencesDatOriginal.EpisodeTargetNum))
                    {
                        while (!CheckRenderSize(opImage.OpeImageSizeOfShapeInPixel, preferencesDatOriginal.EpisodeTargetNum--))
                        {

                        }
                    }
                }
                //random設定
                {
                    userControlPreferencesTabOnFormMain.checkBoxEpisodeCoordinateRandom.Checked = preferencesDatTemp.EpisodeRandomCoordinate;
                    userControlPreferencesTabOnFormMain.checkBoxEpisodeColorRandom.Checked = preferencesDatTemp.EpisodeRandomColor;
                    userControlPreferencesTabOnFormMain.checkBoxEpisodeShapeRandom.Checked = preferencesDatTemp.EpisodeRandomShape;
                }
                //Color候補追加
                {
                    // ShapeColorコンボ・ボックス選択候補設定
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor1.Items.Clear();
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor1.Items.Add(ECpColor.Black.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor1.Items.Add(ECpColor.White.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor1.Items.Add(ECpColor.Red.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor1.Items.Add(ECpColor.Green.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor1.Items.Add(ECpColor.Blue.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor1.Items.Add(ECpColor.Yellow.ToString());

                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor1.SelectedItem = preferencesDatTemp.EpColorFix1;
                }
                //Shape候補追加
                {
                    // TypeOfShapeコンボ・ボックス選択候補設定
                    userControlPreferencesTabOnFormMain.comboBoxEpFixShape1.Items.Clear();
                    userControlPreferencesTabOnFormMain.comboBoxEpFixShape1.Items.Add(ECpShape.Circle.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixShape1.Items.Add(ECpShape.Rectangle.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixShape1.Items.Add(ECpShape.Triangle.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixShape1.Items.Add(ECpShape.Star.ToString());

                    //SVG
                    List<ECpShape> nonExistShapeList = OpeImage.CheckExistSvgShape();
                    foreach (var n in nonExistShapeList)
                    {
                        userControlPreferencesTabOnFormMain.comboBoxEpFixShape1.Items.Add(n.ToString());
                    }

                    // 設定が異常の時、Whiteとする
                    if (opImage.ConvertStringToCpShape(preferencesDatTemp.EpShapeFix1, out ECpShape ecpFixCorrectShape) != true)
                    {
                        ecpFixCorrectShape = ECpShape.Circle;
                        preferencesDatTemp.EpShapeFix1 = ecpFixCorrectShape.ToString();
                    }
                    userControlPreferencesTabOnFormMain.comboBoxEpFixShape1.SelectedItem = preferencesDatTemp.EpShapeFix1;
                }

                {
                    // ShapeColorコンボ・ボックス選択候補設定
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor2.Items.Clear();
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor2.Items.Add(ECpColor.Black.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor2.Items.Add(ECpColor.White.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor2.Items.Add(ECpColor.Red.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor2.Items.Add(ECpColor.Green.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor2.Items.Add(ECpColor.Blue.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor2.Items.Add(ECpColor.Yellow.ToString());

                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor2.SelectedItem = preferencesDatTemp.EpColorFix2;
                }

                {
                    // TypeOfShapeコンボ・ボックス選択候補設定
                    userControlPreferencesTabOnFormMain.comboBoxEpFixShape2.Items.Clear();
                    userControlPreferencesTabOnFormMain.comboBoxEpFixShape2.Items.Add(ECpShape.Circle.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixShape2.Items.Add(ECpShape.Rectangle.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixShape2.Items.Add(ECpShape.Triangle.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixShape2.Items.Add(ECpShape.Star.ToString());

                    //SVG
                    List<ECpShape> nonExistShapeList = OpeImage.CheckExistSvgShape();
                    foreach (var n in nonExistShapeList)
                    {
                        userControlPreferencesTabOnFormMain.comboBoxEpFixShape2.Items.Add(n.ToString());
                    }

                    // 設定が異常の時、Whiteとする
                    if (opImage.ConvertStringToCpShape(preferencesDatTemp.EpShapeFix2, out ECpShape ecpFixCorrectShape) != true)
                    {
                        ecpFixCorrectShape = ECpShape.Circle;
                        preferencesDatTemp.EpShapeFix2 = ecpFixCorrectShape.ToString();
                    }
                    userControlPreferencesTabOnFormMain.comboBoxEpFixShape2.SelectedItem = preferencesDatTemp.EpShapeFix2;
                }

                {
                    // ShapeColorコンボ・ボックス選択候補設定
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor3.Items.Clear();
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor3.Items.Add(ECpColor.Black.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor3.Items.Add(ECpColor.White.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor3.Items.Add(ECpColor.Red.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor3.Items.Add(ECpColor.Green.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor3.Items.Add(ECpColor.Blue.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor3.Items.Add(ECpColor.Yellow.ToString());

                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor3.SelectedItem = preferencesDatTemp.EpColorFix3;
                }

                {
                    // TypeOfShapeコンボ・ボックス選択候補設定
                    userControlPreferencesTabOnFormMain.comboBoxEpFixShape3.Items.Clear();
                    userControlPreferencesTabOnFormMain.comboBoxEpFixShape3.Items.Add(ECpShape.Circle.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixShape3.Items.Add(ECpShape.Rectangle.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixShape3.Items.Add(ECpShape.Triangle.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixShape3.Items.Add(ECpShape.Star.ToString());

                    //SVG
                    List<ECpShape> nonExistShapeList = OpeImage.CheckExistSvgShape();
                    foreach (var n in nonExistShapeList)
                    {
                        userControlPreferencesTabOnFormMain.comboBoxEpFixShape3.Items.Add(n.ToString());
                    }

                    // 設定が異常の時、Whiteとする
                    if (opImage.ConvertStringToCpShape(preferencesDatTemp.EpShapeFix3, out ECpShape ecpFixCorrectShape) != true)
                    {
                        ecpFixCorrectShape = ECpShape.Circle;
                        preferencesDatTemp.EpShapeFix3 = ecpFixCorrectShape.ToString();
                    }
                    userControlPreferencesTabOnFormMain.comboBoxEpFixShape3.SelectedItem = preferencesDatTemp.EpShapeFix3;
                }

                {
                    // ShapeColorコンボ・ボックス選択候補設定
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor4.Items.Clear();
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor4.Items.Add(ECpColor.Black.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor4.Items.Add(ECpColor.White.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor4.Items.Add(ECpColor.Red.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor4.Items.Add(ECpColor.Green.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor4.Items.Add(ECpColor.Blue.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor4.Items.Add(ECpColor.Yellow.ToString());

                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor4.SelectedItem = preferencesDatTemp.EpColorFix4;
                }

                {
                    // TypeOfShapeコンボ・ボックス選択候補設定
                    userControlPreferencesTabOnFormMain.comboBoxEpFixShape4.Items.Clear();
                    userControlPreferencesTabOnFormMain.comboBoxEpFixShape4.Items.Add(ECpShape.Circle.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixShape4.Items.Add(ECpShape.Rectangle.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixShape4.Items.Add(ECpShape.Triangle.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixShape4.Items.Add(ECpShape.Star.ToString());

                    //SVG
                    List<ECpShape> nonExistShapeList = OpeImage.CheckExistSvgShape();
                    foreach (var n in nonExistShapeList)
                    {
                        userControlPreferencesTabOnFormMain.comboBoxEpFixShape4.Items.Add(n.ToString());
                    }

                    // 設定が異常の時、Whiteとする
                    if (opImage.ConvertStringToCpShape(preferencesDatTemp.EpShapeFix4, out ECpShape ecpFixCorrectShape) != true)
                    {
                        ecpFixCorrectShape = ECpShape.Circle;
                        preferencesDatTemp.EpShapeFix4 = ecpFixCorrectShape.ToString();
                    }
                    userControlPreferencesTabOnFormMain.comboBoxEpFixShape4.SelectedItem = preferencesDatTemp.EpShapeFix4;
                }

                {
                    // ShapeColorコンボ・ボックス選択候補設定
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor5.Items.Clear();
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor5.Items.Add(ECpColor.Black.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor5.Items.Add(ECpColor.White.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor5.Items.Add(ECpColor.Red.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor5.Items.Add(ECpColor.Green.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor5.Items.Add(ECpColor.Blue.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor5.Items.Add(ECpColor.Yellow.ToString());

                    userControlPreferencesTabOnFormMain.comboBoxEpFixColor5.SelectedItem = preferencesDatTemp.EpColorFix5;
                }

                {
                    // TypeOfShapeコンボ・ボックス選択候補設定
                    userControlPreferencesTabOnFormMain.comboBoxEpFixShape5.Items.Clear();
                    userControlPreferencesTabOnFormMain.comboBoxEpFixShape5.Items.Add(ECpShape.Circle.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixShape5.Items.Add(ECpShape.Rectangle.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixShape5.Items.Add(ECpShape.Triangle.ToString());
                    userControlPreferencesTabOnFormMain.comboBoxEpFixShape5.Items.Add(ECpShape.Star.ToString());

                    //SVG
                    List<ECpShape> nonExistShapeList = OpeImage.CheckExistSvgShape();
                    foreach (var n in nonExistShapeList)
                    {
                        userControlPreferencesTabOnFormMain.comboBoxEpFixShape5.Items.Add(n.ToString());
                    }

                    // 設定が異常の時、Whiteとする
                    if (opImage.ConvertStringToCpShape(preferencesDatTemp.EpShapeFix5, out ECpShape ecpFixCorrectShape) != true)
                    {
                        ecpFixCorrectShape = ECpShape.Circle;
                        preferencesDatTemp.EpShapeFix5 = ecpFixCorrectShape.ToString();
                    }
                    userControlPreferencesTabOnFormMain.comboBoxEpFixShape5.SelectedItem = preferencesDatTemp.EpShapeFix5;
                }

                {
                    userControlPreferencesTabOnFormMain.numericUpDownEpTarget1FixX.Value = preferencesDatTemp.EpTarget1FixX;
                    userControlPreferencesTabOnFormMain.numericUpDownEpTarget1FixX.Text = preferencesDatTemp.EpTarget1FixX.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownEpTarget2FixX.Value = preferencesDatTemp.EpTarget2FixX;
                    userControlPreferencesTabOnFormMain.numericUpDownEpTarget2FixX.Text = preferencesDatTemp.EpTarget2FixX.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownEpTarget3FixX.Value = preferencesDatTemp.EpTarget3FixX;
                    userControlPreferencesTabOnFormMain.numericUpDownEpTarget3FixX.Text = preferencesDatTemp.EpTarget3FixX.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownEpTarget4FixX.Value = preferencesDatTemp.EpTarget4FixX;
                    userControlPreferencesTabOnFormMain.numericUpDownEpTarget4FixX.Text = preferencesDatTemp.EpTarget4FixX.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownEpTarget5FixX.Value = preferencesDatTemp.EpTarget5FixX;
                    userControlPreferencesTabOnFormMain.numericUpDownEpTarget5FixX.Text = preferencesDatTemp.EpTarget5FixX.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownEpTarget1FixY.Value = preferencesDatTemp.EpTarget1FixY;
                    userControlPreferencesTabOnFormMain.numericUpDownEpTarget1FixY.Text = preferencesDatTemp.EpTarget1FixY.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownEpTarget2FixY.Value = preferencesDatTemp.EpTarget2FixY;
                    userControlPreferencesTabOnFormMain.numericUpDownEpTarget2FixY.Text = preferencesDatTemp.EpTarget2FixY.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownEpTarget3FixY.Value = preferencesDatTemp.EpTarget3FixY;
                    userControlPreferencesTabOnFormMain.numericUpDownEpTarget3FixY.Text = preferencesDatTemp.EpTarget3FixY.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownEpTarget4FixY.Value = preferencesDatTemp.EpTarget4FixY;
                    userControlPreferencesTabOnFormMain.numericUpDownEpTarget4FixY.Text = preferencesDatTemp.EpTarget4FixY.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownEpTarget5FixY.Value = preferencesDatTemp.EpTarget5FixY;
                    userControlPreferencesTabOnFormMain.numericUpDownEpTarget5FixY.Text = preferencesDatTemp.EpTarget5FixY.ToString();

                    userControlPreferencesTabOnFormMain.numericUpDownEpisodeTargetNum.Value = preferencesDatTemp.EpisodeTargetNum;
                    userControlPreferencesTabOnFormMain.numericUpDownEpisodeTargetNum.Text = preferencesDatTemp.EpisodeTargetNum.ToString();

                    userControlPreferencesTabOnFormMain.checkBoxEnableEpImageShape.Checked = preferencesDatTemp.EnableEpImageShape;
                    userControlPreferencesTabOnFormMain.textBoxEpImageFileFolder.Text = preferencesDatTemp.EpImageFileFolder;

                }
                #endregion

                #region MarmosetDetectionカテゴリ

                userControlPreferencesTabOnFormMain.checkBoxMarmosetDetection.Checked = preferencesDatTemp.EnableMamosetDetection;
                userControlPreferencesTabOnFormMain.checkBoxDetectionSaveImageMarmosetDetection.Checked = preferencesDatTemp.EnableMamosetDetectionSaveImageMode;
                userControlPreferencesTabOnFormMain.checkBoxSaveOnlyImage.Checked = preferencesDatTemp.EnableMamosetDetectionSaveOnlyMode;
                userControlPreferencesTabOnFormMain.folderBrowserDialogLearningSaveImageFolder.SelectedPath = preferencesDatTemp.LearningSaveImageFolder;
                userControlPreferencesTabOnFormMain.textBoxLearningSaveImageFolder.Text = preferencesDatTemp.LearningSaveImageFolder;
                userControlPreferencesTabOnFormMain.textBoxModelFile.Text = preferencesDatTemp.LearningModelPath;

                userControlPreferencesTabOnFormMain.numericUpDownShotCamInterval.Value = preferencesDatTemp.ShotCamInterval;
                userControlPreferencesTabOnFormMain.numericUpDownMarmoDetectionThreshold.Value = (decimal)preferencesDatTemp.DetectThreshold;

                #endregion

            }
        }

        /// <summary>
        /// userControlPreferencesTabOnFormMain: 表示
        /// </summary>
        private void VisibleUcPreferencesTab()
        {
            // シリアル・ポート・オープンしている時
            if (serialPortOpenFlag == true)
            {
                // シリアル・ポート: クローズ
                serialHelperPort.Close();
                serialPortOpenFlag = false;
            }

            // userControlPreferencesTabOnFormMain: 表示
            userControlMainOnFormMain.Visible = false;
            userControlOperationOnFormMain.Visible = false;
            userControlCheckDeviceOnFormMain.Visible = false;
            userControlCheckIoOnFormMain.Visible = false;
            userControlPreferencesTabOnFormMain.Visible = true;
            userControlPreferencesTabOnFormMain.Dock = DockStyle.Fill;
            userControlInputComOnFormMain.Visible = false;
            // Form.Text設定
            this.Text = "Preferences";

            // DiscoveryClientでCam発見されてたら
            // 表示するたびにリスト更新されてしまうので考える
            if (camImage.DevicesDiscovered)
            {
                userControlPreferencesTabOnFormMain.comboBoxCamUri.Items.Clear();
                foreach (var uri in camImage.DeviceUris)
                {
                    userControlPreferencesTabOnFormMain.comboBoxCamUri.Items.Add("CAM" + userControlPreferencesTabOnFormMain.comboBoxCamUri.Items.Count + " \"" + uri.Host + "\"");
                    //userControlPreferencesTabOnFormMain.comboBoxCamUri.Items.Add(uri);
                }
                if (userControlPreferencesTabOnFormMain.comboBoxCamUri.Items.Count > 0)
                {
                    if (preferencesDatTemp.SelectCamUri is null)
                        return;

                    UriBuilder settingUri = new UriBuilder(preferencesDatTemp.SelectCamUri);
                    int selectedItemCount = camImage.DeviceUris.IndexOf(settingUri);
                    //int selectedItemCount = userControlPreferencesTabOnFormMain.comboBoxCamUri.Items.IndexOf(preferencesDatTemp.SelectCamUri);
                    userControlPreferencesTabOnFormMain.comboBoxCamUri.SelectedIndex = selectedItemCount;

                }
            }
            else
            {
                MessageBox.Show("カメラが見つかりません");
            }

        }

        private bool CheckRenderSize(int pixel, int targetNum)
        {
            const double marginFactor = 0.8;
            if (opImage != null)
            {
                int validRectSize = (int)((opImage.RectOpeImageValidArea.Width - opImage.RectOpeImageValidArea.X) * (opImage.RectOpeImageValidArea.Height - opImage.RectOpeImageValidArea.Y) * marginFactor);

                if (validRectSize > (pixel * pixel * targetNum))
                {
                    return true;
                }
            }
            else
            {
                return false;
            }

            return true;
        }
    }

}

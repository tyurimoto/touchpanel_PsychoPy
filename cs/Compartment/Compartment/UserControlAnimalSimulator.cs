using System;
using System.Windows.Forms;

namespace Compartment
{
    public partial class UserControlAnimalSimulator : UserControl
    {
        private IoMicrochipDummyEx dummyIoBoard;
        private FormMain formMain;

        public enum AnimalPosition
        {
            Outside,        // 外（部屋の外）
            Entering,       // 入室中
            Inside,         // 在室中
            Exiting,        // 退室中
            AtLever         // レバー付近
        }

        public UserControlAnimalSimulator(FormMain parent)
        {
            InitializeComponent();
            formMain = parent;

            // IOボードの参照を取得
            if (parent.ioBoardDevice is IoMicrochipDummyEx dummyEx)
            {
                dummyIoBoard = dummyEx;
            }
            else
            {
                // デバッグモード以外では無効化
                this.Enabled = false;
                labelStatus.Text = "状態: エラー - デバッグモードではありません";
                return;
            }

            // イベントハンドラ登録
            buttonApply.Click += ButtonApply_Click;

            // ラジオボタン変更イベント
            radioButtonOutside.CheckedChanged += RadioButton_CheckedChanged;
            radioButtonEntering.CheckedChanged += RadioButton_CheckedChanged;
            radioButtonInside.CheckedChanged += RadioButton_CheckedChanged;
            radioButtonExiting.CheckedChanged += RadioButton_CheckedChanged;
            radioButtonAtLever.CheckedChanged += RadioButton_CheckedChanged;

            // チェックボックス変更イベント
            checkBoxPressingLever.CheckedChanged += CheckBoxPressingLever_CheckedChanged;

            // 初期状態を適用
            ApplyAnimalPosition();
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            // レバー押下はレバー付近でのみ有効
            checkBoxPressingLever.Enabled = radioButtonAtLever.Checked;
            if (!radioButtonAtLever.Checked)
            {
                checkBoxPressingLever.Checked = false;
            }
        }

        private void CheckBoxPressingLever_CheckedChanged(object sender, EventArgs e)
        {
            // レバー押下時は自動的にレバー付近を選択
            if (checkBoxPressingLever.Checked && !radioButtonAtLever.Checked)
            {
                radioButtonAtLever.Checked = true;
            }
        }

        private void ButtonApply_Click(object sender, EventArgs e)
        {
            ApplyAnimalPosition();
        }

        private void ApplyAnimalPosition()
        {
            if (dummyIoBoard == null) return;

            try
            {
                AnimalPosition position = GetCurrentPosition();
                bool pressingLever = checkBoxPressingLever.Checked;

                // すべてのセンサーを初期化
                dummyIoBoard.SetManualSensorState(IoBoardDInLogicalName.RoomEntrance, false);
                dummyIoBoard.SetManualSensorState(IoBoardDInLogicalName.RoomExit, false);
                dummyIoBoard.SetManualSensorState(IoBoardDInLogicalName.RoomStay, false);
                dummyIoBoard.SetManualSensorState(IoBoardDInLogicalName.LeverSw, false);

                // 位置に応じてセンサーを設定
                switch (position)
                {
                    case AnimalPosition.Outside:
                        // 外: すべてOFF（既に初期化済み）
                        labelStatus.Text = "状態: 外（部屋の外） - すべてのセンサーOFF";
                        break;

                    case AnimalPosition.Entering:
                        // 入室中: 入室センサーON
                        dummyIoBoard.SetManualSensorState(IoBoardDInLogicalName.RoomEntrance, true);
                        labelStatus.Text = "状態: 入室中 - 入室センサーON";
                        break;

                    case AnimalPosition.Inside:
                        // 在室中: 在室センサーON
                        dummyIoBoard.SetManualSensorState(IoBoardDInLogicalName.RoomStay, true);
                        labelStatus.Text = "状態: 在室中 - 在室センサーON";
                        break;

                    case AnimalPosition.Exiting:
                        // 退室中: 退室センサーON
                        dummyIoBoard.SetManualSensorState(IoBoardDInLogicalName.RoomExit, true);
                        labelStatus.Text = "状態: 退室中 - 退室センサーON";
                        break;

                    case AnimalPosition.AtLever:
                        // レバー付近: 在室センサーON + レバー押下状態
                        dummyIoBoard.SetManualSensorState(IoBoardDInLogicalName.RoomStay, true);
                        if (pressingLever)
                        {
                            dummyIoBoard.SetManualSensorState(IoBoardDInLogicalName.LeverSw, true);
                            labelStatus.Text = "状態: レバー付近 - 在室センサーON, レバーSW ON";
                        }
                        else
                        {
                            labelStatus.Text = "状態: レバー付近 - 在室センサーON, レバーSW OFF";
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                labelStatus.Text = $"エラー: {ex.Message}";
            }
        }

        private AnimalPosition GetCurrentPosition()
        {
            if (radioButtonOutside.Checked) return AnimalPosition.Outside;
            if (radioButtonEntering.Checked) return AnimalPosition.Entering;
            if (radioButtonInside.Checked) return AnimalPosition.Inside;
            if (radioButtonExiting.Checked) return AnimalPosition.Exiting;
            if (radioButtonAtLever.Checked) return AnimalPosition.AtLever;
            return AnimalPosition.Outside;
        }
    }
}

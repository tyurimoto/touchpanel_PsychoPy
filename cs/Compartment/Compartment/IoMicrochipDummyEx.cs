using System;
using System.Collections.Generic;

namespace Compartment
{
    /// <summary>
    /// デバッグモード用の拡張ダミーIOボード
    /// センサー状態を保持し、手動で設定可能
    /// </summary>
    public class IoMicrochipDummyEx : IoBoardBase
    {
        // センサー状態を保持するDictionary
        private Dictionary<IoBoardDInLogicalName, bool> sensorStates = new Dictionary<IoBoardDInLogicalName, bool>();
        private readonly object sensorStateLock = new object();

        public IoMicrochipDummyEx()
        {
            errorMsg = "";

            // すべてのセンサーを初期化（全てfalse）
            foreach (IoBoardDInLogicalName sensor in Enum.GetValues(typeof(IoBoardDInLogicalName)))
            {
                sensorStates[sensor] = false;
            }

            // 初期状態: ドアは閉じている、レバーは引っ込んでいる
            sensorStates[IoBoardDInLogicalName.DoorClose] = true;
            sensorStates[IoBoardDInLogicalName.LeverIn] = true;
        }

        public override bool AcquireDevice()
        {
            return true;
        }

        public override bool ReleaseDevice()
        {
            return true;
        }

        public override bool DirectOut(IoBoardPortNo a_IoBoardPortNoObj, ushort a_ushortOutCode)
        {
            return true;
        }

        public override bool DirectIn(IoBoardPortNo a_IoBoardPortNoObj, out ushort a_ushortInCode)
        {
            a_ushortInCode = 0;
            return true;
        }

        public override bool SaveDIn()
        {
            return true;
        }

        /// <summary>
        /// センサー状態を手動で設定する（デバッグ用）
        /// </summary>
        /// <param name="sensor">センサー名</param>
        /// <param name="state">状態（true=ON, false=OFF）</param>
        public void SetManualSensorState(IoBoardDInLogicalName sensor, bool state)
        {
            lock (sensorStateLock)
            {
                sensorStates[sensor] = state;
            }
        }

        /// <summary>
        /// センサー状態を取得
        /// </summary>
        public override bool GetRawStateOfSaveDIn(IoBoardDInLogicalName a_IoBoardDInLogicalNameObj, out bool a_boolRawState)
        {
            lock (sensorStateLock)
            {
                if (sensorStates.ContainsKey(a_IoBoardDInLogicalNameObj))
                {
                    a_boolRawState = sensorStates[a_IoBoardDInLogicalNameObj];
                }
                else
                {
                    a_boolRawState = false;
                }
            }
            return true;
        }

        public override bool GetUpperStateOfSaveDIn(IoBoardDInLogicalName a_IoBoardDInLogicalNameObj, out bool a_boolLogicalState)
        {
            return GetRawStateOfSaveDIn(a_IoBoardDInLogicalNameObj, out a_boolLogicalState);
        }

        /// <summary>
        /// 出力制御（ドア開閉、レバー出し入れなど）
        /// デバイスコマンドに応じてセンサー状態を自動更新
        /// </summary>
        public override bool SetUpperStateOfDOut(IoBoardDOutLogicalName a_IoBoardDOutLogicalNameObj)
        {
            switch (a_IoBoardDOutLogicalNameObj)
            {
                case IoBoardDOutLogicalName.DoorOpen:
                    // ドアを開く動作をシミュレート
                    SimulateDoorOpen();
                    break;

                case IoBoardDOutLogicalName.DoorClose:
                    // ドアを閉じる動作をシミュレート
                    SimulateDoorClose();
                    break;

                case IoBoardDOutLogicalName.DoorStop:
                    // ドア停止 → 状態変更なし
                    break;

                case IoBoardDOutLogicalName.LeverOut:
                    // レバーを出す動作をシミュレート
                    SimulateLeverOut();
                    break;

                case IoBoardDOutLogicalName.LeverIn:
                    // レバーを引っ込める動作をシミュレート
                    SimulateLeverIn();
                    break;

                case IoBoardDOutLogicalName.LeverStop:
                    // レバー停止 → 状態変更なし
                    break;

                case IoBoardDOutLogicalName.RoomLampOn:
                case IoBoardDOutLogicalName.RoomLampOff:
                case IoBoardDOutLogicalName.LeverLampOn:
                case IoBoardDOutLogicalName.LeverLampOff:
                    // ランプ制御 → センサー状態に影響なし
                    break;

                default:
                    break;
            }
            return true;
        }

        /// <summary>
        /// ドアを開く動作をシミュレート
        /// </summary>
        private void SimulateDoorOpen()
        {
            // 即座にドアが開いた状態にする（リミットスイッチをfalseにしてeDoorを進める）
            lock (sensorStateLock)
            {
                sensorStates[IoBoardDInLogicalName.DoorOpen] = true;
                sensorStates[IoBoardDInLogicalName.DoorClose] = false;
            }
        }

        /// <summary>
        /// ドアを閉じる動作をシミュレート
        /// </summary>
        private void SimulateDoorClose()
        {
            // 即座にドアが閉じた状態にする（リミットスイッチをfalseにしてeDoorを進める）
            lock (sensorStateLock)
            {
                sensorStates[IoBoardDInLogicalName.DoorOpen] = false;
                sensorStates[IoBoardDInLogicalName.DoorClose] = true;
            }
        }

        /// <summary>
        /// レバーを出す動作をシミュレート
        /// </summary>
        private void SimulateLeverOut()
        {
            // 即座にレバーが出た状態にする
            lock (sensorStateLock)
            {
                sensorStates[IoBoardDInLogicalName.LeverIn] = false;
                sensorStates[IoBoardDInLogicalName.LeverOut] = true;
            }
        }

        /// <summary>
        /// レバーを引っ込める動作をシミュレート
        /// </summary>
        private void SimulateLeverIn()
        {
            // 即座にレバーが引っ込んだ状態にする
            lock (sensorStateLock)
            {
                sensorStates[IoBoardDInLogicalName.LeverOut] = false;
                sensorStates[IoBoardDInLogicalName.LeverIn] = true;
            }
        }

        public override bool GetData(IoMicrochip.IoBoardDInCode ioBoardDInCode)
        {
            return true;
        }

        public override bool GetData(IoMicrochip.IoBoardDInStatusCode ioBoardDInCode, bool n)
        {
            return true;
        }

        /// <summary>
        /// センサーデータを取得（eDoor用のリミットスイッチなど）
        /// </summary>
        public override bool GetRecieveData(int portNum)
        {
            lock (sensorStateLock)
            {
                // リミットスイッチの状態を返す
                // DoorMotorCWLimit_B: ドアが完全に開いているときfalse、それ以外true
                // DoorMotorCCWLimit_B: ドアが完全に閉じているときfalse、それ以外true
                switch ((IoMicrochip.IoBoardInPortNum)portNum)
                {
                    case IoMicrochip.IoBoardInPortNum.DoorMotorCWLimit_B:
                        // ドアが完全に開いている場合はfalse
                        return !sensorStates[IoBoardDInLogicalName.DoorOpen];

                    case IoMicrochip.IoBoardInPortNum.DoorMotorCCWLimit_B:
                        // ドアが完全に閉じている場合はfalse
                        return !sensorStates[IoBoardDInLogicalName.DoorClose];

                    case IoMicrochip.IoBoardInPortNum.DetectOutDirection:
                    case IoMicrochip.IoBoardInPortNum.DetectInDirection:
                    case IoMicrochip.IoBoardInPortNum.DoorOutsideSensor:
                    case IoMicrochip.IoBoardInPortNum.DoorInsideSensor:
                        // その他のセンサーは常にfalse
                        return false;

                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// すべてのセンサー状態をリセット
        /// </summary>
        public void ResetAllSensors()
        {
            lock (sensorStateLock)
            {
                foreach (IoBoardDInLogicalName sensor in Enum.GetValues(typeof(IoBoardDInLogicalName)))
                {
                    sensorStates[sensor] = false;
                }

                // 初期状態に戻す
                sensorStates[IoBoardDInLogicalName.DoorClose] = true;
                sensorStates[IoBoardDInLogicalName.LeverIn] = true;
            }
        }

        /// <summary>
        /// 現在のセンサー状態を取得（デバッグ表示用）
        /// </summary>
        public Dictionary<IoBoardDInLogicalName, bool> GetAllSensorStates()
        {
            lock (sensorStateLock)
            {
                return new Dictionary<IoBoardDInLogicalName, bool>(sensorStates);
            }
        }

        /// <summary>
        /// 出力ビットを設定（eDoor用のモーター制御など）
        /// デバッグモードでは実際には何もしない
        /// </summary>
        public override void SetOutBit(byte bitMask, bool data, int frameCount)
        {
            // デバッグモードでは出力制御は無視
        }

        /// <summary>
        /// モーター速度を設定
        /// デバッグモードでは実際には何もしない
        /// </summary>
        public override void SetMotorSpeed(int speed)
        {
            // デバッグモードではモーター速度設定は無視
        }
    }
}

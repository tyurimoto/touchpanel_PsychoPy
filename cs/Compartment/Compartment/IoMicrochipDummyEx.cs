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
            lock (sensorStateLock)
            {
                switch (a_IoBoardDOutLogicalNameObj)
                {
                    case IoBoardDOutLogicalName.DoorOpen:
                        // ドアを開く → DoorOpenセンサーON、DoorCloseセンサーOFF
                        sensorStates[IoBoardDInLogicalName.DoorOpen] = true;
                        sensorStates[IoBoardDInLogicalName.DoorClose] = false;
                        break;

                    case IoBoardDOutLogicalName.DoorClose:
                        // ドアを閉じる → DoorOpenセンサーOFF、DoorCloseセンサーON
                        sensorStates[IoBoardDInLogicalName.DoorOpen] = false;
                        sensorStates[IoBoardDInLogicalName.DoorClose] = true;
                        break;

                    case IoBoardDOutLogicalName.DoorStop:
                        // ドア停止 → 状態変更なし
                        break;

                    // レバー制御は将来拡張
                    default:
                        break;
                }
            }
            return true;
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
    }
}

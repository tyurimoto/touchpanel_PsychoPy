using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Compartment
{
    public class IoMicrochip : IoBoardBase, IDisposable
    {
        readonly SyncObject<byte[]> _UsbSendData = new SyncObject<byte[]>(new byte[64]);
        byte[] UsbSendData { get => _UsbSendData.Value; set => _UsbSendData.Value = value; }  //送信パケット

        readonly SyncObject<byte[]> _UsbRecieveData = new SyncObject<byte[]>(new byte[64]);
        byte[] UsbRecieveData { get => _UsbRecieveData.Value; set => _UsbRecieveData.Value = value; }

        readonly SyncObject<byte[]> _UsbRecieveDataRecent = new SyncObject<byte[]>(new byte[64]);
        byte[] UsbRecieveDataRecent { get => _UsbRecieveDataRecent.Value; set => _UsbRecieveDataRecent.Value = value; }

        readonly SyncObject<byte[]> _UsbRecieveRiseEdge = new SyncObject<byte[]>(new byte[64]);
        byte[] UsbRecieveRiseEdge { get => _UsbRecieveRiseEdge.Value; set => _UsbRecieveRiseEdge.Value = value; }

        readonly SyncObject<byte[]> _UsbRecieveFallEdge = new SyncObject<byte[]>(new byte[64]);
        byte[] UsbRecieveFallEdge { get => _UsbRecieveFallEdge.Value; set => _UsbRecieveFallEdge.Value = value; }

        //byte[] UsbRecieveData = new byte[64];  //受信パケット
        readonly SyncObject<bool> _USB_Connect = new SyncObject<bool>(false);
        bool USB_Connect { get => _USB_Connect.Value; set => _USB_Connect.Value = value; }       //接続フラグ

        private readonly SyncObject<byte> OutData = new SyncObject<byte>(0);
        private readonly SyncObject<byte> OutData2 = new SyncObject<byte>(0);
        private readonly SyncObject<byte> OutData3 = new SyncObject<byte>(0);
        //byte OutputFlag = 0;
        byte DataBit;

        readonly SyncObject<bool> kanshi = new SyncObject<bool>(true);
        Task task;
        CancellationTokenSource cancellationTokenSource;
        //public string errorMsg { get; set; }

        Stopwatch mainCountSw = new Stopwatch();

        public event EventHandler ConveyorErrorHandler;
        public event EventHandler RecieveDataHandler;

        //// 互換性用
        //public enum IoBoardPortNo : int
        //{
        //    /// <summary>
        //    /// ポート番号
        //    /// </summary>
        //    /// 
        //    PortMin = 0,    // Min
        //    Port0 = 0,  // PortA	out
        //    Port1 = 1,  // PortB	in
        //    Port2 = 2,  // PortCL	out
        //    Port3 = 3,  // PortCH	in
        //    PortMax = 3,    // Max
        //    PortRangeOver = 4
        //};

        public enum IOBoardDOutCodeBasedAddress : ushort
        {
            //DoorOpen = 0x01,
            //DoorClose = 0x02,

            //LeverIn = 0x10,
            //LeverOut = 0x20,

            //LeverLamp = 0x01,

            //AirPuff = 0x02,

            RoomLamp = 0x40,

            FeedLamp = 0x80,

            FeedForward = 0x04,
            FeedReverse = 0x08,
        };

        //-------ローカル用

        public enum IoBoardDInCode : ushort
        {
            // Input
            OutSideSensor = 0x02,
            InSideSensor = 0x01,
            PresenceSensor = 0x04,
            FeedSensor = 0x08,
            CwSwitch = 0x10,
            CcwSwitch = 0x20,
        };

        public enum IoBoardDInStatusCode : ushort
        {
            // Flag
            MarmosetReady = 0x01,       // OutFg Bit0 
            MarmosetChkError = 0x02,    // OutFg Bit1 
            ConveyorBusy = 0x04,        // OutFg Bit2 コンベア動作中
            ConveyorError = 0x08,       // OutFg Bit3 コンベア動作タイムアウト
        }

        public enum IoBoardDOutCode : ushort
        {
            // Output SendData[2]
            FeedForward = 0x01,     // PC4.Bit2
            FeedReverse = 0x02,     // PC5.Bit3
            FeedLamp = 0x04,        // P4.Bit1
            RoomLamp = 0x08,        // P3.Bit0
            FeedConveyor = 0x10,    // P5.Bit4

        };

        public enum IoBoardDOutOptionCode : ushort
        {
            // Flag SendData[2]
            MarmotChkStart = 0x01,  // OutFg Bit0 
            ConveyorStart = 0x02,   // OutFg Bit1
        }

        // int用
        public enum IoBoardInPortNum
        {
            OutsideSensor = 0,
            InsideSensor,
            PresenceSensor,
            FeedSensor,
            FeedMotorCWSwitch,
            FeedMotorCCWSwitch,
            Frame0Reserve6,
            Frame0Reserve7,
            Frame1Reserve0,
            Frame1Reserve1,
            Frame1Reserve2,
            Frame1Reserve3,
            Frame1Reserve4,
            Frame1Reserve5,
            Frame1Reserve6,
            Frame1Reserve7,
            MarmosetReady,
            MarmosetTimeoverError,
            ConveyorBusy,
            ConveyorTimeOverError,
            Frame2Reserve4,
            Frame2Reserve5,
            Frame2Reserve6,
            Frame2Reserve7,
            MarmosetCounter0,
            MarmosetCounter1,
            MarmosetCounter2,
            MarmosetCounter3,
            MarmosetCounter4,
            MarmosetCounter5,
            MarmosetCounter6,
            MarmosetCounter7,
            DoorOutsideSensor,
            DoorInsideSensor,
            DoorMotorCWLimit_B,
            DoorMotorCCWLimit_B,
            ExtraFeederError,
            DetectInDirection,
            DetectOutDirection,
            ExtraFeeder2Error,
        }
        public enum IoBoardOutPortNum
        {
            FeedMotorForward = 0,
            FeedMotorReverse,
            FeedLamp,
            RoomLamp,
            FeedConveyor,
            Frame0Reserve5,
            Frame0Reserve6,
            Frame0Reserve7,

            Frame1Reserve0,
            Frame1Reserve1,
            Frame1Reserve2,
            Frame1Reserve3,
            Frame1Reserve4,
            Frame1Reserve5,
            Frame1Reserve6,
            Frame1Reserve7,

            Frame2Reserve0,
            ConveyarStart,
            Frame2Reserve2,
            Frame2Reserve3,
            Frame2Reserve4,
            Frame2Reserve5,
            Frame2Reserve6,
            Frame2Reserve7,

            Frame3Reserve0,
            Frame3Reserve1,
            Frame3Reserve2,
            Frame3Reserve3,
            Frame3Reserve4,
            Frame3Reserve5,
            Frame3Reserve6,
            Frame3Reserve7,

            DoorMotorForward,
            DoorMotorReverse,
            AoutMode,
            ExtraFeederStart,
            ExtraFeeder2Start,
            Frame4Reserve5,
            Frame4Reserve6,
            Frame4Reserve7,

            Frame5Reserve0,
            Frame5Reserve1,
            Frame5Reserve2,
            Frame5Reserve3,
            Frame5Reserve4,
            Frame5Reserve5,
            Frame5Reserve6,
            Frame5Reserve7,

            Frame6bitData0,
            Frame6bitData1,
            Frame6bitData2,
            Frame6bitData3,
            Frame6bitData4,
            Frame6bitData5,
            Frame6bitData6,
            Frame6bitData7,

        }
        //-------
        public IoMicrochip()
        {
            cancellationTokenSource = new CancellationTokenSource();
            RecieveDataHandler += new EventHandler(OnRecieveData);

            task = new Task(new Action(() =>
            {
                mainCountSw.Start();
                while (kanshi.Value)
                {
                    Thread.Sleep(10);
                    SendGetUsb();
                }
            }), cancellationTokenSource.Token);
        }
        void IDisposable.Dispose()
        {
            kanshi.Value = false;
            task?.Wait();
            cancellationTokenSource?.Cancel();
        }

        public void OnRecieveData(object sender, EventArgs e)
        {
            if (mainCountSw.ElapsedMilliseconds > 300)
            {
                for (int i = 0; i < UsbRecieveData.Length; i++)
                {
                    UsbRecieveRiseEdge[i] = StoreByteEdge(UsbRecieveData[i], UsbRecieveDataRecent[i]);
                    UsbRecieveFallEdge[i] = StoreByteFEdge(UsbRecieveData[i], UsbRecieveDataRecent[i]);
                }
                UsbRecieveDataRecent = UsbRecieveData;
                mainCountSw.Restart();
            }

        }
        public byte StoreByteEdge(byte input, byte recent)
        {
            byte ret;
            ret = (byte)(input & ~recent);
            return ret;
        }

        public byte StoreByteFEdge(byte input, byte recent)
        {
            byte ret;
            ret = (byte)(recent & ~input);
            return ret;
        }
        public override string GetVersionData()
        {
            return UsbRecieveData[5].ToString("d4");
        }
        public override bool GetData(IoBoardDInCode ioBoardDInCode)
        {
            switch (ioBoardDInCode)
            {
                //case IOBoardDInCode.MarmosetReady:
                //    return ((Get_Data[2] & (byte)IOBoardDInCode.MarmosetReady) == 0);

                case IoBoardDInCode.OutSideSensor:
                    return ((UsbRecieveData[0] & (byte)IoBoardDInCode.OutSideSensor) == 0);

                case IoBoardDInCode.InSideSensor:
                    return ((UsbRecieveData[0] & (byte)IoBoardDInCode.InSideSensor) == 0);

                case IoBoardDInCode.PresenceSensor:
                    return ((UsbRecieveData[0] & (byte)IoBoardDInCode.PresenceSensor) == 0);

                case IoBoardDInCode.FeedSensor:
                    return ((UsbRecieveData[0] & (byte)IoBoardDInCode.FeedSensor) == 0);

                case IoBoardDInCode.CwSwitch:
                    return ((UsbRecieveData[0] & (byte)IoBoardDInCode.CwSwitch) == 0);

                case IoBoardDInCode.CcwSwitch:
                    return ((UsbRecieveData[0] & (byte)IoBoardDInCode.CcwSwitch) == 0);

                    //case IOBoardDInCode.MarmosetReady:
                    //    return ((Get_Data[2] & (byte)IOBoardDInCode.MarmosetReady) == 0);

                    //case IOBoardDInCode.MarmosetChkErr:
                    //    return ((Get_Data[2] & (byte)IOBoardDInCode.MarmosetChkErr) == 0);

                    //case IOBoardDInCode.ConveyorBusy:
                    //    return ((Get_Data[2] & (byte)IOBoardDInCode.ConveyorBusy) == 0);

                    //case IOBoardDInCode.ConveyorError:
                    //    return ((Get_Data[2] & (byte)IOBoardDInCode.ConveyorError) == 0);
            }
            return false;
        }

        public override bool GetData(IoBoardDInStatusCode ioBoardDInStatusCode, bool n)
        {
            int result;
            switch (ioBoardDInStatusCode)
            {
                case IoBoardDInStatusCode.MarmosetReady:
                    result = UsbRecieveData[2] & (byte)IoBoardDInStatusCode.MarmosetReady;
                    return (result != 0);

                case IoBoardDInStatusCode.MarmosetChkError:
                    result = (UsbRecieveData[2] & (byte)IoBoardDInStatusCode.MarmosetChkError);
                    return (result != 0);

                case IoBoardDInStatusCode.ConveyorBusy:
                    result = ((UsbRecieveData[2] & (byte)IoBoardDInStatusCode.ConveyorBusy));
                    return (result != 0);

                case IoBoardDInStatusCode.ConveyorError:
                    result = ((UsbRecieveData[2] & (byte)IoBoardDInStatusCode.ConveyorError));
                    return (result != 0);

                default:
                    result = 0x00;
                    return false;
            }
        }
        public override bool GetRecieveData(int index)
        {
            BitArray recieveDataBitArray = new BitArray(UsbRecieveData);
            return recieveDataBitArray.Get(index);
        }
        public override bool GetEdge(int index)
        {
            BitArray resiveDataEdge = new BitArray(UsbRecieveRiseEdge);
            return resiveDataEdge.Get(index);
        }
        public override bool GetFEdge(int index)
        {
            BitArray resiveDataFEdge = new BitArray(UsbRecieveFallEdge);
            return resiveDataFEdge.Get(index);
        }


        public int GetCountData()
        {
            const int CounterIndex = 24;
            BitArray recieveDataBitArray = new BitArray(UsbRecieveData);
            byte[] bits = new byte[1];
            recieveDataBitArray.CopyTo(bits, CounterIndex);
            int ret = bits[1];
            return ret;
        }

        //public enum IoBoardDOutLogicalName : int
        //{
        //    /// <summary>
        //    /// 入力論理名
        //    /// </summary>
        //    /// 
        //    Min = 0,    // Min
        //    DoorOpen = 1,
        //    DoorClose = 2,
        //    DoorStop = 3,

        //    LeverIn = 4,
        //    LeverOut = 5,
        //    LeverStop = 6,

        //    LeverLampOn = 7,
        //    LeverLampOff = 8,

        //    AirPuffOn = 9,
        //    AirPuffOff = 10,

        //    RoomLampOn = 11,
        //    RoomLampOff = 12,

        //    FeedLampOn = 13,
        //    FeedLampOff = 14,

        //    FeedForward = 15,
        //    FeedReverse = 16,
        //    FeedStop = 17,

        //    Max = 100,    // Max
        //    RangeOver = 101
        //};

        HIDSimple USB_Device = new HIDSimple();

        const int VendorID = 0x04D8;
        const int ProductID = 0x003F;


        public void SendGetUsb()    //HID通信メソッド
        {
            if (!USB_Connect)
            {
                if (USB_Device.Open(VendorID, ProductID))   // 接続成功、引数は (ベンダID,プロダクトID)
                {
                    USB_Connect = true;

                    for (int i = 0; i < 64; i++)   //送信パケットを初期化
                    {
                        UsbSendData[i] = 0;
                    }
                }
                else      //接続失敗
                {
                }
            }
            else
            {
                try
                {
                    if (USB_Connect)
                    {
                        USB_Device.Send(UsbSendData);  //送信パケットを送信
                        UsbRecieveData = USB_Device.Receive(); //受信パケットに受信
                        RecieveDataHandler(null, null);
                    }
                }
                catch (InvalidOperationException)
                {
                    //オープンされていない
                    USB_Connect = false;
                }
                catch (ArgumentOutOfRangeException)
                {
                    //データサイズエラー
                }
                catch (Exception)
                {
                    USB_Device.Dispose();  //切断
                    USB_Connect = false;
                }
            }
        }

        public override bool AcquireDevice()
        {
            if (!USB_Connect)
            {
                try
                {

                    if (USB_Device.Open(VendorID, ProductID))   // 接続成功、引数は (ベンダID,プロダクトID)
                    {
                        USB_Connect = true;
                        for (uint i = 0; i < 64; i++)   //送信パケットを初期化
                        {
                            UsbSendData[i] = 0;
                        }
                        task.Start();
                        return true;
                    }
                    else    //接続失敗
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return false;
                    //throw;
                }
            }
            else { return true; }
        }

        /// <summary>
        /// DirectIn CheckIO用
        /// </summary>
        /// <param name="ioBoardPortNo">Port number</param>
        /// <param name="inCode">input data</param>
        /// <returns></returns>
        public override bool DirectIn(IoBoardPortNo ioBoardPortNo, out ushort inCode)
        {
            if (ioBoardPortNo == IoBoardPortNo.Port0)
            {
                inCode = BitConverter.ToUInt16(UsbRecieveData, 0);
            }
            else if (ioBoardPortNo == IoBoardPortNo.Port1)
            {
                inCode = BitConverter.ToUInt16(UsbRecieveData, 1);
            }
            else if (ioBoardPortNo == IoBoardPortNo.Port2)
            {
                inCode = BitConverter.ToUInt16(UsbRecieveData, 2);
            }
            else if (ioBoardPortNo == IoBoardPortNo.Port3)
            {
                inCode = BitConverter.ToUInt16(UsbRecieveData, 3);
            }
            else if (ioBoardPortNo == IoBoardPortNo.Port4)
            {
                inCode = BitConverter.ToUInt16(UsbRecieveData, 4);
            }
            else if (ioBoardPortNo == IoBoardPortNo.Port5)
            {
                inCode = BitConverter.ToUInt16(UsbRecieveData, 5);
            }
            else if (ioBoardPortNo == IoBoardPortNo.Port6)
            {
                inCode = BitConverter.ToUInt16(UsbRecieveData, 6);
            }
            else if (ioBoardPortNo == IoBoardPortNo.Port7)
            {
                inCode = BitConverter.ToUInt16(UsbRecieveData, 7);
            }
            else
            {
                inCode = BitConverter.ToUInt16(UsbRecieveData, 0);
            }
            return true;
        }

        /// <summary>
        /// DirectOut CheckIO用
        /// </summary>
        /// <param name="a_IoBoardPortNoObj">Port num</param>
        /// <param name="a_ushortOutCode"></param>
        /// <returns>bool</returns>
        public override bool DirectOut(IoBoardPortNo a_IoBoardPortNoObj, ushort a_ushortOutCode)
        {
            byte[] sendData = BitConverter.GetBytes(a_ushortOutCode);
            try
            {
                if (a_IoBoardPortNoObj == IoBoardPortNo.Port0)
                {
                    UsbSendData[0] = sendData[0];
                }
                else if (a_IoBoardPortNoObj == IoBoardPortNo.Port1)
                {
                    UsbSendData[1] = sendData[0];
                }
                else if (a_IoBoardPortNoObj == IoBoardPortNo.Port2)
                {
                    UsbSendData[2] = sendData[0];
                }
                else if (a_IoBoardPortNoObj == IoBoardPortNo.Port3)
                {
                    UsbSendData[3] = sendData[0];
                }
                else if (a_IoBoardPortNoObj == IoBoardPortNo.Port4)
                {
                    UsbSendData[4] = sendData[0];
                }
                else if (a_IoBoardPortNoObj == IoBoardPortNo.Port5)
                {
                    UsbSendData[5] = sendData[0];
                }
                else if (a_IoBoardPortNoObj == IoBoardPortNo.Port6)
                {
                    UsbSendData[6] = sendData[0];
                }
                else if (a_IoBoardPortNoObj == IoBoardPortNo.Port7)
                {
                    UsbSendData[7] = sendData[0];
                }
                else
                {

                }

                USB_Device.Send(UsbSendData);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public override byte[] GetSendData()
        {
            return UsbSendData;
        }
        public override ushort GetSendData(IoBoardPortNo a_IoBoardPortNoObj)
        {
            if (a_IoBoardPortNoObj == IoBoardPortNo.Port0)
            {
                return UsbSendData[0];
            }
            else if (a_IoBoardPortNoObj == IoBoardPortNo.Port1)
            {
                return UsbSendData[1];
            }
            else if (a_IoBoardPortNoObj == IoBoardPortNo.Port2)
            {
                return UsbSendData[2];
            }
            else if (a_IoBoardPortNoObj == IoBoardPortNo.Port3)
            {
                return UsbSendData[3];
            }
            else if (a_IoBoardPortNoObj == IoBoardPortNo.Port4)
            {
                return UsbSendData[4];
            }
            else if (a_IoBoardPortNoObj == IoBoardPortNo.Port5)
            {
                return UsbSendData[5];
            }
            else if (a_IoBoardPortNoObj == IoBoardPortNo.Port6)
            {
                return UsbSendData[6];
            }
            else if (a_IoBoardPortNoObj == IoBoardPortNo.Port7)
            {
                return UsbSendData[7];
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// SetOutBit0 指定ビット以外0リセットするので注意
        /// ReadModifyWriteにするならOutDataを更新するべき
        /// </summary>
        /// <param name="bitMask">ビットマスク列</param>
        /// <param name="data"></param>
        private void SetOutBit0(byte bitMask, bool data)
        {
            DataBit = bitMask;
            if (data)
            {
                OutData.Value |= (byte)DataBit;
            }
            else
            {
                OutData.Value &= (byte)~DataBit;
            }
            UsbSendData[0] = (byte)OutData.Value;


        }

        private void SetOutBit2(byte bitMask, bool data)
        {
            DataBit = bitMask;
            if (data)
            {
                OutData2.Value |= (byte)DataBit;
            }
            else
            {
                OutData2.Value &= (byte)~DataBit;
            }
            UsbSendData[2] = (byte)OutData2.Value;

        }

        public override void SetOutBit(byte bitMask, bool data, int frameCount)
        {
            OutData3.Value = UsbSendData[frameCount];
            // 16bit毎の為
            DataBit = (byte)(1 << (bitMask % 16));
            if (data)
            {
                OutData3.Value |= DataBit;
            }
            else
            {
                OutData3.Value &= (byte)~DataBit;
            }
            UsbSendData[frameCount] = OutData3.Value;
            //USB_Device.Send(UsbSendData);
        }
        /// <summary>
        /// Motorスピード設定簡易
        /// </summary>
        /// <param name="speed"></param>
        public override void SetMotorSpeed(int speed)
        {
            //const int MotorIndex = 0x30;
            if (speed > 160 && speed < 255)
            {
                UsbSendData[6] = (byte)speed;
            }
            else
            {
                UsbSendData[6] = 255;
            }
        }

        byte BitArrayToByte(BitArray bits)
        {
            if (bits.Count != 8)
            {
                throw new ArgumentException("Not length byte");
            }
            byte[] ret = new byte[1];
            bits.CopyTo(ret, 0);
            return ret[0];
        }

        public void DirectOut(IoBoardDOutCode code, bool status)
        {
            SetOutBit0((byte)code, status);
            USB_Device.Send(UsbSendData);
        }

        public void DirectOut(IoBoardDOutOptionCode code, bool status)
        {
            SetOutBit2((byte)code, status);
            USB_Device.Send(UsbSendData);
        }
        public void DirectSet(IoBoardDOutCode code, bool status)
        {
            SetOutBit0((byte)code, status);
        }
        public void DirectSet(IoBoardDOutOptionCode code, bool status)
        {
            SetOutBit2((byte)code, status);
        }
        public void DirectSend()
        {
            USB_Device.Send(UsbSendData);
        }
        public override bool GetRawStateOfSaveDIn(IoBoardDInLogicalName a_IoBoardDInLogicalNameObj, out bool a_boolRawState)
        {
            bool ret = true;
            a_boolRawState = false;

            switch (a_IoBoardDInLogicalNameObj)
            {
                // 接続機能無し部分は固定
                case IoBoardDInLogicalName.DoorOpen:
                    a_boolRawState = true;
                    break;
                case IoBoardDInLogicalName.DoorClose:
                    a_boolRawState = false;
                    break;
                case IoBoardDInLogicalName.LeverIn:
                    a_boolRawState = true;
                    break;
                case IoBoardDInLogicalName.LeverOut:
                    a_boolRawState = false;
                    break;
                case IoBoardDInLogicalName.LeverSw:
                    a_boolRawState = false;
                    break;
                case IoBoardDInLogicalName.RoomEntrance:
                    a_boolRawState = !GetData(IoBoardDInCode.OutSideSensor);
                    break;
                case IoBoardDInLogicalName.RoomExit:
                    a_boolRawState = !GetData(IoBoardDInCode.InSideSensor);
                    break;
                case IoBoardDInLogicalName.RoomStay:
                    a_boolRawState = !GetData(IoBoardDInCode.PresenceSensor);
                    break;
            }
            return ret;
        }

        public override bool GetUpperStateOfSaveDIn(IoBoardDInLogicalName a_IoBoardDInLogicalNameObj, out bool a_boolLogicalState)
        {
            return GetRawStateOfSaveDIn(a_IoBoardDInLogicalNameObj, out a_boolLogicalState);
        }

        public override bool ReleaseDevice()
        {
            kanshi.Value = false;
            if (USB_Device != null)
            {
                try
                {
                    cancellationTokenSource?.Cancel();
                    task?.Wait();
                    USB_Device.Close();

                }
                catch (Exception)
                {

                    //throw;
                    return false;
                }
                return true;
            }
            return false;
        }

        public override bool SaveDIn()
        {
            return false;
        }

        public override bool SetUpperStateOfDOut(IoBoardDOutLogicalName a_IoBoardDOutLogicalNameObj)
        {
            bool ret = true;
            switch (a_IoBoardDOutLogicalNameObj)
            {
                case IoBoardDOutLogicalName.RoomLampOn:
                    DirectOut(IoBoardDOutCode.RoomLamp, true);
                    //ret = false;
                    return ret;
                case IoBoardDOutLogicalName.RoomLampOff:
                    DirectOut(IoBoardDOutCode.RoomLamp, false);
                    //ret = false;
                    return ret;
                case IoBoardDOutLogicalName.FeedForward:
                    DirectOut(IoBoardDOutCode.FeedForward, true);
                    return ret;
                case IoBoardDOutLogicalName.FeedReverse:
                    DirectOut(IoBoardDOutCode.FeedReverse, true);
                    return ret;
                case IoBoardDOutLogicalName.FeedConveyor:
                    DirectOut(IoBoardDOutOptionCode.ConveyorStart, true);
                    return ret;
                case IoBoardDOutLogicalName.FeedStop:
                    DirectSet(IoBoardDOutCode.FeedForward, false);
                    DirectSet(IoBoardDOutCode.FeedReverse, false);
                    DirectSet(IoBoardDOutOptionCode.ConveyorStart, false);
                    DirectSet(IoBoardDOutCode.FeedConveyor, false);
                    DirectSend();
                    return ret;
                case IoBoardDOutLogicalName.FeedLampOn:
                    DirectOut(IoBoardDOutCode.FeedLamp, true);
                    return ret;
                case IoBoardDOutLogicalName.FeedLampOff:
                    DirectOut(IoBoardDOutCode.FeedLamp, false);

                    return ret;
                default:
                    //mErrorValue = (int)ErrorInfo.ErrorCode.BadPortNum;
                    //errorMsg = "Bad port num[in GetRawStateOfSaveDIn()]";
                    //Debug.WriteLine(errorMsg);
                    ret = false;
                    return ret;
            }
        }
    }

    public class HIDSimple : IDisposable
    {

        /// <summary>
        /// パケットサイズ（IN/OUT共通）
        /// </summary>
        public uint PacketSize
        {
            get;
            private set;
        }

        /// <summary>
        /// HIDDeviceオブジェクトを作成する
        /// </summary>
        public HIDSimple()
        {
            PacketSize = 64;
        }

        private SafeFileHandle HidDeviceObject;

        /// <summary>
        /// USB HIDデバイスをオープンする
        /// </summary>
        /// <param name="vid">デバイスのVID</param>
        /// <param name="pid">デバイスのPID</param>
        /// <returns>成功した場合true、存在しなかった場合false</returns>
        /// <exception cref="InvalidOperationException">デバイスをオープンできなかった場合</exception>
        public bool Open(uint vid, uint pid)
        {
            string path = string.Format(@"\\?\hid#vid_{0,0:x4}&pid_{1,0:x4}", vid, pid);

            Guid guid = new Guid();
            Native.HidD_GetHidGuid(ref guid);

            IntPtr hDeviceInfoSet = Native.SetupDiGetClassDevs(ref guid, 0, IntPtr.Zero, Native.DIGCF_PRESENT | Native.DIGCF_DEVICEINTERFACE);

            Native.SP_DEVICE_INTERFACE_DATA spid = new Native.SP_DEVICE_INTERFACE_DATA();
            spid.cbSize = (uint)Marshal.SizeOf(spid);
            int i = 0;
            while (Native.SetupDiEnumDeviceInterfaces(hDeviceInfoSet, null, ref guid, i, (Native.SP_DEVICE_INTERFACE_DATA)spid))
            {
                i++;

                Native.SP_DEVINFO_DATA devData = new Native.SP_DEVINFO_DATA();
                devData.cbSize = (uint)Marshal.SizeOf(devData);
                int size = 0;
                Native.SetupDiGetDeviceInterfaceDetail(hDeviceInfoSet, spid, IntPtr.Zero, 0, out size, devData);

                IntPtr buffer = Marshal.AllocHGlobal(size);
                Native.SP_DEVICE_INTERFACE_DETAIL_DATA detailData = new Native.SP_DEVICE_INTERFACE_DETAIL_DATA();
                //detailData.cbSize = (uint)Marshal.SizeOf(typeof(Native.SP_DEVICE_INTERFACE_DETAIL_DATA));
                detailData.cbSize = (uint)Native.SP_DEVICE_INTERFACE_DETAIL_DATA.SizeOfThis * 4;

                // Marshal.StructureToPtr(detailData, buffer, false);
                // x86時はbyte packing + 1char, auto -> unicode -> 4 + 2 * 1 から6 x64時は8 bytes packing常時
                Marshal.WriteInt32(buffer, Native.SP_DEVICE_INTERFACE_DETAIL_DATA.SizeOfThis);

                Native.SetupDiGetDeviceInterfaceDetail(hDeviceInfoSet, spid, buffer, size, out size, devData);
                //IntPtr pDevicePath = (IntPtr)((int)buffer + Marshal.SizeOf(typeof(uint)));
                string devicePath = Marshal.PtrToStringAuto(buffer + 4);
                //string devicePath = Marshal.PtrToStringAuto(pDevicePath);
                Marshal.FreeHGlobal(buffer);

                if (devicePath.IndexOf(path) == 0)
                {
                    HidDeviceObject = Native.CreateFile(
                        devicePath,
                        Native.GENERIC_READ | Native.GENERIC_WRITE,
                        0,
                        IntPtr.Zero,
                        Native.OPEN_EXISTING,
                        0,
                        IntPtr.Zero
                    );
                    // VIDとPIDが一致しているが開けないデバイスだった場合に探索に戻る
                    if (HidDeviceObject.IsInvalid)
                    {
                        Native.CloseHandle(HidDeviceObject);
                        continue;
                    }
                    break;
                }
            }

            Native.SetupDiDestroyDeviceInfoList(hDeviceInfoSet);

            if (HidDeviceObject == null)
            { throw new InvalidOperationException(GetErrorMessage()); }


            if (HidDeviceObject.IsInvalid)
            {
                HidDeviceObject = null;
                throw new InvalidOperationException(GetErrorMessage());
            }
            else if (DeviceReady)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// アンマネージドリソースを開放する
        /// </summary>
        public void Dispose()
        {
            Close();
        }

        /// <summary>
        /// デバイスをクローズする
        /// </summary>
        public void Close()
        {
            if (DeviceReady)
            {
                HidDeviceObject.Close();
            }
        }

        ~HIDSimple()
        {
            Close();
        }

        /// <summary>
        /// デバイスが利用可能かどうかを示す
        /// </summary>
        public bool DeviceReady
        {
            get
            {
                if (HidDeviceObject is null)
                {
                    return false;
                }
                if (HidDeviceObject.IsInvalid)
                {
                    return false;
                }
                else if (HidDeviceObject.Equals(IntPtr.Zero))
                {
                    return false;
                }
                return true;
            }
        }

        private string GetErrorMessage()
        {
            int errCode = Marshal.GetLastWin32Error();
            StringBuilder message = new StringBuilder(255);
            Native.FormatMessage(
                0x00001000,
                IntPtr.Zero,
                (uint)errCode,
                0,
                message,
                message.Capacity,
                IntPtr.Zero
            );
            return message.ToString();
        }

        /// <summary>
        /// デバイスにデータを送信する
        /// </summary>
        /// <param name="data">送信するデータ。PacketSize以下でなければならない。</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void Send(params byte[] data)
        {
            if (!DeviceReady)
            {
                throw new InvalidOperationException("デバイスがオープンされていません");
            }
            if (data.Length > PacketSize)
            {
                throw new ArgumentOutOfRangeException("パケットサイズに対してデータが大きすぎます");
            }

            byte[] buff = new byte[PacketSize + 1];
            Array.Clear(buff, 0, buff.Length);
            Array.Copy(data, 0, buff, 1, data.Length);

            try
            {
                uint written = 0;
                bool result = Native.WriteFile(HidDeviceObject, buff, (uint)buff.Length, ref written, IntPtr.Zero);
                if (!result)
                {
                    throw new InvalidOperationException(GetErrorMessage());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// デバイスからデータを受信する
        /// </summary>
        /// <returns>受信したデータ</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public byte[] Receive()
        {
            if (!DeviceReady)
            {
                throw new InvalidOperationException("デバイスがオープンされていません");
            }

            byte[] buff = new byte[PacketSize + 1];
            Array.Clear(buff, 0, buff.Length);
            uint received = 0;
            bool result = Native.ReadFile(HidDeviceObject, buff, (uint)buff.Length, ref received, IntPtr.Zero);
            if (!result)
            {
                throw new InvalidOperationException(GetErrorMessage());
            }
            byte[] buff2 = new byte[PacketSize];
            Array.Copy(buff, 1, buff2, 0, buff2.Length);
            return buff2;
        }
    }

    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [SuppressUnmanagedCodeSecurity()]
    static class Native
    {
        [DllImport("kernel32.dll")]
        internal static extern uint FormatMessage(
            uint dwFlags, IntPtr lpSource,
            uint dwMessageId, uint dwLanguageId,
            StringBuilder lpBuffer, int nSize,
            IntPtr Arguments
        );

        [DllImport("hid.dll", SetLastError = true)]
        internal static extern void HidD_GetHidGuid(
            ref Guid lpHidGuid
        );

        [DllImport("hid.dll", SetLastError = true)]
        internal static extern bool HidD_GetAttributes(
            SafeFileHandle hDevice,
            out HIDD_ATTRIBUTES Attributes
        );

        [DllImport("hid.dll", SetLastError = true)]
        internal static extern bool HidD_GetPreparsedData(
            SafeFileHandle hDevice,
            out IntPtr hData
        );

        [DllImport("hid.dll", SetLastError = true)]
        internal static extern bool HidD_FreePreparsedData(
            SafeFileHandle hData
        );

        [DllImport("hid.dll", SetLastError = true)]
        internal static extern bool HidP_GetCaps(
            SafeFileHandle hData,
            out HIDP_CAPS capabilities
        );

        [DllImport("hid.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool HidD_GetFeature(
            SafeFileHandle hDevice,
            IntPtr hReportBuffer,
            uint ReportBufferLength
        );

        [DllImport("hid.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool HidD_SetFeature(
            SafeFileHandle hDevice,
            IntPtr ReportBuffer,
            uint ReportBufferLength
        );

        [DllImport("hid.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool HidD_GetProductString(
            SafeFileHandle hDevice,
            IntPtr Buffer,
            uint BufferLength
        );

        [DllImport("hid.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool HidD_GetSerialNumberString(
            SafeFileHandle hDevice,
            IntPtr Buffer,
            uint BufferLength
        );

        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern IntPtr SetupDiGetClassDevs(
            ref Guid classGuid,
            int enumerator,
            IntPtr hwndParent,
            int flags
        );

        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern bool SetupDiDestroyDeviceInfoList(
            IntPtr deviceInfoSet
        );

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool SetupDiEnumDeviceInterfaces(
            IntPtr deviceInfoSet,
            SP_DEVINFO_DATA deviceInfoData,
            ref Guid interfaceClassGuid,
            int memberIndex,
            SP_DEVICE_INTERFACE_DATA deviceInterfaceData
        );

        [DllImport("setupapi.dll")]
        internal static extern bool SetupDiOpenDeviceInfo(
            IntPtr deviceInfoSet,
            string deviceInstanceId,
            IntPtr hwndParent,
            int openFlags,
            SP_DEVINFO_DATA deviceInfoData
         );

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool SetupDiGetDeviceInterfaceDetail(
            IntPtr deviceInfoSet,
            SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
            IntPtr deviceInterfaceDetailData,
            int deviceInterfaceDetailDataSize,
            out int requiredSize,
            SP_DEVINFO_DATA deviceInfoData
        );

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern SafeFileHandle CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr SecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile
        );

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern bool CloseHandle(
            SafeFileHandle hHandle
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool ReadFile(
            SafeFileHandle hFile,
            byte[] lpBuffer,
            uint nNumberOfBytesToRead,
            ref uint lpNumberOfBytesRead,
            IntPtr lpOverlapped
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool WriteFile(
            SafeFileHandle hFile,
            byte[] lpBuffer,
            uint nNumberOfBytesToWrite,
            ref uint lpNumberOfBytesWritten,
            IntPtr lpOverlapped
        );

        [StructLayout(LayoutKind.Sequential)]
        internal class SP_DEVICE_INTERFACE_DATA
        {
            internal uint cbSize;
            internal Guid interfaceClassGuid = Guid.Empty;
            internal uint flags = 0;
            internal IntPtr reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal class SP_DEVINFO_DATA
        {
            internal uint cbSize;
            internal Guid classGuid = Guid.Empty;
            internal uint devInst = 0;
            internal IntPtr reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            internal uint cbSize;
            internal uint devicePath;
            //LayoutKind.Sequentialが指定されているので、プロパティはアンマネージド側から無視される
            public static int OffsetOfDevicePath { get { return sizeof(int); } }
            public static int SizeOfThis { get { return OffsetOfDevicePath + (IntPtr.Size == 4 ? 2 : 4); } }
            //SizeOfThis: 32bit-OSでは(4+2)byteを、64bit-OSでは(4+4)byteを返すようにする。
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HIDD_ATTRIBUTES
        {
            public uint Size;
            public ushort VendorID;
            public ushort ProductId;
            public ushort VersionNumber;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct HIDP_CAPS
        {
            public ushort Usage;
            public ushort UsagePage;
            public ushort InputReportByteLength;
            public ushort OutputReportByteLength;
            public ushort FeatureReportByteLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
            public ushort[] Reserved;
            public ushort NumberLinkCollectionNodes;
            public ushort NumberInputButtonCaps;
            public ushort NumberInputValueCaps;
            public ushort NumberInputDataIndices;
            public ushort NumberOutputButtonCaps;
            public ushort NumberOutputValueCaps;
            public ushort NumberOutputDataIndices;
            public ushort NumberFeatureButtonCaps;
            public ushort NumberFeatureValueCaps;
            public ushort NumberFeatureDataIndices;
        }

        internal const int DIGCF_PRESENT = 0x00000002;
        internal const int DIGCF_DEVICEINTERFACE = 0x00000010;
        internal const uint GENERIC_READ = 0x80000000;
        internal const uint GENERIC_WRITE = 0x40000000;
        internal const uint FILE_SHARE_READ = 0x00000001;
        internal const uint FILE_SHARE_WRITE = 0x00000002;
        internal const int OPEN_EXISTING = 3;
        internal const int FILE_FLAG_OVERLAPPED = 0x40000000;
        internal const uint MAX_USB_DEVICES = 16;

    }
}

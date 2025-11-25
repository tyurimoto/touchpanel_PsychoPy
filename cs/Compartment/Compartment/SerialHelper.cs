using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.IO.Ports;
using System.Management;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Compartment
{
    /// <summary>
    /// シリアル通信制御ヘルパー
    /// </summary>
//    [TypeConverter(typeof(DefinitionOrderTypeConverter))]
    public partial class SerialHelper
    {
        string NEW_LINE = "\r\n";

        #region プロパティ・変数

        SerialPort mSerialPort;

        #region エディタクラス

        class ComboBoxComPortEditor : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.DropDown;
            }

            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                IWindowsFormsEditorService s = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;

                // シリアルポート複数デバイス名取得
                Func<string[]> GetSerialDeviceNames = () =>
                {
                    var deviceNameList = new System.Collections.ArrayList();
                    var check = new System.Text.RegularExpressions.Regex("(COM[1-9][0-9]?[0-9]?)");

                    ManagementClass mcPnPEntity = new ManagementClass("Win32_PnPEntity");
                    ManagementObjectCollection manageObjCol = mcPnPEntity.GetInstances();

                    //全てのPnPデバイスを探索しシリアル通信が行われるデバイスを随時追加する
                    foreach (ManagementObject manageObj in manageObjCol)
                    {
                        //Nameプロパティを取得
                        var namePropertyValue = manageObj.GetPropertyValue("Name");
                        if (namePropertyValue == null)
                        {
                            continue;
                        }

                        //Nameプロパティ文字列の一部が"(COM1)～(COM999)"と一致するときリストに追加"
                        string name = namePropertyValue.ToString();
                        if (check.IsMatch(name))
                        {
                            deviceNameList.Add(name);
                        }
                    };

                    //戻り値作成
                    if (deviceNameList.Count > 0)
                    {
                        string[] deviceNames = new string[deviceNameList.Count];
                        int index = 0;
                        foreach (var name in deviceNameList)
                        {
                            deviceNames[index++] = name.ToString();
                        }
                        return deviceNames;
                    }
                    else
                    {
                        return null;
                    }
                };

                if (s != null)
                {
                    var list = new ListBox();

                    // リストボックスに項目をセット
                    {
                        string[] ports = GetSerialDeviceNames();
                        if (ports != null)
                        {
                            foreach (string port in ports) { list.Items.Add(port); }
                            list.SelectedIndex = 0;
                        }
                    }

                    // リストの項目に一致するものがあれば選択する
                    if (list.Items.Contains(value.ToString()))
                    {
                        list.SelectedItem = value.ToString();
                    }

                    // クリックで閉じるようにする
                    EventHandler onclick = (sender, e) =>
                    {
                        s.CloseDropDown();
                    };

                    list.Click += onclick;

                    // ドロップダウンリストの表示
                    s.DropDownControl(list);

                    list.Click -= onclick;

                    // 選択されていればその値を返す
                    return (list.SelectedItem != null) ? list.SelectedItem : value;
                }
                return value;
            }
        }

        class ComboBoxBaudRateEditor : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.DropDown;
            }

            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                IWindowsFormsEditorService s = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;

                if (s != null)
                {
                    var list = new ListBox();

                    // リストボックスに項目をセット
                    list.Items.Add("1200");
                    list.Items.Add("2400");
                    list.Items.Add("4800");
                    list.Items.Add("9600");
                    list.Items.Add("19200");
                    list.Items.Add("38400");
                    list.Items.Add("57600");
                    list.Items.Add("115200");
                    list.Items.Add("230400");
                    list.Items.Add("460800");
                    list.Items.Add("921600");

                    // リストの項目に一致するものがあれば選択する
                    if (list.Items.Contains(value.ToString()))
                    {
                        list.SelectedItem = value.ToString();
                    }

                    // クリックで閉じるようにする
                    EventHandler onclick = (sender, e) =>
                    {
                        s.CloseDropDown();
                    };

                    list.Click += onclick;

                    // ドロップダウンリストの表示
                    s.DropDownControl(list);

                    list.Click -= onclick;

                    // 選択されていればその値を返す
                    return (list.SelectedItem != null) ? list.SelectedItem : value;
                }
                return value;
            }
        }

        class ComboBoxDataBitsEditor : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.DropDown;
            }

            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                IWindowsFormsEditorService s = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;

                if (s != null)
                {
                    var list = new ListBox();

                    // リストボックスに項目をセット
                    list.Items.Add("7");
                    list.Items.Add("8");

                    // リストの項目に一致するものがあれば選択する
                    if (list.Items.Contains(value.ToString()))
                    {
                        list.SelectedItem = value.ToString();
                    }

                    // クリックで閉じるようにする
                    EventHandler onclick = (sender, e) =>
                    {
                        s.CloseDropDown();
                    };

                    list.Click += onclick;

                    // ドロップダウンリストの表示
                    s.DropDownControl(list);

                    list.Click -= onclick;

                    // 選択されていればその値を返す
                    return (list.SelectedItem != null) ? list.SelectedItem : value;
                }
                return value;
            }
        }

        class ComboBoxHandshakeEditor : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.DropDown;
            }

            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                IWindowsFormsEditorService s = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;

                if (s != null)
                {
                    var list = new ListBox();

                    // リストボックスに項目をセット
                    list.Items.Add(System.IO.Ports.Handshake.None.ToString());
                    list.Items.Add(System.IO.Ports.Handshake.XOnXOff.ToString());
                    list.Items.Add(System.IO.Ports.Handshake.RequestToSend.ToString());
                    list.Items.Add(System.IO.Ports.Handshake.RequestToSendXOnXOff.ToString());

                    // リストの項目に一致するものがあれば選択する
                    if (list.Items.Contains(value.ToString()))
                    {
                        list.SelectedItem = value.ToString();
                    }

                    // クリックで閉じるようにする
                    EventHandler onclick = (sender, e) =>
                    {
                        s.CloseDropDown();
                    };

                    list.Click += onclick;

                    // ドロップダウンリストの表示
                    s.DropDownControl(list);

                    list.Click -= onclick;

                    // 選択されていればその値を返す
                    return (list.SelectedItem != null) ? list.SelectedItem : value;
                }
                return value;
            }
        }

        class ComboBoxParityEditor : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.DropDown;
            }

            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                IWindowsFormsEditorService s = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;

                if (s != null)
                {
                    var list = new ListBox();

                    // リストボックスに項目をセット
                    list.Items.Add(System.IO.Ports.Parity.None.ToString());
                    list.Items.Add(System.IO.Ports.Parity.Odd.ToString());
                    list.Items.Add(System.IO.Ports.Parity.Even.ToString());
                    list.Items.Add(System.IO.Ports.Parity.Mark.ToString());
                    list.Items.Add(System.IO.Ports.Parity.Space.ToString());

                    // リストの項目に一致するものがあれば選択する
                    if (list.Items.Contains(value.ToString()))
                    {
                        list.SelectedItem = value.ToString();
                    }

                    // クリックで閉じるようにする
                    EventHandler onclick = (sender, e) =>
                    {
                        s.CloseDropDown();
                    };

                    list.Click += onclick;

                    // ドロップダウンリストの表示
                    s.DropDownControl(list);

                    list.Click -= onclick;

                    // 選択されていればその値を返す
                    return (list.SelectedItem != null) ? list.SelectedItem : value;
                }
                return value;
            }
        }

        class ComboBoxStopBitsEditor : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.DropDown;
            }

            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                IWindowsFormsEditorService s = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;

                if (s != null)
                {
                    var list = new ListBox();

                    // リストボックスに項目をセット
                    list.Items.Add(System.IO.Ports.StopBits.One.ToString());
                    list.Items.Add(System.IO.Ports.StopBits.Two.ToString());
                    list.Items.Add(System.IO.Ports.StopBits.OnePointFive.ToString());

                    // リストの項目に一致するものがあれば選択する
                    if (list.Items.Contains(value.ToString()))
                    {
                        list.SelectedItem = value.ToString();
                    }

                    // クリックで閉じるようにする
                    EventHandler onclick = (sender, e) =>
                    {
                        s.CloseDropDown();
                    };

                    list.Click += onclick;

                    // ドロップダウンリストの表示
                    s.DropDownControl(list);

                    list.Click -= onclick;

                    // 選択されていればその値を返す
                    return (list.SelectedItem != null) ? list.SelectedItem : value;
                }
                return value;
            }
        }

        #endregion エディタクラス

        #region プロパティ

        [Browsable(false)]
        [Category("シリアル設定")]
        [DisplayName("ストップビット数")]
        [Description("転送または受信されたバイト毎のストップビット数です。")]
        [Editor(typeof(ComboBoxStopBitsEditor), typeof(UITypeEditor))]
        public string StopBits { get; set; } = System.IO.Ports.StopBits.One.ToString();

        [Category("シリアル設定")]
        [DisplayName("ポート名")]
        [Description("このシリアルポートで使用するポートです。")]
        [Editor(typeof(ComboBoxComPortEditor), typeof(UITypeEditor))]
        public string ComPort { get; set; } = "";

        [Browsable(false)]
        [Category("シリアル設定")]
        [DisplayName("ボーレート")]
        [Description("このシリアルポートで使用するボーレートです。")]
        [Editor(typeof(ComboBoxBaudRateEditor), typeof(UITypeEditor))]
        public string BaudRate { get; set; } = "19200";

        [Browsable(false)]
        [Category("シリアル設定")]
        [DisplayName("データビット数")]
        [Description("転送または受信されたバイト毎のデータビット数です。")]
        [Editor(typeof(ComboBoxDataBitsEditor), typeof(UITypeEditor))]
        public string DataBits { get; set; } = "8";

        [Browsable(false)]
        [Category("シリアル設定")]
        [DisplayName("フロー制御")]
        [Description("データ交換のフロー制御のためのハンドシェイキングプロトコルです。")]
        [Editor(typeof(ComboBoxHandshakeEditor), typeof(UITypeEditor))]
        public string Handshake { get; set; } = System.IO.Ports.Handshake.None.ToString();

        [Browsable(false)]
        [Category("シリアル設定")]
        [DisplayName("パリティ")]
        [Description("各受信バイトのパリティチェックと各転送バイトの設定のためのスキームです。")]
        [Editor(typeof(ComboBoxParityEditor), typeof(UITypeEditor))]
        public string Parity { get; set; } = System.IO.Ports.Parity.None.ToString();

        public string NewLine { get; set; }
        #endregion

        private string rxLineBuffer = "";

        #endregion プロパティ・変数

        #region イベント

        public Action callbackInitialized = () => { };
        public Action<string> callbackOpened = (comPort) => { };
        public Action callbackClosed = () => { };
        public Action<string> callbackWrote = (str) => { };
        public Action<string> callbackReceivedData = (str) => { };
        public Action<byte[]> callbackReceivedDatagram = (datagram) => { };
        public Action<string> callbackReceivedLine = (str) => { };
        public Action<string> callbackMessageNormal = (str) => { };
        public Action<string> callbackMessageDebug = (str) => { };
        public Action<string> callbackMessageError = (str) => { };

        #endregion

        #region 関数

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SerialHelper()
        {
            // comPort = "STMicroelectronics STLink Virtual COM Port (COM3)";
            return;
        }

        public void Init(SerialPort serialPort)
        {
            mSerialPort = serialPort;
            callbackInitialized();
        }

        private T GetValue<T>(string value)
        {
            foreach (T item in Enum.GetValues(typeof(T)))
            {
                string name = Enum.GetName(typeof(T), item);
                if (name == value)
                {
                    return item;
                }
            }
            return default(T);
        }

        /// <summary>
        /// シリアルポート名取得
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetSerialPortName()
        {
            var check = new System.Text.RegularExpressions.Regex("(COM[1-9][0-9]?[0-9]?)");
            if (check.IsMatch(ComPort))
            {
                return check.Match(ComPort).ToString();
            }
            return "";
        }

        /// <summary>
        /// シリアルポートを開く
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            if (ComPort == "")
            {
                callbackMessageError("ポート名がない");
                return false;
            }

            string portName = GetSerialPortName();
            if (portName == "")
            {
                callbackMessageError("ポート名が無効");
                return false;
            }

            if (mSerialPort.IsOpen == true)
            {
                if (portName == mSerialPort.PortName) { return true; }
                mSerialPort.Close();
            };


            mSerialPort.PortName = portName;
            mSerialPort.BaudRate = Convert.ToInt32(BaudRate);
            mSerialPort.DataBits = Convert.ToInt32(DataBits);
            mSerialPort.Handshake = GetValue<Handshake>(Handshake);
            mSerialPort.Parity = GetValue<Parity>(Parity);
            mSerialPort.StopBits = GetValue<StopBits>(StopBits);

            try
            {
                mSerialPort.Open();
                mSerialPort.ReadExisting();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            callbackOpened(portName);
            return true;
        }

        /// <summary>
        /// シリアルポートを切断
        /// </summary>
        public void Close()
        {
            if (mSerialPort == null) return;
            if (mSerialPort.IsOpen == false) return;
            mSerialPort.Close();
            callbackClosed();
        }

        /// <summary>
        /// シリアルポートのイベントから呼ばれる
        /// </summary>
        /// <param name="waitTime"></param>
        public void OnDataReceived(int waitTime = 0)
        {
            // コマ切れになるのを防ぐ
            if (waitTime > 0)
            {
                Thread.Sleep(500);
            }

            // シリアルポートからデータグラムの読み出し
            Func<byte[]> getReceiveDataGram = () =>
            {
                int len = mSerialPort.BytesToRead;
                if (len <= 0) { return null; }

                byte[] bytes = new byte[len];

                mSerialPort.Read(bytes, 0, bytes.GetLength(0));

                return bytes;
            };

            byte[] datagram = getReceiveDataGram();
            if (datagram == null) { return; }

            for (int i = 0; i < datagram.Length; i++)
            {
                switch (datagram[i])
                {
                    case 0x0D:
                        break;
                    case 0x0A:
                        {
                            string buf = rxLineBuffer;
                            rxLineBuffer = "";
                            callbackReceivedLine(buf);
                            break;
                        }
                    default:
                        {
                            rxLineBuffer += (char)datagram[i];
                            break;
                        }
                }
            }

            callbackReceivedData(Encoding.ASCII.GetString(datagram));
        }
        // public bool boolEnableCallBackReceivedData = false;
        public SyncObject<bool> isEnableCallBackReceivedData = new SyncObject<bool>(false);
        public void OnDataReceived2(int waitTime = 0)
        {
            // コマ切れになるのを防ぐ
            if (waitTime > 0)
            {
                Thread.Sleep(waitTime);
            }

            // シリアルポートからデータグラムの読み出し
            Func<byte[]> getReceiveDataGram = () =>
            {
                int len = mSerialPort.BytesToRead;
                if (len <= 0) { return null; }

                byte[] bytes = new byte[len];

                mSerialPort.Read(bytes, 0, bytes.GetLength(0));

                return bytes;
            };

            byte[] datagram = getReceiveDataGram();
            if (datagram == null) { return; }
            // rxLineBuffer = BitConverter.ToString(datagram);

            //          Debug.WriteLine("Rx:{0}", rxLineBuffer);
            if (isEnableCallBackReceivedData.Value)
            {
                // callbackReceivedData(rxLineBuffer);
                callbackReceivedDatagram(datagram);
            }




            //            rxLineBuffer= String.Format("{0} ", datagram.Length);
            //            callbackReceivedData(rxLineBuffer);

            //              rxLineBuffer = "";
            //            for (int i = 0; i < datagram.Length; i++)
            //            {
            //                // rxLineBuffer += String.Format("{0:X2} ", datagram[i]);
            //            }
            //           rxLineBuffer = BitConverter.ToString(datagram).Replace("-", " ");

            //           callbackReceivedData(rxLineBuffer);
            //          callbackReceivedData(Encoding.ASCII.GetString(datagram));
        }


        /// <summary>
        /// 文字列を送信
        /// </summary>
        /// <param name="str"></param>
        public void Write(string str)
        {
            if (mSerialPort.IsOpen == false) return;

            // 受信したデータを文字列に変換
            byte[] bytes = Encoding.ASCII.GetBytes(str);

            int len = bytes.GetLength(0);


            try
            {
                mSerialPort.Write(bytes, 0, len);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return;
            }
            callbackWrote(str);
        }

        /// <summary>
        /// 文字列を送信
        /// </summary>
        /// <param name="str"></param>
        public void WriteLine(string str)
        {
            Write(str + NEW_LINE);
        }

        /// <summary>
        /// シリアルポート複数デバイス名を取得
        /// </summary>
        /// <returns></returns>
        public string[] GetSerialDeviceNames()
        {
            var deviceNameList = new System.Collections.ArrayList();
            var check = new System.Text.RegularExpressions.Regex("(COM[1-9][0-9]?[0-9]?)");

            ManagementClass mcPnPEntity = new ManagementClass("Win32_PnPEntity");
            ManagementObjectCollection manageObjCol = mcPnPEntity.GetInstances();

            //全てのPnPデバイスを探索しシリアル通信が行われるデバイスを随時追加する
            foreach (ManagementObject manageObj in manageObjCol)
            {
                //Nameプロパティを取得
                var namePropertyValue = manageObj.GetPropertyValue("Name");
                if (namePropertyValue == null)
                {
                    continue;
                }

                //Nameプロパティ文字列の一部が"(COM1)～(COM999)"と一致するときリストに追加"
                string name = namePropertyValue.ToString();
                if (check.IsMatch(name))
                {
                    deviceNameList.Add(name);
                }
            }

            //戻り値作成
            if (deviceNameList.Count > 0)
            {
                string[] deviceNames = new string[deviceNameList.Count];
                int index = 0;
                foreach (var name in deviceNameList)
                {
                    deviceNames[index++] = name.ToString();
                }
                return deviceNames;
            }
            else
            {
                return null;
            }
        }

        #endregion

    }
}

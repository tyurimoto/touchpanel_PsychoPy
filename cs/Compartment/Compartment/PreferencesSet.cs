using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Management;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Compartment
{
    public class PreferencesSet : ICloneable
    {
        #region メソッド:クローン
        // 複製を作成するメソッド
        public PreferencesSet Clone()
        {
            return (PreferencesSet)MemberwiseClone();
        }

        // ICloneable.Cloneの明示的な実装
        object ICloneable.Clone()
        {
            return Clone();
        }
        #endregion

        #region メソッド:設定を保存
#if false
        public bool SaveToSettings()
        {
            bool l_boolRet = true;

            Properties.Settings.Default.COMPARTMENT_NO= iCompartmentNo;
            Properties.Settings.Default.COM_PORT= stringComPort;
            Properties.Settings.Default.COM_BAUDRATE= stringComBaudRate;
            Properties.Settings.Default.COM_DATA_BIT_LENGTH= stringComDataBitLength;
            Properties.Settings.Default.COM_STOP_BIT_LENGTH= stringComStopBitLength;
            Properties.Settings.Default.COM_PARITY = stringComParity;
            Properties.Settings.Default.OPE_TYPE_OF_TASK= stringOpeTypeOfTask;
            Properties.Settings.Default.OPE_NUMBER_OF_TRIAL = iOpeNumberOfTrial;
            Properties.Settings.Default.OPE_TIME_TO_DISPLAY_CORRECT_IMAGE = iOpeTimeToDisplayCorrectImage;
            Properties.Settings.Default.OPE_TIME_TO_DISPLAY_NO_IMAGE = iOpeTimeToDisplayNoImage;
            Properties.Settings.Default.OPE_INTERVAL_TIME_MIN = iOpeIntervalTimeMinimum;
            Properties.Settings.Default.OPE_INTERVAL_TIME_MAX = iOpeIntervalTimeMaximum;
            Properties.Settings.Default.OPE_FEEDING_RATE= iOpeFeedingRate;
            Properties.Settings.Default.OPE_TIME_TO_FEED= iOpeTimeToFeed;
            Properties.Settings.Default.OPE_TIME_TO_TURN_ON_FEED_LAMP = iOpeTimeToTurnOnFeedLamp;
            Properties.Settings.Default.OPE_TIMEOUT_OF_START = iOpeTimeoutOfStart;
            Properties.Settings.Default.OPE_TIME_OUT_OF_TRIAL = iOpeTimeoutOfTrial;
            Properties.Settings.Default.IMG_TRIGGER_IMAGE = stringTriggerImageFile;
            Properties.Settings.Default.IMG_BACK_COLOR= stringBackColor;
            Properties.Settings.Default.IMG_SHAPE_COLOR= stringShapeColor;
            Properties.Settings.Default.IMG_TYPE_OF_SHAPE = stringTypeOfShape;
            Properties.Settings.Default.IMG_SIZE_OF_SHAPE_IN_STEP= stringSizeOfShapeInStep;
            Properties.Settings.Default.IMG_SIZE_OF_SHAPE_IN_PIXEL= iSizeOfShapeInPixel;
            Properties.Settings.Default.SND_SOUND_FILE_OF_END= stringSoundFileOfEnd;
            Properties.Settings.Default.SND_TIME_TO_OUTPUT_SOUND_OF_END= iTimeToOutputSoundOfEnd;
            Properties.Settings.Default.SND_SOUND_FILE_OF_REWARD= stringSoundFileOfReward;
            Properties.Settings.Default.SND_TIME_TO_OUTPUT_SOUND_OF_REWARD= iTimeToOutputSoundOfReward;
            Properties.Settings.Default.MEC_TIMEOUT_OF_LEVER_IN= iTimeoutOfLeverIn;
            Properties.Settings.Default.MEC_TIMEOUT_OF_LEVER_OUT= iTimeoutOfLeverOut;
            Properties.Settings.Default.MEC_TIMEOUT_OF_DOOR_OPEN= itimeoutOfDoorOpen;
            Properties.Settings.Default.MEC_TIMEOUT_OF_DOOR_CLOSE= itimeoutOfDoorClose;
            Properties.Settings.Default.OUT_OUTPUT_RESULT_FILE= stringOutputResultFile;

            Properties.Settings.Default.Save();
            return l_boolRet;
        }
		public bool LoadFromSettings()
        {
            bool l_boolRet = true;

            iCompartmentNo = Properties.Settings.Default.COMPARTMENT_NO;
            stringComPort= Properties.Settings.Default.COM_PORT;
            stringComBaudRate= Properties.Settings.Default.COM_BAUDRATE;
            stringComDataBitLength= Properties.Settings.Default.COM_DATA_BIT_LENGTH;
            stringComStopBitLength= Properties.Settings.Default.COM_STOP_BIT_LENGTH;
            stringComParity= Properties.Settings.Default.COM_PARITY;
            stringOpeTypeOfTask= Properties.Settings.Default.OPE_TYPE_OF_TASK;
            iOpeNumberOfTrial = Properties.Settings.Default.OPE_NUMBER_OF_TRIAL;
            iOpeTimeToDisplayCorrectImage= Properties.Settings.Default.OPE_TIME_TO_DISPLAY_CORRECT_IMAGE;
            iOpeTimeToDisplayNoImage= Properties.Settings.Default.OPE_TIME_TO_DISPLAY_NO_IMAGE;
            iOpeIntervalTimeMinimum= Properties.Settings.Default.OPE_INTERVAL_TIME_MIN;
            iOpeIntervalTimeMaximum= Properties.Settings.Default.OPE_INTERVAL_TIME_MAX;
            iOpeFeedingRate= Properties.Settings.Default.OPE_FEEDING_RATE;
            iOpeTimeToFeed= Properties.Settings.Default.OPE_TIME_TO_FEED;
            iOpeTimeToTurnOnFeedLamp= Properties.Settings.Default.OPE_TIME_TO_TURN_ON_FEED_LAMP;
            iOpeTimeoutOfStart= Properties.Settings.Default.OPE_TIMEOUT_OF_START;
            iOpeTimeoutOfTrial= Properties.Settings.Default.OPE_TIME_OUT_OF_TRIAL;
            stringTriggerImageFile= Properties.Settings.Default.IMG_TRIGGER_IMAGE;
            stringBackColor = Properties.Settings.Default.IMG_BACK_COLOR;
            stringShapeColor = Properties.Settings.Default.IMG_SHAPE_COLOR;
            stringTypeOfShape= Properties.Settings.Default.IMG_TYPE_OF_SHAPE;
            stringSizeOfShapeInStep = Properties.Settings.Default.IMG_SIZE_OF_SHAPE_IN_STEP;
            iSizeOfShapeInPixel= Properties.Settings.Default.IMG_SIZE_OF_SHAPE_IN_PIXEL;
            stringSoundFileOfEnd= Properties.Settings.Default.SND_SOUND_FILE_OF_END;
            iTimeToOutputSoundOfEnd= Properties.Settings.Default.SND_TIME_TO_OUTPUT_SOUND_OF_END;
            stringSoundFileOfReward= Properties.Settings.Default.SND_SOUND_FILE_OF_REWARD;
            iTimeToOutputSoundOfReward= Properties.Settings.Default.SND_TIME_TO_OUTPUT_SOUND_OF_REWARD;
            iTimeoutOfLeverIn= Properties.Settings.Default.MEC_TIMEOUT_OF_LEVER_IN;
            iTimeoutOfLeverOut= Properties.Settings.Default.MEC_TIMEOUT_OF_LEVER_OUT;
            itimeoutOfDoorOpen= Properties.Settings.Default.MEC_TIMEOUT_OF_DOOR_OPEN;
            itimeoutOfDoorClose= Properties.Settings.Default.MEC_TIMEOUT_OF_DOOR_CLOSE;
            stringOutputResultFile= Properties.Settings.Default.OUT_OUTPUT_RESULT_FILE;

            return l_boolRet;
        }

#endif
        #endregion

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

        class ComboBoxColorEditor : UITypeEditor
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
                    list.Items.Add("Black");
                    list.Items.Add("White");
                    list.Items.Add("Red");
                    list.Items.Add("Green");
                    list.Items.Add("Blue");
                    list.Items.Add("Yellow");

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
        class ComboBoxTaskEditor : UITypeEditor
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
                    list.Items.Add("Training");
                    list.Items.Add("DelayMatch");

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
        class ComboBoxSizeOfShapeInStepEditor : UITypeEditor
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
                    list.Items.Add("1");
                    list.Items.Add("2");
                    list.Items.Add("3");
                    list.Items.Add("4");
                    list.Items.Add("5");

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
        class ComboBoxTypeOfShapeEditor : UITypeEditor
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
                    list.Items.Add("Circle");
                    list.Items.Add("Rectangle");
                    list.Items.Add("Triangle");
                    list.Items.Add("Star");

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
        #region プロパティ
        //----------------------------------------------------------------------------
        // Compartmentカテゴリ
        [Category("1.Compartment")]
        [DisplayName("Compartment no")]
        [Description("Compartment no")]
        public int iCompartmentNo { get; set; } = 0;

        //----------------------------------------------------------------------------
        // ID codeカテゴリ
        [Category("2.ID code")]
        [DisplayName("COM: port")]
        [Description("Input COM port")]
        [Editor(typeof(ComboBoxComPortEditor), typeof(UITypeEditor))]
        public string stringComPort { get; set; } = "";

        [Category("2.ID code")]
        [DisplayName("COM: baudrate")]
        [Description("Input COM baudrate")]
        [Editor(typeof(ComboBoxBaudRateEditor), typeof(UITypeEditor))]
        public string stringComBaudRate { get; set; } = "19200";

        [Category("2.ID code")]
        [DisplayName("COM: data bit length")]
        [Description("Input COM data bit length")]
        [Editor(typeof(ComboBoxDataBitsEditor), typeof(UITypeEditor))]
        public string stringComDataBitLength { get; set; } = "8";

        [Category("2.ID code")]
        [DisplayName("COM: Stop bit length")]
        [Description("Input COM stop bit length")]
        [Editor(typeof(ComboBoxStopBitsEditor), typeof(UITypeEditor))]
        public string stringComStopBitLength { get; set; } = System.IO.Ports.StopBits.One.ToString();

        //      [Category("ID code")]
        //      [DisplayName("フロー制御")]
        //      [Description("データ交換のフロー制御のためのハンドシェイキングプロトコルです。")]
        //      [Editor(typeof(ComboBoxHandshakeEditor), typeof(UITypeEditor))]
        //      public string Handshake { get; set; } = System.IO.Ports.Handshake.None.ToString();

        [Category("2.ID code")]
        [DisplayName("COM: Parity")]
        [Description("Input COM parity")]
        [Editor(typeof(ComboBoxParityEditor), typeof(UITypeEditor))]
        public string stringComParity { get; set; } = System.IO.Ports.Parity.None.ToString();

        //----------------------------------------------------------------------------
        // Operationカテゴリ
        [Category("3.Operation")]
        [DisplayName("Type of task")]
        [Description("Input type of task")]
        [Editor(typeof(ComboBoxTaskEditor), typeof(UITypeEditor))]
        public string stringOpeTypeOfTask { get; set; } = "Training";

        [Category("3.Operation")]
        [DisplayName("Number of trial")]
        [Description("Input number of trial")]
        public int iOpeNumberOfTrial { get; set; } = 1;

        [Category("3.Operation")]
        [DisplayName("Time to display correct image")]
        [Description("Input time to display correct image[s]")]
        public int iOpeTimeToDisplayCorrectImage { get; set; } = 1;

        [Category("3.Operation")]
        [DisplayName("Time to display no image")]
        [Description("Input time to display no image[s]")]
        public int iOpeTimeToDisplayNoImage { get; set; } = 1;

        [Category("3.Operation")]
        [DisplayName("Interval time: Minimum")]
        [Description("Input interval time: Minimum[s]")]
        public int iOpeIntervalTimeMinimum { get; set; } = 1;

        [Category("3.Operation")]
        [DisplayName("Interval time: Maximum")]
        [Description("Input interval time: Maximum[s]")]
        public int iOpeIntervalTimeMaximum { get; set; } = 1;

        [Category("3.Operation")]
        [DisplayName("Feeding rate")]
        [Description("Input feeding rate[%]")]
        public int iOpeFeedingRate { get; set; } = 100;

        [Category("3.Operation")]
        [DisplayName("Time to feed")]
        [Description("Input time to feed[ms]")]
        public int iOpeTimeToFeed { get; set; } = 500;

        [Category("3.Operation")]
        [DisplayName("Time to turn on feed lamp")]
        [Description("Input time to turn on feed lamp[ms]")]
        public int iOpeTimeToTurnOnFeedLamp { get; set; } = 500;

        [Category("3.Operation")]
        [DisplayName("Timeout of start")]
        [Description("Input timeout of start[m]")]
        public int iOpeTimeoutOfStart { get; set; } = 10;

        [Category("3.Operation")]
        [DisplayName("Timeout of trial")]
        [Description("Input timeout of trial[m]")]
        public int iOpeTimeoutOfTrial { get; set; } = 10;

        //----------------------------------------------------------------------------
        // Imageカテゴリ
        [Category("4.Image")]
        [DisplayName("Trigger image file")]
        [Description("Input trigger image file name")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor),
            typeof(System.Drawing.Design.UITypeEditor))]
        public string stringTriggerImageFile { get; set; } = "";

        [Category("4.Image")]
        [DisplayName("Back color")]
        [Description("Input back color")]
        [Editor(typeof(ComboBoxColorEditor), typeof(UITypeEditor))]
        public String stringBackColor { get; set; } = "Green";

        [Category("4.Image")]
        [DisplayName("Shape color")]
        [Description("Input shape color")]
        [Editor(typeof(ComboBoxColorEditor), typeof(UITypeEditor))]
        public String stringShapeColor { get; set; } = "White";

        [Category("4.Image")]
        [DisplayName("Type of shape")]
        [Description("Input type of shape")]
        [Editor(typeof(ComboBoxTypeOfShapeEditor), typeof(UITypeEditor))]
        public String stringTypeOfShape { get; set; } = "Circle";

        [Category("4.Image")]
        [DisplayName("Size of shape in step")]
        [Description("Input size of shape in step")]
        [Editor(typeof(ComboBoxSizeOfShapeInStepEditor), typeof(UITypeEditor))]
        public String stringSizeOfShapeInStep { get; set; } = "1";

        [Category("4.Image")]
        [DisplayName("Size of shape in pixel")]
        [Description("Input size of shape in pixel")]
        public int iSizeOfShapeInPixel { get; set; } = 100;

        //----------------------------------------------------------------------------
        // Soundカテゴリ
        [Category("5.Sound")]
        [DisplayName("Sound file of end")]
        [Description("Input sound file name of end")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor),
            typeof(System.Drawing.Design.UITypeEditor))]
        public String stringSoundFileOfEnd { get; set; } = "";

        [Category("5.Sound")]
        [DisplayName("Time to output sound of end")]
        [Description("Input time to output sound of end[s]")]
        public int iTimeToOutputSoundOfEnd { get; set; } = 500;

        [Category("5.Sound")]
        [DisplayName("Sound file of reward")]
        [Description("Inout sound file name of reward")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor),
            typeof(System.Drawing.Design.UITypeEditor))]
        public String stringSoundFileOfReward { get; set; } = "";

        [Category("5.Sound")]
        [DisplayName("Time to output sound of reward")]
        [Description("Input time to output sound of reward[ms]")]
        public int iTimeToOutputSoundOfReward { get; set; } = 500;

        //----------------------------------------------------------------------------
        // Mechanical thingカテゴリ
        [Category("6.Mechanical thing")]
        [DisplayName("Timeout of lever in")]
        [Description("Input timeout of lever in[s]")]
        public int iTimeoutOfLeverIn { get; set; } = 5;

        [Category("6.Mechanical thing")]
        [DisplayName("Timeout of lever out")]
        [Description("Input timeout of lever out[s]")]
        public int iTimeoutOfLeverOut { get; set; } = 5;

        [Category("6.Mechanical thing")]
        [DisplayName("Timeout of door open")]
        [Description("Input timeout of door open[s]")]
        public int itimeoutOfDoorOpen { get; set; } = 5;

        [Category("6.Mechanical thing")]
        [DisplayName("Timeout of door close")]
        [Description("Input timeout of door close[s]")]
        public int itimeoutOfDoorClose { get; set; } = 5;

        //----------------------------------------------------------------------------
        // Outputカテゴリ
        [Category("7.Output")]
        [DisplayName("Output result file")]
        [Description("Input output result file name")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor),
            typeof(System.Drawing.Design.UITypeEditor))]
        public string stringOutputResultFile { get; set; } = "";

        #endregion
    }
}

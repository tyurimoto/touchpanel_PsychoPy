using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;

namespace BlockProgramming
{
    public abstract class FuncArg : BindableBase
    {
        public FuncArg() { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name">引数の表示名</param>
        /// <param name="value">引数の初期値</param>
        public FuncArg(string name, object value)
        {
            argName = name;
            argValue = value;
        }

        public FuncArg(FuncArg source)
        {
            argName = source.argName;
            argValue = source.argValue;
            description = source.description;
        }

        public void ValueChangeSequenceStart()
        {
            OnUndoPointSequenceCreating();
        }

        public void ValueChangeSequenceEnd()
        {
            OnUndopointSequenceCreated();
        }

        protected string argName;
        public string ArgName
        {
            get { return argName; }
            set { SetProperty(ref argName, value); }
        }

        protected string description;
        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value); }
        }

        protected object argValue;
        public virtual object ArgValue
        {
            get { return argValue; }
            set { if (argValue != value) PropertySetter(x => { argValue = x; }, argValue, value); }
        }

        protected bool isValid;
        public bool IsValid
        {
            get { return isValid; }
            set { SetProperty(ref isValid, value); }
        }


        public Type GetArgType() { return argValue.GetType(); }
    }

    public class ValueTypeFuncArg : FuncArg
    {
        /// <summary>
        /// 最大値・最小値を指定するコンストラクタ
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        public ValueTypeFuncArg(string name, object value, int max, int min)
        {
            argName = name;
            maxmin = MaxMin.CreateMaxMin(min, max) ?? MaxMin.CreateMaxMin(0, 10000);
            ArgValue = value is ValueType ? value : 0;
        }

        public ValueTypeFuncArg(string name, object value)
        {
            argName = name;
            maxmin = MaxMin.CreateMaxMin(0, 10000);
            ArgValue = value is ValueType ? value : 0;
        }

        private MaxMin maxmin;
        public MaxMin Maxmin
        {
            get { return maxmin; }
            set { SetProperty(ref maxmin, value); }
        }

        public override object ArgValue
        {
            get { return Convert.ToDouble(argValue); }
            set
            {
                base.ArgValue = value;
                IsValid = Convert.ToDouble(value) >= maxmin.Min && Convert.ToDouble(value) <= maxmin.Max;
            }
        }

        public class MaxMin
        {
            private MaxMin() { }
            private MaxMin(double _max, double _min)
            {
                max = _max;
                min = _min;
            }

            static public MaxMin CreateMaxMin(double _min, double _max)
            {
                if (_max <= _min)
                {
                    return null;
                }
                else
                {
                    return new MaxMin(_max, _min);
                }
            }

            public MaxMin(MaxMin source)
            {
                max = source.max;
                min = source.min;
            }

            private double max;
            public double Max { get => max; set => max = value; }

            private double min;
            public double Min { get => min; set => min = value; }
        }
    }


    public class PathFuncArg : FuncArg
    {
        public PathFuncArg(string name, object value, string filter = "(*.*)|*.*", string initial = @"C:\") : base(name, value)
        {
            Filter = filter;
            InitialPath = initial;

            openFiledialog = new RelayCommand(() =>
            {
                string findPath = InitialPath;
                if (File.Exists(ArgValue.ToString()))
                {
                    try
                    {
                        findPath = Path.GetDirectoryName(ArgValue.ToString());
                    }
                    catch (Exception)
                    {
                        findPath = InitialPath;
                    }
                }

                var dialog = new OpenFileDialog()
                {
                    Title = "ファイルを選択",
                    Filter = filter,
                    InitialDirectory = findPath,
                };

                if (dialog.ShowDialog() == true)
                {
                    ArgValue = dialog.FileName;
                }
            });
        }

        public override object ArgValue
        {
            get => base.ArgValue;
            set
            {
                base.ArgValue = value;
                IsValid = File.Exists(value.ToString());
            }
        }

        public string Filter { get; }
        public string InitialPath { get; }

        public PathFuncArg(PathFuncArg source)
        {
            ArgName = source.ArgName;
            ArgValue = source.ArgValue;
        }

        private RelayCommand openFiledialog;
        public RelayCommand OpenFileDialog
        {
            get => openFiledialog;
            set { SetProperty(ref openFiledialog, value); }
        }
    }
}

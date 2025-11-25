using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace BlockProgramming
{
    [Serializable]
    public class FunctionBlockViewModel : BindableBase
    {
        private UndoRedoManager manager = UndoRedoManager.Instance();

        public FunctionBlockViewModel()
        {
            Items = new ObservableCollection<FunctionItem>();
            Function = new FunctionItem();
        }

        public FunctionBlockViewModel GetSelf()
        {
            return this;
        }



        public ObservableCollection<FunctionItem> Items { get; set; }

        private int selectedIndex = -1;
        public int SelectedIndex
        {
            get => selectedIndex;
            set
            {
                OnUndoPointSequenceCreating();
                PropertySetter(x => { selectedIndex = x; }, selectedIndex, value);

                var source = Items.ElementAt(value);
                if (source.FuncName != Function.FuncName)
                {
                    var target = new FunctionItem();
                    target.FuncName = source.FuncName;
                    target.Description = source.Description;
                    target.IsAsync = source.IsAsync;
                    foreach(var arg in source.FuncArgs)
                    {
                        switch (arg)
                        {
                            case ValueTypeFuncArg va:
                                var valueArg = new ValueTypeFuncArg(va.ArgName, va.ArgValue, (int)va.Maxmin.Max, (int)va.Maxmin.Min);
                                target.FuncArgs.Add(valueArg);
                                break;
                            case PathFuncArg pa:
                                var pathtArg = new PathFuncArg(pa.ArgName, pa.ArgValue, pa.Filter, pa.InitialPath);
                                target.FuncArgs.Add(pathtArg);
                                break;
                            default:
                                break;
                        }
                    }
                    Function = target;

                    foreach(var args in Function.FuncArgs)
                    {
                        manager.Add(args);
                        args.PropertyChanged += (e, o) => { OnPropertyChanged(); };
                    }
                }
                OnUndopointSequenceCreated();
            }
        }

        private FunctionItem function;
        public FunctionItem Function
        {
            get { return function; }
            set { PropertySetter(x=> { function = x; }, function, value); }
        }

        private RelayCommand removeCommand;
        public RelayCommand RemoveCommand
        {
            get { return removeCommand; }
            set { PropertySetter( x=> { removeCommand = x; }, removeCommand, value); }
        }
    }

    public class FuncArgViewModel : BindableBase
    {
        public FuncArgViewModel(FuncArg funcArg) { Arg = funcArg; }

        protected FuncArg arg;
        public FuncArg Arg { get => arg; set => SetProperty(ref arg, value); }
    }

    public class ValueArgViewModel : FuncArgViewModel
    {
        public ValueArgViewModel(FuncArg funcArg) : base(funcArg) {; }
    }

    public class ImagePathArgViewModel : FuncArgViewModel
    {
        public ImagePathArgViewModel(FuncArg funcArg) : base(funcArg) {; }
    }
}

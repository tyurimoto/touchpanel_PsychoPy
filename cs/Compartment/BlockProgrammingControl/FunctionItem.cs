using System;
using System.Collections.ObjectModel;

namespace BlockProgramming
{
    [Serializable]
    public class FunctionItem : BindableBase
    {
        /// <summary>
        /// コンストラクタ　引数なしの場合 Id=-1
        /// </summary>
        public FunctionItem() : this(-1) { }
        public FunctionItem(int id)
        {
            Id = id;
            Description = String.Empty;
            FuncName = String.Empty;
            FuncArgs = new ObservableCollection<FuncArg>();
        }

        private int id;
        public int Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set { PropertySetter( x => { description = x; }, description, value); }
        }

        private string funcName;
        public string FuncName
        {
            get { return funcName; }
            set { PropertySetter( x => { funcName = x; }, funcName, value); }
        }

        private bool isAsync;
        public bool IsAsync
        {
            get => isAsync;
            set { PropertySetter(x => { isAsync = x; }, isAsync, value); }
        }

        private ObservableCollection<FuncArg> funcArgs;
        public ObservableCollection<FuncArg> FuncArgs
        {
            get { return funcArgs; }
            set { SetProperty(ref funcArgs, value); }
        }
    }
}

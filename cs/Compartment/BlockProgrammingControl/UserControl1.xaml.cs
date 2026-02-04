using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace BlockProgramming
{
    /// <summary>
    /// UserControl1.xaml の相互作用ロジック
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        private UndoRedoManager manager = UndoRedoManager.Instance();
        public UndoRedoManager Manager { get; set; }

        public UndoRedoViewModel undoRedoViewModel { get; set; }

        public string JsonString;
        public UserControl1()
        {
            dataStore = new DataStore();
            Items = dataStore.Items;
            Functions = new ObservableCollection<FunctionBlockViewModel>() { };
            Functions.CollectionChanged += OnChanged;
            ListContent = new CompositeCollection
            {
                new CollectionContainer() { Collection = Functions }
            };
            manager.Add(Functions);
            undoRedoViewModel = new UndoRedoViewModel(manager);
            InitializeComponent();
        }

        #region View
        public Action<int> DropCallback { get { return OnDrop; } }

        /// <summary>
        /// 選択したアイテムをドロップ位置に挿入する
        /// </summary>
        /// <param name="targetIndex">変更後のインデックス</param>
        private void OnDrop(int targetIndex)
        {
            var currentIndex = listView.SelectedIndex;
            if (currentIndex < 0 || targetIndex < 0 || targetIndex >= listView.Items.Count) return;
            Functions.Move(currentIndex, targetIndex);
            ModifyActionEventHandler(null, null);
        }
        #endregion

        private void OnChanged(object sender, EventArgs e)
        {
            ModifyActionEventHandler(null, null);
        }

        #region Property
        public CompositeCollection ListContent { get; set; }

        public ObservableCollection<FunctionBlockViewModel> Functions { get; set; }

        public ObservableCollection<FunctionItem> Items { get; set; }

        public int SelectedIndex { get => selectedIndex; set => selectedIndex = value; }

        private int selectedIndex;
        #endregion

        private DataStore dataStore;

        public void ToUpper()
        {
            foreach (var item in Functions)
            {
                item.Function.FuncName = item.Function.FuncName.ToUpper();
            }
        }

        public EventHandler SendJsonEventHandler;
        public EventHandler UndoActionEventHandler;
        public EventHandler ModifyActionEventHandler;

        public void JsonOutput(object sender, RoutedEventArgs e)
        {
            var jsonObject = new List<Message>();


            // OutputResultがTouchDelay前にない場合
            if (Functions.Where(x => x.Function.FuncName == "TouchDelay").Count() > 0
                && Functions.Where((x, index) => x.Function.FuncName == "OutputResult").Count() > 0)
            {
                var TouchDelayIndex = Functions.Select((x, i) => new { Cont = x, Index = i })
                .Where(ano => ano.Cont.Function.FuncName == "TouchDelay")
                .Select(x => x.Index);
                var OutputResultIndex = Functions.Select((x, i) => new { Cont = x, Index = i })
                    .Where(ano => ano.Cont.Function.FuncName == "OutputResult")
                    .Select(x => x.Index);
                int touchIndex = TouchDelayIndex.First();
                int outputIndex = OutputResultIndex.First();


                if (touchIndex < outputIndex)
                {
                    MessageBox.Show("OutputResultをTouchDelayよりも前に配置してください", "情報", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
            // OutputResultがない場合
            var outputResultCheck = Functions.Where(x => x.Function.FuncName == "OutputResult");
            if (outputResultCheck.Count() == 0)
            {
                var ret = MessageBox.Show("OutputResultがありません。\n結果が保存されませんがよろしいですか？", "情報", MessageBoxButton.OKCancel, MessageBoxImage.Information);

                if (ret == MessageBoxResult.Cancel)
                    return;
            }

            foreach (var f in Functions)
            {
                var message = new Message() { Name = "", Param1 = null, Param2 = null };
                if (f.Function.FuncName == "") continue;
                message.Name = f.Function.FuncName.ToString();

                // 無効なパラメータを持つ場合
                if (f.Function.FuncArgs.Any(a => !a.IsValid))
                {
                    var block = listView.ItemContainerGenerator.ContainerFromItem(f);
                    if (block is ListBoxItem target)
                    {
                        target.Focus();
                    }
                    MessageBox.Show(message.Name + ": 無効なパラメータがあります", "情報", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                };

                var valueArgs = f.Function.FuncArgs.OfType<ValueTypeFuncArg>();
                var pathArgs = f.Function.FuncArgs.OfType<PathFuncArg>();

                message.Param1 = valueArgs.Count() > 0 ? Message.GetIntValue(valueArgs.First().ArgValue) : 0;
                message.Param2 = valueArgs.Count() > 1 ? Message.GetIntValue(valueArgs.Skip(1).First().ArgValue) : 0;
                message.Param3 = pathArgs.Count() > 0 ? pathArgs.First().ArgValue.ToString() : null;
                jsonObject.Add(message);
            }
            var jsonString = JsonConvert.SerializeObject(jsonObject, Formatting.Indented,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            //console.Text = jsonString;
            JsonString = jsonString;
            SendJsonEventHandler(JsonString, null);
        }

        public void JsonLoad(string jsontext)
        {
            var errors = new List<string>();
            var messageList = JsonConvert.DeserializeObject<List<Message>>(jsontext,
                new JsonSerializerSettings()
                {
                    Error = (_, arg) => { errors.Add(arg.ErrorContext.Error.Message); arg.ErrorContext.Handled = true; }
                });
            if (errors.Count > 0)
            {
                MessageBox.Show("ファイルを開けませんでした", "情報", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (messageList is null)
            {
                return;
            }
            Functions.Clear();
            foreach (var message in messageList)
            {
                FunctionAdd(message);
            }
        }

        public void FunctionAdd(string funcName)
        {
            var block = new FunctionBlockViewModel()
            {
                Items = new ObservableCollection<FunctionItem>(this.Items),
            };

            if (Items.Where(f => f.FuncName == funcName).Count() > 0)
            {
                block.SelectedIndex = Items.IndexOf(Items.First(f => f.FuncName == funcName));
            }
            block.RemoveCommand = new RelayCommand(
                () =>
                {
                    var self = block.GetSelf();
                    Functions.Remove(self);
                });
            foreach (var arg in block.Function.FuncArgs)
            {
                manager.Add(arg);
                arg.PropertyChanged += OnChanged;
            }
            block.PropertyChanged += OnChanged;
            manager.Add(block);
            Functions.Add(block);
            ModifyActionEventHandler(null, null);
        }

        private void FunctionAdd(Message message)
        {
            var block = new FunctionBlockViewModel()
            {
                Items = new ObservableCollection<FunctionItem>(Items),
            };
            if (Items.Where(f => f.FuncName == message.Name).Count() > 0)
            {
                block.SelectedIndex = Items.IndexOf(Items.First(f => f.FuncName == message.Name));


                foreach (var arg in block.Function.FuncArgs)
                {
                    if (arg is PathFuncArg)
                    {
                        arg.ArgValue = message.Param3 ?? "";
                    }
                    else if (arg is ValueTypeFuncArg)
                    {
                        switch (block.Function.FuncArgs.IndexOf(arg))
                        {
                            case 0:
                                arg.ArgValue = message.Param1;
                                break;
                            case 1:
                                arg.ArgValue = message.Param2;
                                break;
                        }
                    }
                    arg.PropertyChanged += OnChanged;
                }
            }
            block.RemoveCommand = new RelayCommand(
                () =>
                {
                    var self = block.GetSelf();
                    Functions.Remove(self);
                });
            block.PropertyChanged += OnChanged;
            manager.Add(block);
            Functions.Add(block);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Functions[0].Function.FuncName = "HOGEHOGE";
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            FunctionAdd("fuga");
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Functions.Clear();
            FunctionAdd("DrawScreenReset");
            FunctionAdd("ViewTriggerImage");
            FunctionAdd("WaitTouchTrigger");
            FunctionAdd("DrawScreenReset");
            FunctionAdd("Delay");
            FunctionAdd("ViewCorrectImage");
            FunctionAdd("Delay");
            FunctionAdd("DrawScreenReset");
            FunctionAdd("Delay");
            FunctionAdd("ViewCorrectWrongImage");
            FunctionAdd("WaitCorrectTouchTrigger");
            FunctionAdd("DrawScreenReset");
            FunctionAdd("PlaySound");
            FunctionAdd("FeedSound");
            FunctionAdd("FeedLamp");
            FunctionAdd("Feed");
            FunctionAdd("OutputResult");
            FunctionAdd("TouchDelay");
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            string json =
                @"[
                    {
                        ""ActionName"": ""DrawScreenReset""
                    },
                    {
                        ""ActionName"": ""WaitTouchTrigger"",
                        ""Param1"": 1000
                    },
                    {
                        ""ActionName"": ""Delay"",
                        ""Param1"": 3000,
                        ""Param2"": 4000
                    }
                ]";
            JsonLoad(json);
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            manager.Undo();
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            manager.Redo();
        }

        public class UndoRedoViewModel : BindableBase
        {
            public UndoRedoViewModel(UndoRedoManager manager)
            {
                manager.CanUndoChange += (object sender, EventArgs e) => { this.CanUndo = manager.CanUndo; };
                manager.CanRedoChange += (object sender, EventArgs e) => { this.CanRedo = manager.CanRedo; };
            }

            private bool canUndo;
            public bool CanUndo { get => canUndo; set { SetProperty(ref canUndo, value); } }
            private bool canRedo;
            public bool CanRedo { get => canRedo; set { SetProperty(ref canRedo, value); } }
        }
    }

    public class RelayCommand : ICommand
    {
        private Action action;

        public RelayCommand(Action a)
        {
            action = a;
        }

#pragma warning disable CS0067 // Event is never used
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067

        public bool CanExecute(object parameter)
        {
            return action != null;
        }

        public void Execute(object parameter)
        {
            action.Invoke();
        }
    }

    class Helper
    {
        /// <summary>
        /// currentの親要素のうちT型で直近のものを探す
        /// </summary>
        /// <typeparam name="T">探す型</typeparam>
        /// <param name="current">現在の子要素</param>
        /// <returns>T型の親要素</returns>
        public static T FindAncestor<T>(Visual visual)
            where T : DependencyObject
        {
            var current = visual as DependencyObject;
            do
            {
                if (current is T) return (T)current;
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }
    }

    public interface IUndoRedoCommand
    {
        void Undo();
        void Redo();
    }

    public class UndoPointCreatedArgs : EventArgs
    {
        public IUndoRedoCommand Command { get; set; }
    }

    public interface INotifyUndoPointCreated
    {
        event EventHandler<UndoPointCreatedArgs> UndoPointCreated;
    }

    public interface INotifyUndPpointSequenceCreated : INotifyUndoPointCreated
    {
        event EventHandler<UndoPointCreatedArgs> UndoPointSequenceCreating;
        event EventHandler<UndoPointCreatedArgs> UndoPointSequenceCreated;
    }

    public class UndoRedoManager
    {
        public EventHandler CanUndoChange;
        public EventHandler CanRedoChange;

        public static UndoRedoManager Instance()
        {
            if (instance == null)
            {
                instance = new UndoRedoManager();
            }
            return instance;
        }
        private static UndoRedoManager instance;
        private UndoRedoManager() { }

        private bool isReserving = false;
        public bool IsReserving
        {
            get => isReserving;
            set
            {
                if (!value && reservingCommand.Count > 0)
                {
                    redoStack.Clear();
                    undoStack.Push(reservingCommand);
                    CanUndo = undoStack.Count > 0;
                    CanRedo = redoStack.Count > 0;
                    reservingCommand = new ReservableUndoRedoCommand();
                }
                isReserving = value;
            }
        }

        private bool canUndo;
        public bool CanUndo
        {
            get => canUndo;
            private set
            {
                if (canUndo != value)
                {
                    canUndo = value;
                    CanUndoChange?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private bool canRedo;
        public bool CanRedo
        {
            get => canRedo;
            private set
            {
                if (canRedo != value)
                {
                    canRedo = value;
                    CanRedoChange?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private ReservableUndoRedoCommand reservingCommand = new ReservableUndoRedoCommand();

        public void Add(INotifyUndoPointCreated obj)
        {
            obj.UndoPointCreated += CreatedUndoPoint;
        }

        public void Remove(INotifyUndoPointCreated obj)
        {
            obj.UndoPointCreated -= CreatedUndoPoint;
        }

        public void Add(INotifyUndPpointSequenceCreated obj)
        {
            obj.UndoPointCreated += CreatedUndoPoint;
            obj.UndoPointSequenceCreating += CreatingUndoPoinSequence;
            obj.UndoPointSequenceCreated += CreatedUndoPoinSequence;
        }

        public void Remove(INotifyUndPpointSequenceCreated obj)
        {
            obj.UndoPointCreated -= CreatedUndoPoint;
            obj.UndoPointSequenceCreating -= CreatingUndoPoinSequence;
            obj.UndoPointSequenceCreated -= CreatedUndoPoinSequence;
        }

        public void Add(System.Collections.Specialized.INotifyCollectionChanged obj)
        {
            obj.CollectionChanged += CreateCollectionUndoPoint;
        }

        public void Remove(System.Collections.Specialized.INotifyCollectionChanged obj)
        {
            obj.CollectionChanged -= CreateCollectionUndoPoint;
        }

        public void Undo()
        {
            if (undoStack.Count() < 1) return;
            var command = undoStack.Pop();
            command.Undo();
            redoStack.Push(command);
            CanUndo = undoStack.Count > 0;
            CanRedo = redoStack.Count > 0;
        }

        public void Redo()
        {
            if (redoStack.Count() < 1) return;
            var command = redoStack.Pop();
            command.Redo();
            undoStack.Push(command);
            CanUndo = undoStack.Count > 0;
            CanRedo = redoStack.Count > 0;
        }

        private Stack<IUndoRedoCommand> undoStack = new Stack<IUndoRedoCommand>();
        private Stack<IUndoRedoCommand> redoStack = new Stack<IUndoRedoCommand>();

        private void CreatedUndoPoint(object sender, UndoPointCreatedArgs e)
        {
            Debug.WriteLine(sender.GetType() + ":" + sender.GetHashCode());

            if (isReserving)
            {
                reservingCommand.Add(e.Command);
            }
            else
            {
                redoStack.Clear();
                undoStack.Push(e.Command);
                CanUndo = undoStack.Count > 0;
                CanRedo = redoStack.Count > 0;

            }
        }

        private void CreatingUndoPoinSequence(object sender, UndoPointCreatedArgs e)
        {
            isReserving = true;
        }

        private void CreatedUndoPoinSequence(object sender, UndoPointCreatedArgs e)
        {
            isReserving = false;
            if (reservingCommand.Count > 0)
            {
                redoStack.Clear();
                undoStack.Push(reservingCommand);
                reservingCommand = new ReservableUndoRedoCommand();
                CanUndo = undoStack.Count > 0;
                CanRedo = redoStack.Count > 0;
            }
        }

        private void CreateCollectionUndoPoint(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    var command1 = new CollectionUndoRedoCommand<IList>(
                        (_, items) =>
                        {
                            ((System.Collections.Specialized.INotifyCollectionChanged)sender).CollectionChanged -= CreateCollectionUndoPoint;
                            foreach (var item in items)
                            {
                                ((IList)sender).Remove(item);
                            }
                            ((System.Collections.Specialized.INotifyCollectionChanged)sender).CollectionChanged += CreateCollectionUndoPoint;
                        },
                        (index, items) =>
                        {
                            ((System.Collections.Specialized.INotifyCollectionChanged)sender).CollectionChanged -= CreateCollectionUndoPoint;
                            foreach (var item in items)
                            {
                                ((IList)sender).Insert(index, item);
                            }
                            ((System.Collections.Specialized.INotifyCollectionChanged)sender).CollectionChanged += CreateCollectionUndoPoint;
                        },
                        e.NewItems,
                        e.NewItems,
                        e.NewStartingIndex,
                        e.NewStartingIndex
                        );
                    redoStack.Clear();
                    undoStack.Push(command1);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    var command2 = new CollectionUndoRedoCommand<IList>(
                        (index, items) =>
                        {
                            ((System.Collections.Specialized.INotifyCollectionChanged)sender).CollectionChanged -= CreateCollectionUndoPoint;
                            foreach (var item in items)
                            {
                                ((IList)sender).Insert(index, item);
                            }
                            ((System.Collections.Specialized.INotifyCollectionChanged)sender).CollectionChanged += CreateCollectionUndoPoint;
                        },
                        (_, items) =>
                        {
                            ((System.Collections.Specialized.INotifyCollectionChanged)sender).CollectionChanged -= CreateCollectionUndoPoint;
                            foreach (var item in items)
                            {
                                ((IList)sender).Remove(item);
                            }
                            ((System.Collections.Specialized.INotifyCollectionChanged)sender).CollectionChanged += CreateCollectionUndoPoint;
                        },
                        e.OldItems,
                        e.OldItems,
                        e.OldStartingIndex,
                        e.OldStartingIndex
                        );
                    redoStack.Clear();
                    undoStack.Push(command2);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    var command3 = new CollectionUndoRedoCommand<IList>(
                        (_, items) =>
                        {
                            ((System.Collections.Specialized.INotifyCollectionChanged)sender).CollectionChanged -= CreateCollectionUndoPoint;
                            foreach (var item in items)
                            {
                                ((IList)sender).Remove(item);
                            }
                            ((System.Collections.Specialized.INotifyCollectionChanged)sender).CollectionChanged += CreateCollectionUndoPoint;
                        },
                        (index, items) =>
                        {
                            ((System.Collections.Specialized.INotifyCollectionChanged)sender).CollectionChanged -= CreateCollectionUndoPoint;
                            foreach (var item in items)
                            {
                                ((IList)sender).Insert(index, item);
                            }
                            ((System.Collections.Specialized.INotifyCollectionChanged)sender).CollectionChanged += CreateCollectionUndoPoint;
                        },
                        e.NewItems,
                        e.NewItems,
                        e.NewStartingIndex,
                        e.NewStartingIndex
                        );

                    var command4 = new CollectionUndoRedoCommand<IList>(
                        (index, items) =>
                        {
                            ((System.Collections.Specialized.INotifyCollectionChanged)sender).CollectionChanged -= CreateCollectionUndoPoint;
                            foreach (var item in items)
                            {
                                ((IList)sender).Insert(index, item);
                            }
                            ((System.Collections.Specialized.INotifyCollectionChanged)sender).CollectionChanged += CreateCollectionUndoPoint;
                        },
                        (_, items) =>
                        {
                            ((System.Collections.Specialized.INotifyCollectionChanged)sender).CollectionChanged -= CreateCollectionUndoPoint;
                            foreach (var item in items)
                            {
                                ((IList)sender).Remove(item);
                            }
                            ((System.Collections.Specialized.INotifyCollectionChanged)sender).CollectionChanged += CreateCollectionUndoPoint;
                        },
                        e.NewItems,
                        e.NewItems,
                        e.OldStartingIndex,
                        e.NewStartingIndex
                        );
                    reservingCommand.Add(command4);
                    reservingCommand.Add(command3);
                    redoStack.Clear();
                    undoStack.Push(reservingCommand);
                    reservingCommand = new ReservableUndoRedoCommand();
                    isReserving = false;

                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    undoStack.Clear();
                    redoStack.Clear();
                    break;
                default:
                    break;
            }
            CanUndo = undoStack.Count > 0;
            CanRedo = redoStack.Count > 0;
        }
    }
}

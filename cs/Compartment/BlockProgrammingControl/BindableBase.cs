using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace BlockProgramming
{
    /// <summary>
    /// INotifyPropertyChangedの実装
    /// </summary>
    public abstract class BindableBase : INotifyPropertyChanged, INotifyUndPpointSequenceCreated
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<UndoPointCreatedArgs> UndoPointCreated;
        public event EventHandler<UndoPointCreatedArgs> UndoPointSequenceCreating;
        public event EventHandler<UndoPointCreatedArgs> UndoPointSequenceCreated;

        protected void PropertySetter<T>(Action<T> action, T current, T next, [CallerMemberName] String propertyName = null)
        {
            action += (_) => { OnPropertyChanged(propertyName); };
            var command = new PropertyChangedCommand<T>(action, current, next);
            OnUndoPointCreated(command);
            action(next);
        }

        /// <summary>
        /// T型のプロパティのセッター
        /// </summary>
        /// <typeparam name="T">プロパティの型</typeparam>
        /// <param name="storage">プロパティへの参照</param>
        /// <param name="value">プロパティ設定値</param>
        /// <param name="propertyName">プロパティの名前</param>
        /// <returns>値の変更があったときtrue</returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value)) return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnUndoPointCreated(IUndoRedoCommand command)
        {
            
            UndoPointCreated?.Invoke(this, new UndoPointCreatedArgs() { Command = command });
        }

        protected void OnUndoPointSequenceCreating()
        {
            UndoPointSequenceCreating?.Invoke(this, new UndoPointCreatedArgs());
        }

        protected void OnUndopointSequenceCreated()
        {
            UndoPointSequenceCreated?.Invoke(this, new UndoPointCreatedArgs());
        }
    }

    public class PropertyChangedCommand<T> : IUndoRedoCommand
    {
        public PropertyChangedCommand(Action<T> action, T undoArg, T redoArg)
        {
            _action = action;
            _undoArg = undoArg;
            _redoArg = redoArg;
        }

        public void Undo()
        {
            _action(_undoArg);
        }

        public void Redo()
        {
            _action(_redoArg);
        }

        private Action<T> _action;
        private T _undoArg;
        private T _redoArg;
    }

    public class ReservableUndoRedoCommand : IUndoRedoCommand
    {
        private List<IUndoRedoCommand> list = new List<IUndoRedoCommand>();

        public int Count { get => list.Count; }

        public void Add(IUndoRedoCommand command)
        {
            list.Add(command);
        }

        public void Undo()
        {
            for (int i = Count - 1; i >= 0; i--)
            {
                list[i].Undo();
            }
        }

        public void Redo()
        {
            foreach (var command in list)
            {
                command.Redo();
            }
        }

    }

    public class CollectionUndoRedoCommand<T> : IUndoRedoCommand where T: IList
    {
        public CollectionUndoRedoCommand(Action<int, T> undo, Action<int, T> redo, T undoItems, T redoItems, int undoIndex, int redoIndex)
        {
            _undoAction = undo;
            _redoAction = redo;
            _undoItems = undoItems;
            _redoItems = redoItems;
            _undoIndex = undoIndex;
            _redoIndex = redoIndex;
        }

        public void Undo()
        {
            _undoAction(_undoIndex, _undoItems);
        }

        public void Redo()
        {
            _redoAction(_redoIndex, _redoItems);
        }

        private Action<int, T> _undoAction;
        private Action<int, T> _redoAction;
        private int _undoIndex;
        private int _redoIndex;
        private T _undoItems;
        private T _redoItems;
    }
}

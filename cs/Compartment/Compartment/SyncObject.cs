namespace Compartment
{
    public class SyncObject<T>
    {
        public SyncObject() { return; }
        /// <summary>
        /// SyncObject ジェネリック型オブジェクト
        /// </summary>
        /// <param name="value">初期値</param>
        public SyncObject(T value) { Set(value); }
        private readonly object SyncLock = new object();
        private T Value_;
        public T Value { get { return Get(); } set { Set(value); } }
        public T Get() { lock (SyncLock) { return Value_; } }
        public void Set(T value) { lock (SyncLock) { Value_ = value; } }
    }
}

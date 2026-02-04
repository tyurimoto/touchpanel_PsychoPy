using System;

namespace Compartment
{
    /// <summary>
    /// デバッグモード用のダミーRFIDリーダー
    /// 手動でRFID値を設定可能
    /// </summary>
    public class RFIDReaderDummy
    {
        // ID Code重複排除用にID Codeを保存
        public SyncObject<string> CurrentIDCode = new SyncObject<string>("");

        // API access property wrapper
        public string RFID => CurrentIDCode.Value;

        public Action<string> callbackReceivedDataSub = (str) => { };

        private bool hasNewID = false;
        private readonly object idLock = new object();

        public RFIDReaderDummy()
        {
        }

        /// <summary>
        /// 手動でRFID値を設定する（デバッグ用）
        /// </summary>
        /// <param name="id">RFID値（16桁の数字）</param>
        public void SetRFID(string id)
        {
            lock (idLock)
            {
                if (CurrentIDCode.Value != id)
                {
                    CurrentIDCode.Value = id;
                    hasNewID = true;

                    // コールバックを呼び出す
                    callbackReceivedDataSub?.Invoke(id);
                }
            }
        }

        /// <summary>
        /// RFID値をクリア
        /// </summary>
        public void ClearRFID()
        {
            lock (idLock)
            {
                CurrentIDCode.Value = "";
                hasNewID = false;
            }
        }

        /// <summary>
        /// ランダムなRFID値を設定（デバッグ用）
        /// </summary>
        public string SetRandomRFID()
        {
            var random = new Random();
            string randomId = "";
            for (int i = 0; i < 16; i++)
            {
                randomId += random.Next(0, 10).ToString();
            }
            SetRFID(randomId);
            return randomId;
        }

        /// <summary>
        /// Universal ID読み取りアクション取得
        /// 実際のRFIDReaderHelperと同じシグネチャ
        /// </summary>
        public Action<byte[]> GetUnivrsalIDAction()
        {
            Action<byte[]> readIdAction = (datagram) =>
            {
                // ダミー実装: SetRFID()で設定された値がある場合のみコールバックを呼び出す
                lock (idLock)
                {
                    if (hasNewID && !string.IsNullOrEmpty(CurrentIDCode.Value))
                    {
                        callbackReceivedDataSub?.Invoke(CurrentIDCode.Value);
                        hasNewID = false;
                    }
                }
            };
            return readIdAction;
        }

        /// <summary>
        /// Doset ID読み取りアクション取得
        /// 実際のRFIDReaderHelperと同じシグネチャ
        /// </summary>
        public Action<byte[]> GetDosetIDAction()
        {
            return GetUnivrsalIDAction();
        }
    }
}

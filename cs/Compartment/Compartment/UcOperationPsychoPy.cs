using System;
using System.Diagnostics;

namespace Compartment
{
    /// <summary>
    /// PsychoPyエンジン - 外部制御専用
    /// C#状態機械を無効化し、APIサーバー経由での外部制御を可能にする
    /// </summary>
    class UcOperationPsychoPy
    {
        readonly ParentHelper<FormMain> mainForm = new ParentHelper<FormMain>();

        private ref OpCollection opCollection => ref mainForm.Parent.opCollection;
        private ref PreferencesDat PreferencesDatOriginal
        {
            get { return ref mainForm.Parent.preferencesDatOriginal; }
        }

        private bool _eventLoggerEnabled = false;

        public UcOperationPsychoPy(FormMain baseForm)
        {
            mainForm.SetParent(baseForm);
            Debug.WriteLine("[PsychoPy] Engine initialized");
        }

        /// <summary>
        /// 状態機械処理（PsychoPyエンジン専用）
        /// Start/Stop コマンドのみ処理し、EventLoggerを制御
        /// </summary>
        public void OnOperationStateMachineProc()
        {
            // コマンド取得（読み取るとNopにリセットされる）
            OpCollection.ECommand command = opCollection.Command;

            // Start コマンド: EventLogger有効化
            if (command == OpCollection.ECommand.Start && !_eventLoggerEnabled)
            {
                mainForm.Parent._hardwareService?.EventLogger.Enable();
                _eventLoggerEnabled = true;
                Debug.WriteLine("[PsychoPy] EventLogger enabled");
            }

            // Stop コマンド: EventLogger無効化
            if (command == OpCollection.ECommand.Stop && _eventLoggerEnabled)
            {
                mainForm.Parent._hardwareService?.EventLogger.Disable();
                _eventLoggerEnabled = false;
                Debug.WriteLine("[PsychoPy] EventLogger disabled");
            }

            // 状態機械は完全に無効化 - APIサーバー経由での外部制御のみ
            return;
        }
    }
}

using System;
using System.Diagnostics;
using System.IO;

namespace Compartment
{
    /// <summary>
    /// PsychoPyエンジン - 外部制御専用
    /// C#状態機械を無効化し、APIサーバー経由での外部制御を可能にする
    /// Pythonスクリプトをプロセスとして起動・停止する
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
        private Process _pythonProcess = null;

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

            // Start コマンド: EventLogger有効化 + Pythonスクリプト起動
            if (command == OpCollection.ECommand.Start && !_eventLoggerEnabled)
            {
                mainForm.Parent._hardwareService?.EventLogger.Enable();
                _eventLoggerEnabled = true;
                Debug.WriteLine("[PsychoPy] EventLogger enabled");

                StartPythonScript();
            }

            // Stop コマンド: Pythonスクリプト停止 + EventLogger無効化
            if (command == OpCollection.ECommand.Stop && _eventLoggerEnabled)
            {
                StopPythonScript();

                mainForm.Parent._hardwareService?.EventLogger.Disable();
                _eventLoggerEnabled = false;
                Debug.WriteLine("[PsychoPy] EventLogger disabled");
            }

            // 状態機械は完全に無効化 - APIサーバー経由での外部制御のみ
            return;
        }

        /// <summary>
        /// Pythonスクリプトをプロセスとして起動
        /// </summary>
        private void StartPythonScript()
        {
            string scriptPath = Program.PsychoPyScriptPath;

            if (string.IsNullOrEmpty(scriptPath) || !File.Exists(scriptPath))
            {
                Debug.WriteLine("[PsychoPy] Script path is empty or file not found: " + scriptPath);
                return;
            }

            try
            {
                _pythonProcess = new Process();
                _pythonProcess.StartInfo.FileName = "python";
                _pythonProcess.StartInfo.Arguments = "\"" + scriptPath + "\"";
                _pythonProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(scriptPath);
                _pythonProcess.StartInfo.UseShellExecute = false;
                _pythonProcess.StartInfo.RedirectStandardOutput = true;
                _pythonProcess.StartInfo.RedirectStandardError = true;
                _pythonProcess.StartInfo.CreateNoWindow = true;

                _pythonProcess.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        Debug.WriteLine("[PsychoPy:stdout] " + e.Data);
                };

                _pythonProcess.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        Debug.WriteLine("[PsychoPy:stderr] " + e.Data);
                };

                _pythonProcess.EnableRaisingEvents = true;
                _pythonProcess.Exited += (sender, e) =>
                {
                    Debug.WriteLine("[PsychoPy] Python process exited with code: " + _pythonProcess.ExitCode);
                    _pythonProcess = null;
                };

                _pythonProcess.Start();
                _pythonProcess.BeginOutputReadLine();
                _pythonProcess.BeginErrorReadLine();

                Debug.WriteLine("[PsychoPy] Python script started: " + scriptPath);
                Debug.WriteLine("[PsychoPy] Python process ID: " + _pythonProcess.Id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[PsychoPy] Failed to start Python script: " + ex.Message);
                _pythonProcess = null;
            }
        }

        /// <summary>
        /// 実行中のPythonプロセスを停止
        /// </summary>
        private void StopPythonScript()
        {
            if (_pythonProcess == null)
                return;

            try
            {
                if (!_pythonProcess.HasExited)
                {
                    _pythonProcess.Kill();
                    _pythonProcess.WaitForExit(3000);
                    Debug.WriteLine("[PsychoPy] Python process killed");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[PsychoPy] Error stopping Python process: " + ex.Message);
            }
            finally
            {
                _pythonProcess = null;
            }
        }
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

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
        private StringBuilder _stderrBuffer = new StringBuilder();
        private bool _killedByUser = false;

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
                _stderrBuffer.Clear();
                _killedByUser = false;

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
                    {
                        Debug.WriteLine("[PsychoPy:stderr] " + e.Data);
                        _stderrBuffer.AppendLine(e.Data);
                    }
                };

                _pythonProcess.EnableRaisingEvents = true;
                _pythonProcess.Exited += OnPythonProcessExited;

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
        /// Pythonプロセス終了時の処理
        /// 異常終了の場合はエラー内容をユーザーに表示する
        /// </summary>
        private void OnPythonProcessExited(object sender, EventArgs e)
        {
            int exitCode = -1;
            try { exitCode = _pythonProcess.ExitCode; } catch { }

            Debug.WriteLine("[PsychoPy] Python process exited with code: " + exitCode);

            // ユーザーがStopで停止した場合はエラー表示しない
            if (_killedByUser)
            {
                _pythonProcess = null;
                return;
            }

            // 正常終了（exit code 0）はエラー表示しない
            if (exitCode == 0)
            {
                _pythonProcess = null;
                return;
            }

            // 異常終了: stderrの内容からエラーメッセージを構築
            string stderr = _stderrBuffer.ToString().Trim();
            string userMessage = BuildErrorMessage(exitCode, stderr);

            // UIスレッドでMessageBoxを表示
            try
            {
                mainForm.Parent.BeginInvoke((MethodInvoker)(() =>
                {
                    MessageBox.Show(mainForm.Parent, userMessage,
                        "Pythonスクリプトエラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[PsychoPy] Failed to show error dialog: " + ex.Message);
            }

            _pythonProcess = null;
        }

        /// <summary>
        /// stderrの内容を解析してユーザー向けエラーメッセージを構築する
        /// </summary>
        private string BuildErrorMessage(int exitCode, string stderr)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Pythonスクリプトがエラーで終了しました。");
            sb.AppendLine();

            // ModuleNotFoundError を検出 → pip install を案内
            var moduleMatch = Regex.Match(stderr, @"ModuleNotFoundError: No module named '([^']+)'");
            if (moduleMatch.Success)
            {
                string moduleName = moduleMatch.Groups[1].Value;
                // サブモジュール（例: "psychopy.visual"）の場合はトップレベルパッケージ名を取得
                string packageName = moduleName.Split('.')[0];

                sb.AppendLine("必要なパッケージ '" + packageName + "' がインストールされていません。");
                sb.AppendLine();
                sb.AppendLine("対処法:");
                sb.AppendLine("コマンドプロンプトで以下を実行してください:");
                sb.AppendLine();
                sb.AppendLine("  pip install " + packageName);
                sb.AppendLine();
                sb.AppendLine("--- エラー詳細 ---");
                sb.AppendLine(TruncateStderr(stderr));
                return sb.ToString();
            }

            // SyntaxError を検出
            if (stderr.Contains("SyntaxError"))
            {
                sb.AppendLine("スクリプトに構文エラーがあります。");
                sb.AppendLine("スクリプトの内容を確認してください。");
                sb.AppendLine();
                sb.AppendLine("--- エラー詳細 ---");
                sb.AppendLine(TruncateStderr(stderr));
                return sb.ToString();
            }

            // FileNotFoundError を検出
            if (stderr.Contains("FileNotFoundError"))
            {
                sb.AppendLine("スクリプトが参照するファイルが見つかりません。");
                sb.AppendLine();
                sb.AppendLine("--- エラー詳細 ---");
                sb.AppendLine(TruncateStderr(stderr));
                return sb.ToString();
            }

            // その他のエラー
            sb.AppendLine("終了コード: " + exitCode);
            sb.AppendLine();
            if (!string.IsNullOrEmpty(stderr))
            {
                sb.AppendLine("--- エラー詳細 ---");
                sb.AppendLine(TruncateStderr(stderr));
            }
            else
            {
                sb.AppendLine("エラー詳細はありません。");
            }

            return sb.ToString();
        }

        /// <summary>
        /// stderr出力が長すぎる場合は末尾部分のみ返す
        /// </summary>
        private string TruncateStderr(string stderr)
        {
            const int maxLength = 1500;
            if (stderr.Length <= maxLength)
                return stderr;

            return "...(省略)...\n" + stderr.Substring(stderr.Length - maxLength);
        }

        /// <summary>
        /// 実行中のPythonプロセスを停止
        /// </summary>
        private void StopPythonScript()
        {
            if (_pythonProcess == null)
                return;

            _killedByUser = true;

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

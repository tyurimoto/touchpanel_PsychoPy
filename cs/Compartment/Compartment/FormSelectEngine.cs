using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;

namespace Compartment
{
    public partial class FormSelectEngine : Form
    {
        public FormSelectEngine()
        {
            string subVersion = "c";

            InitializeComponent();
            radioButtonPsychoPy.Checked = true;  // デフォルトをPsychoPyに変更

            // 対応ケージ判別表示
            labelVersion.Text = "A-Cage Version with eDoor&&CAM Multi ID";
            labelVersion.ForeColor = Color.Red;
            // 位置ラベル長に合わせて自動
            labelVersion.Location = new System.Drawing.Point(530 - labelVersion.Size.Width, labelVersion.Location.Y);

            labelAssemblyVersion.Text = "V" + Assembly.GetExecutingAssembly().GetName().Version.ToString() + subVersion;

            // スクリプト選択UIの初期状態
            UpdateScriptSelectionUI();
        }

        private void radioButtonPsychoPy_CheckedChanged(object sender, EventArgs e)
        {
            UpdateScriptSelectionUI();
        }

        private void UpdateScriptSelectionUI()
        {
            bool enabled = radioButtonPsychoPy.Checked;
            labelScriptPath.Enabled = enabled;
            textBoxScriptPath.Enabled = enabled;
            buttonBrowseScript.Enabled = enabled;
        }

        private void buttonBrowseScript_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Pythonスクリプトを選択";
                ofd.Filter = "Pythonファイル (*.py)|*.py|すべてのファイル (*.*)|*.*";
                ofd.FilterIndex = 1;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    textBoxScriptPath.Text = ofd.FileName;
                }
            }
        }

        /// <summary>
        /// Python環境チェック
        /// Pythonの存在、バージョン、必要パッケージを検証する
        /// </summary>
        /// <param name="errorMessage">エラー時のメッセージ</param>
        /// <returns>true: 環境OK, false: 問題あり</returns>
        private bool CheckPythonEnvironment(out string errorMessage)
        {
            errorMessage = "";

            // 1. python --version でPythonの存在とバージョンを確認
            string versionOutput;
            try
            {
                var proc = new Process();
                proc.StartInfo.FileName = "python";
                proc.StartInfo.Arguments = "--version";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();

                // python --version は stdout または stderr に出力する
                string stdout = proc.StandardOutput.ReadToEnd();
                string stderr = proc.StandardError.ReadToEnd();
                proc.WaitForExit(5000);

                versionOutput = (!string.IsNullOrEmpty(stdout) ? stdout : stderr).Trim();
            }
            catch (Exception)
            {
                errorMessage = "Pythonが見つかりません。\n\n"
                    + "対処法:\n"
                    + "・Python 3.8以上をインストールしてください\n"
                    + "・インストール時に「Add Python to PATH」にチェックを入れてください\n"
                    + "・インストール後、PCを再起動してください";
                return false;
            }

            // バージョン文字列をパース ("Python 3.11.5" → 3, 11)
            if (!string.IsNullOrEmpty(versionOutput) && versionOutput.StartsWith("Python "))
            {
                string versionStr = versionOutput.Substring("Python ".Length);
                string[] parts = versionStr.Split('.');
                if (parts.Length >= 2
                    && int.TryParse(parts[0], out int major)
                    && int.TryParse(parts[1], out int minor))
                {
                    if (major < 3 || (major == 3 && minor < 8))
                    {
                        errorMessage = "Pythonバージョンが古すぎます。\n\n"
                            + "検出: " + versionOutput + "\n"
                            + "必要: Python 3.8 以上\n\n"
                            + "対処法:\n"
                            + "・Python 3.8以上をインストールしてください";
                        return false;
                    }
                }
            }
            else
            {
                errorMessage = "Pythonバージョンを確認できませんでした。\n\n"
                    + "出力: " + versionOutput;
                return false;
            }

            // 2. requests パッケージの確認
            try
            {
                var proc = new Process();
                proc.StartInfo.FileName = "python";
                proc.StartInfo.Arguments = "-c \"import requests\"";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();

                string stderr = proc.StandardError.ReadToEnd();
                proc.WaitForExit(10000);

                if (proc.ExitCode != 0)
                {
                    errorMessage = "必要なPythonパッケージ 'requests' がインストールされていません。\n\n"
                        + "検出したPython: " + versionOutput + "\n\n"
                        + "対処法:\n"
                        + "・コマンドプロンプトで以下を実行してください:\n"
                        + "  pip install requests";
                    return false;
                }
            }
            catch (Exception ex)
            {
                errorMessage = "パッケージチェック中にエラーが発生しました。\n\n" + ex.Message;
                return false;
            }

            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButtonPsychoPy.Checked)
            {
                // PsychoPy選択時はスクリプトパスが必須
                if (string.IsNullOrWhiteSpace(textBoxScriptPath.Text))
                {
                    MessageBox.Show("Pythonスクリプトを選択してください。",
                        "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Python環境チェック
                if (!CheckPythonEnvironment(out string envError))
                {
                    MessageBox.Show(envError, "Python環境エラー",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Program.SelectedEngine = EEngineType.PsychoPy;
                Program.PsychoPyScriptPath = textBoxScriptPath.Text;
            }
            else if (radioButtonBlockEngine.Checked)
            {
                Program.SelectedEngine = EEngineType.BlockProgramming;
            }
            else // radioButton1 (Old Engine)
            {
                Program.SelectedEngine = EEngineType.OldEngine;
            }
            Close();
        }
    }
}

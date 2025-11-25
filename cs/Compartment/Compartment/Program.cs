using System;
using System.Windows.Forms;

namespace Compartment
{
    static class Program
    {

        public static bool EnableNewEngine = false;
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            SetDpiAwareness();
            try
            {
                // プロセスの二重起動禁止
                if (IsThisProcessAlreadyRunning())
                {
                    MessageBox.Show("This program has already been running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new FormSelectEngine());
                    Application.Run(new FormMain());
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// このプロセスが既に起動しているか検査する
        /// </summary>
        /// <returns>
        /// bool:	==true:		既に起動している
        ///			==false:	起動していない
        /// </returns>
        public static bool IsThisProcessAlreadyRunning()
        {
            bool l_boolRet = false;     // 戻り値: 初期化(起動していない)
                                        // このアプリケーションのプロセス名を取得
            string stringThisProcess = System.Diagnostics.Process.GetCurrentProcess().ProcessName;

            // 同名のプロセスが他に存在した場合は、既に起動していると判断
            if (System.Diagnostics.Process.GetProcessesByName(stringThisProcess).Length > 1)
            {
                l_boolRet = true;       // 戻り値: 起動している
            }
            return l_boolRet;
        }

        /// <summary>
        /// HiDPI対応実装
        /// </summary>
        /// <param name="awareness"></param>
        /// <returns></returns>
        [System.Runtime.InteropServices.DllImport("shcore.dll")]
        private static extern bool SetProcessDpiAwareness(EProcessDpiAwareness awareness);

        private enum EProcessDpiAwareness
        {
            ProcessDpiUnaware = 0,
            ProcessSystemDpiAware = 1,
            ProcessPerMonitorDpiAware = 2
        }
        private static void SetDpiAwareness()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                SetProcessDpiAwareness(EProcessDpiAwareness.ProcessPerMonitorDpiAware);
            }
        }

    }
}

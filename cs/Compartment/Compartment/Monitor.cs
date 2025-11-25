using System;
using System.Runtime.InteropServices;
using System.Threading;
namespace MonitorPower
{
    public class Monitor
    {
        const int SC_MONITORPOWER = 0xf170;
        const int WM_SYSCOMMAND = 0x112;
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(int hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern void mouse_event(Int32 dwFlags, Int32 dx, Int32 dy, Int32 dwData, UIntPtr dwExtraInfo);

        private const int MOUSEEVENTF_MOVE = 0x0001;

        public static void PowerSave()
        {
            //省電力
            SendMessage(-1, WM_SYSCOMMAND, SC_MONITORPOWER, 1);
        }
        public static void PowerOff()
        {
            //モニター停止
            SendMessage(-1, WM_SYSCOMMAND, SC_MONITORPOWER, 2);
        }
        public static void PowerOn()
        {
            //モニター復帰
            //SendMessage(-1, WM_SYSCOMMAND, SC_MONITORPOWER, -1);

            mouse_event(MOUSEEVENTF_MOVE, 0, 1, 0, UIntPtr.Zero);
            Thread.Sleep(40);
            mouse_event(MOUSEEVENTF_MOVE, 0, -1, 0, UIntPtr.Zero);
        }

    }
}
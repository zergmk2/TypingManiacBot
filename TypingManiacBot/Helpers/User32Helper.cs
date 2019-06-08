using System;
using System.Runtime.InteropServices;

namespace TypingBot.WinAPI
{
    public class User32Helper
    {
        public const int VK_RETURN = 0x0D;

        public const uint WM_KEYDOWN = 0x100;
        public const uint WM_KEYUP = 0x101;

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern short VkKeyScan(char ch);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
    }
}

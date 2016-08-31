using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TypingBot.WinAPI;

namespace TypingBot.Extensions
{
    public static class BrowserExtensions
    {
        public static IntPtr AxHandle(this WebBrowser browser)
        {
            var childWindows = new List<string>
            {
                "Shell DocObject View",
                "Internet Explorer_Server",
                "MacromediaFlashPlayerActiveX"
            };

            var handle = user32.FindWindowEx(browser.Handle, IntPtr.Zero, "Shell Embedding", null);

            childWindows.ForEach(
            (window) =>
            {
                if (handle != null)
                    handle = user32.FindWindowEx(handle, IntPtr.Zero, window, null);
            });

            return handle;
        }
    }
}

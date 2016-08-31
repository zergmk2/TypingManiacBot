using System;
using System.Windows.Forms;

namespace TypingBot.Extensions
{
    public static class ControlExtensions
    {
        public static void InvokeAction(this Control control, Action action)
        {
            control.Invoke(action);
        }
    }
}

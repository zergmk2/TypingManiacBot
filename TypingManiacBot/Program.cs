using System;
using System.Windows.Forms;
using TypingBot.Detectors;
using TypingBot.Engines;

namespace TypingBot
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(new OmniPageEngine(), new AccordDetector()));
        }
    }
}

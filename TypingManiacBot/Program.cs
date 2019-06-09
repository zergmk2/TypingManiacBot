using System;
using System.Windows.Forms;
using TypingBot.BlobDetectors;
using TypingBot.OcrEngines;

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
            Application.Run(new Form1(new OmniPageOcrEngine(), new AccordBlobDetector()));
        }
    }
}

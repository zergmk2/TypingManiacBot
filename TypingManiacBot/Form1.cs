using System;
using System.Drawing;
using System.Windows.Forms;
using TypingBot.Contracts;
using TypingBot.EventArgs;
using TypingBot.Extensions;
using TypingBot.WinAPI;
using TypingBot.Common;
using System.Linq;
using System.Threading.Tasks;

namespace TypingBot
{
    public partial class Form1 : Form
    {
        private IntPtr wbAxHandle;

        private readonly IOCREngine ocrEngine;
        private readonly IBlobDetector blobDetector;

        private readonly ScreenCaptureStream screenCapture;

        public Form1(IOCREngine ocrEngine, IBlobDetector blobDetector)
        {
            InitializeComponent();

            screenCapture = new ScreenCaptureStream
            (
                webBrowser1.Handle,
                new Rectangle(72, 5, 273, 145),
                true
            );
            screenCapture.NewFrame += screenCapture_NewFrame;

            this.ocrEngine = ocrEngine;
            this.ocrEngine.RecognizedText += ocrEngine_RecognizedText;

            this.blobDetector = blobDetector;
            this.blobDetector.DetectedBlobs += blobDetector_DetectedBlobs;
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            if (screenCapture.IsRunning)
            {
                screenCapture.Stop();
                btnStart.Text = "Start";
            }
            else
            {
                screenCapture.Start();
                btnStart.Text = "Stop";
            }
        }

        private void screenCapture_NewFrame(object sender, NewFrameArgs eventArgs)
        {
            //eventArgs.Frame.Save($"Test\\{Guid.NewGuid().ToString()}.bmp");

            blobDetector.ProcessImage(eventArgs.Frame);
        }

        private void blobDetector_DetectedBlobs(object sender, DetectedBlobsArgs e)
        {
            if (!e.IsBlobsDetected)
            {
                screenCapture.GrabNextFrame();
                return;
            }

            //Console.WriteLine("A");

            BlobPreview.InvokeAction(() => BlobPreview.Image = new Bitmap(e.Blobs.First()));

            if (e.Blobs.Count() == 1)
            {
                ocrEngine.ProcessImage(e.Blobs.First());
                return;
            }

            ocrEngine.ProcessImages(e.Blobs);
        }

        private void ocrEngine_RecognizedText(object sender, RecognizedTextArgs e)
        {
            if (!e.IsTextRecognized)
            {
                screenCapture.SleepAndGrabNextFrame(250);
                return;
            }

            txtOutput.InvokeAction(() => txtOutput.Text = e.Text);
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            webBrowser1.Navigate("http://media.mindjolt.com/media/typing-maniac.swf?izo0ri");
        }

        private void textBox1_TextChanged(object sender, System.EventArgs e)
        {
            foreach (char c in txtOutput.Text.ToLower())
            {
                user32.PostMessage(wbAxHandle, WindowMessages.WM_KEYDOWN, user32.VkKeyScan(c), 0);
                user32.PostMessage(wbAxHandle, WindowMessages.WM_KEYUP, user32.VkKeyScan(c), 0);
            }

            user32.PostMessage(wbAxHandle, WindowMessages.WM_KEYDOWN, VirtualKeys.VK_RETURN, 0);
            user32.PostMessage(wbAxHandle, WindowMessages.WM_KEYUP, VirtualKeys.VK_RETURN, 0);
        }

        private void btnReload_Click(object sender, System.EventArgs e)
        {
            webBrowser1.Refresh(WebBrowserRefreshOption.IfExpired);
        }

        private void cmbSpecial_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            txtOutput.Text = cmbSpecial.GetItemText(cmbSpecial.SelectedItem);
        }

        private async void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //wait a little for the flash object to laod
            await Task.Delay(250);

            wbAxHandle = webBrowser1.AxHandle();
            btnStart.Enabled = wbAxHandle != IntPtr.Zero;
        }
    }
}

using System;
using System.Drawing;
using System.Windows.Forms;
using TypingBot.Contracts;
using TypingBot.EventArgs;
using TypingBot.Extensions;
using TypingBot.WinAPI;
using System.Linq;
using System.Threading.Tasks;
using Accord.Video;

namespace TypingBot
{
    public partial class Form1 : Form
    {
        private IntPtr wbAxHandle;

        private readonly IOCREngine ocrEngine;
        private readonly IBlobDetector blobDetector;

        private readonly WindowCaptureStream windowCapture;

        public Form1(IOCREngine ocrEngine, IBlobDetector blobDetector)
        {
            InitializeComponent();

            windowCapture = new WindowCaptureStream
            (
                webBrowser1.Handle,
                new Rectangle(0, 0, webBrowser1.Width, webBrowser1.Height)
            );
            windowCapture.NewFrame += WindowCapture_NewFrame;

            this.ocrEngine = ocrEngine;
            this.ocrEngine.RecognizedText += ocrEngine_RecognizedText;

            this.blobDetector = blobDetector;
            this.blobDetector.DetectedBlobs += blobDetector_DetectedBlobs;
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            if (windowCapture.IsRunning)
            {
                windowCapture.Stop();
                btnStart.Text = "Start";
            }
            else
            {
                windowCapture.Start();
                btnStart.Text = "Stop";
            }
        }

        private void WindowCapture_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            blobDetector.ProcessImage(eventArgs.Frame);
        }

        private void blobDetector_DetectedBlobs(object sender, DetectedBlobsArgs e)
        {
            BlobPreview.InvokeAction(() => BlobPreview.Image = new Bitmap(e.Blobs.First()));

            ocrEngine.ProcessImages(e.Blobs);
        }

        private void ocrEngine_RecognizedText(object sender, RecognizedTextArgs e)
        {
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

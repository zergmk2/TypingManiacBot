using Accord.Video;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TypingBot.BlobDetectors;
using TypingBot.Extensions;
using TypingBot.OcrEngines;
using TypingBot.WinAPI;

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
            windowCapture.NewFrame += NewFrame;

            this.ocrEngine = ocrEngine;
            this.ocrEngine.RecognizedText += RecognizedText;

            this.blobDetector = blobDetector;
            this.blobDetector.DetectedBlobs += DetectedBlobs;
        }

        private void button1_Click(object sender, EventArgs e)
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

        private void NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            blobDetector.ProcessImage(eventArgs.Frame);
        }

        private void DetectedBlobs(object sender, DetectedBlobsArgs e)
        {
            BlobPreview.InvokeAction(() => BlobPreview.Image = new Bitmap(e.Blobs.First()));

            ocrEngine.ProcessImages(e.Blobs);
        }

        private void RecognizedText(object sender, RecognizedTextArgs e)
        {
            txtOutput.InvokeAction(() => txtOutput.Text = e.Text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate("http://games.coolgames.com/typing-maniac/en/1.0/typing-maniac.swf");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            foreach (char c in txtOutput.Text.ToLower())
            {
                User32Helper.PostMessage(wbAxHandle, User32Helper.WM_KEYDOWN, User32Helper.VkKeyScan(c), 0);
                User32Helper.PostMessage(wbAxHandle, User32Helper.WM_KEYUP, User32Helper.VkKeyScan(c), 0);
            }

            User32Helper.PostMessage(wbAxHandle, User32Helper.WM_KEYDOWN, User32Helper.VK_RETURN, 0);
            User32Helper.PostMessage(wbAxHandle, User32Helper.WM_KEYUP, User32Helper.VK_RETURN, 0);
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            webBrowser1.Refresh(WebBrowserRefreshOption.IfExpired);
        }

        private void cmbSpecial_SelectionChangeCommitted(object sender, EventArgs e)
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

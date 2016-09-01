using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using TypingBot.EventArgs;

namespace TypingBot.Common
{
    public class ScreenCaptureStream
    {
        private readonly IntPtr wndHandle;
        private readonly Rectangle region;
        private readonly bool reqNextFrame;

        private readonly AutoResetEvent canContinue;
        private readonly AutoResetEvent canExit;

        private Thread thrWorker;

        public bool IsRunning
        {
            get
            {
                if (thrWorker != null)
                {
                    return thrWorker.IsAlive;
                }

                return false;
            }
        }

        public event EventHandler<NewFrameArgs> NewFrame;

        private void OnNewFrame(Bitmap frame)
        {
            NewFrame?.Invoke(this, new NewFrameArgs { Frame = frame });
        }

        public ScreenCaptureStream(IntPtr wndHandle, Rectangle region, bool reqNextFrame)
        {
            this.wndHandle = wndHandle;
            this.region = region;
            this.reqNextFrame = reqNextFrame;

            canContinue = new AutoResetEvent(true);
            canExit = new AutoResetEvent(false);
        }

        public void Start()
        {
            if (!IsRunning)
            {
                thrWorker = new Thread(worker);
                thrWorker.Start();
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                canContinue.Set();
                canExit.Set();
            }
        }

        public void GrabNextFrame()
        {
            canContinue.Set();
        }

        public void SleepAndGrabNextFrame(int timeout)
        {
            Thread.Sleep(timeout);
            GrabNextFrame();
        }

        private void worker()
        {
            int width = region.Width;
            int height = region.Height;
            int x = region.Location.X;
            int y = region.Location.Y;
            bool req = reqNextFrame;


            var screenshot = new Bitmap(width, height);

            var fromHwnd = Graphics.FromHwnd(wndHandle);
            var fromImage = Graphics.FromImage(screenshot);

            while (!canExit.WaitOne(0, false))
            {
                if (req)
                    canContinue.WaitOne();

                IntPtr hdc_screen = fromHwnd.GetHdc();
                IntPtr hdc_screenshot = fromImage.GetHdc();

                BitBlt(hdc_screenshot, 0, 0, width, height, hdc_screen, x, y, (int)CopyPixelOperation.SourceCopy);

                fromImage.ReleaseHdc(hdc_screenshot);
                fromHwnd.ReleaseHdc(hdc_screen);

                OnNewFrame(screenshot);

                Thread.Sleep(25);
            }

            screenshot.Dispose();

            fromHwnd.Dispose();
            fromImage.Dispose();
        }

        [DllImport("gdi32.dll")]
        static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hObjectSource, int nXSrc, int nYSrc, int dwRop);
    }
}

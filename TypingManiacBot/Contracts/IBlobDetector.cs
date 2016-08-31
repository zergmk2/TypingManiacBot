using System;
using System.Drawing;
using TypingBot.EventArgs;

namespace TypingBot.Contracts
{
    public interface IBlobDetector
    {
        event EventHandler<DetectedBlobsArgs> DetectedBlobs;
        void ProcessImage(Bitmap image);
    }
}

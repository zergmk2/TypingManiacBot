using System;
using System.Drawing;

namespace TypingBot.BlobDetectors
{
    public interface IBlobDetector
    {
        event EventHandler<DetectedBlobsArgs> DetectedBlobs;
        void ProcessImage(Bitmap image);
    }
}

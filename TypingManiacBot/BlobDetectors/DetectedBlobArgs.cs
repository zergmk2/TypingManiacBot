using System;
using System.Collections.Generic;
using System.Drawing;

namespace TypingBot.BlobDetectors
{
    public class DetectedBlobsArgs : EventArgs
    {
        public IEnumerable<Bitmap> Blobs;
    }
}

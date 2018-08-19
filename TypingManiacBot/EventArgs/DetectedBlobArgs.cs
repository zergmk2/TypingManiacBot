using System.Collections.Generic;
using System.Drawing;

namespace TypingBot.EventArgs
{
    public class DetectedBlobsArgs : System.EventArgs
    {
        public IEnumerable<Bitmap> Blobs;
    }
}

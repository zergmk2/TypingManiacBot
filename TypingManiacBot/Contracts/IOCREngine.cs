using System;
using System.Collections.Generic;
using System.Drawing;
using TypingBot.EventArgs;

namespace TypingBot.Contracts
{
    public interface IOCREngine
    {
        event EventHandler<RecognizedTextArgs> RecognizedText;
        void ProcessImage(Bitmap image);
        void ProcessImages(IEnumerable<Bitmap> images);
    }
}

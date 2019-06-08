using System;
using System.Collections.Generic;
using System.Drawing;

namespace TypingBot.OcrEngines
{
    public interface IOCREngine
    {
        event EventHandler<RecognizedTextArgs> RecognizedText;
        void ProcessImage(Bitmap image);
        void ProcessImages(IEnumerable<Bitmap> images);
    }
}

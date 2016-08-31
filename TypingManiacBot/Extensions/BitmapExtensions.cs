using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace TypingBot.Extensions
{
    public static class BitmapExtensions
    {
        public static Bitmap Crop(this Bitmap bitmap, Rectangle rect)
        {
            return bitmap.Clone(rect, PixelFormat.Format24bppRgb);
        }
    }
}

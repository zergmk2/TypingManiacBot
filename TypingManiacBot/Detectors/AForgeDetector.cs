using System.Drawing;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using TypingBot.Contracts;
using System;
using TypingBot.EventArgs;
using System.Collections.Generic;
using TypingBot.Extensions;

namespace TypingBot.Detectors
{
    public class AForgeDetector : IBlobDetector
    {
        private const int max = 96;

        private readonly ColorFiltering backgroundFilter;
        private readonly BlobCounter blobCounter;

        public AForgeDetector()
        {
            backgroundFilter = new ColorFiltering
            {
                Red = new IntRange(0, max),
                Green = new IntRange(0, max),
                Blue = new IntRange(0, max),
                FillOutsideRange = false
            };

            blobCounter = new BlobCounter
            {
                MinHeight = 10,
                MaxHeight = 13,
                MinWidth = 40,
                FilterBlobs = true
            };
        }

        public event EventHandler<DetectedBlobsArgs> DetectedBlobs;
        private void OnDetectedBlobs(IEnumerable<Bitmap> blobs, bool isBlobsDetected)
        {
            DetectedBlobs?.Invoke
            (
                this,
                new DetectedBlobsArgs
                {
                    Blobs = blobs,
                    IsBlobsDetected = isBlobsDetected
                }
            );
        }

        public void ProcessImage(Bitmap image)
        {
            /*
            Apply filter and process on a dummy Bitmap,
            so when cropping the original image, 
            the colors will not fade away and the ocr process -
            will be accurate.
            */
            using (var dummyImage = new Bitmap(image))
            {
                backgroundFilter.ApplyInPlace(dummyImage);

                blobCounter.ProcessImage(dummyImage);
            }

            var rects = new List<Rectangle>(blobCounter.GetObjectsRectangles());

            //Console.WriteLine(rects.Count);

            if (rects.Count == 0)
            {
                OnDetectedBlobs(null, false);
                return;
            }

            var result = new List<Bitmap>(rects.Count);

            rects.ForEach(
            (rect) =>
            {
                result.Add(image.Crop(rect));
            });

            OnDetectedBlobs(result, true);
        }
    }
}

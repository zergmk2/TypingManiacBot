using Accord;
using Accord.Imaging;
using Accord.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using TypingBot.Extensions;
using TypingBot.Models;

namespace TypingBot.BlobDetectors
{
    public class AccordBlobDetector : IBlobDetector
    {
        private const int max = 96;

        private readonly ColorFiltering backgroundFilter;
        private readonly BlobCounter blobCounter;

        public AccordBlobDetector()
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

        private void RaiseDetectedBlobs(IEnumerable<Bitmap> blobs)
        {
            DetectedBlobs?.Invoke(this, new DetectedBlobsArgs { Blobs = blobs });
        }

        public void ProcessImage(Bitmap image)
        {
            ThreadPool.QueueUserWorkItem((o) => doWork(new Params { Image = new Bitmap(image) }));
        }

        static readonly object work_lock = new object();

        private void doWork(object o)
        {
            var data = o as Params;

            lock (work_lock)
            {
                using (var filteredImage = new Bitmap(data.Image))
                {
                    backgroundFilter.ApplyInPlace(filteredImage);

                    blobCounter.ProcessImage(filteredImage);
                }

                var rects = blobCounter.GetObjectsRectangles();

                int rectsCount = rects.Count();

                if (rectsCount == 0)
                {
                    return;
                }

                var result = new List<Bitmap>(rectsCount);

                foreach (Rectangle rect in rects)
                {
                    result.Add(data.Image.Crop(rect));
                }

                RaiseDetectedBlobs(result);
            }
        }
    }
}

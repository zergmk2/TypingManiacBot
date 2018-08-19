using System.Drawing;
using Accord;
using Accord.Imaging;
using Accord.Imaging.Filters;
using TypingBot.Contracts;
using System;
using TypingBot.EventArgs;
using System.Collections.Generic;
using TypingBot.Extensions;
using TypingBot.Models;
using System.Linq;
using System.Threading;

namespace TypingBot.Detectors
{
    public class AccordDetector : IBlobDetector
    {
        private const int max = 96;

        private readonly ColorFiltering backgroundFilter;
        private readonly BlobCounter blobCounter;

        public AccordDetector()
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
        private void OnDetectedBlobs(IEnumerable<Bitmap> blobs)
        {
            DetectedBlobs?.Invoke
            (
                this,
                new DetectedBlobsArgs
                {
                    Blobs = blobs
                }
            );
        }

        public void ProcessImage(Bitmap image)
        {
            ThreadPool.QueueUserWorkItem
            (
                new WaitCallback(doWork),
                new Params { Image = new Bitmap(image) }
            );
        }

        static readonly object proclock = new object();
        static readonly object croplock = new object();

        private void doWork(object o)
        {
            var data = o as Params;

            lock (proclock)
            {
                using (var dummyImage = new Bitmap(data.Image))
                {
                    backgroundFilter.ApplyInPlace(dummyImage);

                    blobCounter.ProcessImage(dummyImage);
                }
            }

            var rects = blobCounter.GetObjectsRectangles();

            if (rects.Count() == 0)
            {
                return;
            }

            var result = new List<Bitmap>(rects.Count());

            foreach (Rectangle rect in rects)
            {
                lock (croplock)
                {
                    result.Add(data.Image.Crop(rect));
                }
            }

            OnDetectedBlobs(result);
        }
    }
}

using System;
using System.Drawing;
using TypingBot.Contracts;
using TypingBot.EventArgs;
using System.Collections.Generic;
using System.Threading.Tasks;
using TypingBot.Models;
using Nuance.OmniPage.CSDK.Objects;
using Nuance.OmniPage.CSDK.ArgTypes;

namespace TypingBot.Engines
{
    public class OmniPageEngine : IOCREngine
    {
        public OmniPageEngine()
        {
            Engine.Init(null, null, csdkpath: @"C:\Program Files (x86)\Nuance\OPCaptureSDK20\Bin");
        }

        public event EventHandler<RecognizedTextArgs> RecognizedText;
        private void OnRecognizedText(string text, bool isTextRecognized)
        {
            RecognizedText?.Invoke
            (
                this, 
                new RecognizedTextArgs
                {
                    Text = text,
                    IsTextRecognized = isTextRecognized
                }
            );
        }

        public void ProcessImage(Bitmap image)
        {
            Task.Run(() => doWork(new Params { Image = image })).Wait();

            OnRecognizedText(null, false);
        }

        public void ProcessImages(IEnumerable<Bitmap> images)
        {
            Parallel.ForEach(images, image => { doWork(new Params { Image = image }); });

            OnRecognizedText(null, false);
        }

        private void doWork(object o)
        {
            var p = o as Params;

            using (var scaledImage = scaleImage(p.Image, 3.0f))
            using (var settings = new SettingCollection())
            {
                settings.DefaultRecognitionModule = RECOGNITIONMODULE.RM_OMNIFONT_PLUS3W;
                settings.DefaultFillingMethod = FILLINGMETHOD.FM_OMNIFONT;
                settings.DefaultFilter = CHR_FILTER.FILTER_UPPERCASE;

                var info = new IMG_INFO
                {
                    DPI = new SIZE(
                        (int)scaledImage.HorizontalResolution,
                        (int)scaledImage.VerticalResolution),
                    Size = new SIZE(scaledImage.Width, scaledImage.Height),
                    BitsPerPixel = 24
                };

                using (var Page = new Page(scaledImage, info, settings))
                {
                    Page.Preprocess();

                    Page.Recognize();

                    var letters = Page[IMAGEINDEX.II_CURRENT].GetLetters();

                    if (letters.Length == 0)
                        return;

                    var text = string.Empty;

                    foreach (LETTER letter in letters)
                    {
                        if (!char.IsWhiteSpace(letter.code))
                            text += letter.code;
                    }

                    OnRecognizedText(text, true);
                }
            }
        }

        private Bitmap scaleImage(Bitmap img, float percentage)
        {
            int originalW = img.Width;
            int originalH = img.Height;

            int resizedW = (int)(originalW * percentage);
            int resizedH = (int)(originalH * percentage);

            Bitmap bmp = new Bitmap(resizedW, resizedH);

            using (var graphic = Graphics.FromImage(bmp))
            {
                graphic.DrawImage(img, 0, 0, resizedW, resizedH);
            }

            return bmp;
        }
    }
}

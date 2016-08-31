using System;
using System.Drawing;
using TypingBot.Contracts;
using TypingBot.EventArgs;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
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

        public async void ProcessImage(Bitmap image)
        {
            await Task.Run
            (() => doWork
            (
               new Params
               {
                   Image = image
               }
            ));

            OnRecognizedText(null, false);
        }

        public void ProcessImages(IEnumerable<Bitmap> images)
        {
            Parallel.ForEach(images,
            (image) =>
            {
                var p = new Params
                {
                    Image = image
                };
                doWork(p);
            });

            OnRecognizedText(null, false);
        }

        private void doWork(object o)
        {
            var p = o as Params;

            using (var scaledImage = ScaleImage(p.Image, 3.0f))
            using (var settings = new SettingCollection())
            {
                settings.DefaultRecognitionModule = RECOGNITIONMODULE.RM_OMNIFONT_PLUS3W;

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
                    {
                        OnRecognizedText(null, false);
                        return;
                    }

                    var text = string.Empty;

                    foreach (LETTER letter in letters)
                    {
                        text += letter.code;
                    }

                    //if (isLenMinFourAndLettersOnly(text))
                    OnRecognizedText(text, true);
                }
            }
        }

        [Obsolete]
        private bool isLenMinFourAndLettersOnly(string s)
        {
            return Regex.IsMatch(s, "^[A-Z]{4,}$");
        }

        private Bitmap ScaleImage(Bitmap img, float percentage)
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

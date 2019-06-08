using System;
using System.Drawing;
using System.Collections.Generic;
using TypingBot.Models;
using Nuance.OmniPage.CSDK.Objects;
using Nuance.OmniPage.CSDK.ArgTypes;
using System.Threading;

namespace TypingBot.OcrEngines
{
    public class OmniPageOcrEngine : IOCREngine
    {
        private readonly SettingCollection _settings;

        public OmniPageOcrEngine()
        {
            Engine.Init(null, null, csdkpath: @"C:\Program Files (x86)\Nuance\OPCaptureSDK20\Bin");

            _settings = new SettingCollection
            {
                DefaultRecognitionModule = RECOGNITIONMODULE.RM_AUTO,
                DefaultFillingMethod = FILLINGMETHOD.FM_DEFAULT,
                DefaultFilter = CHR_FILTER.FILTER_UPPERCASE
            };
        }

        public event EventHandler<RecognizedTextArgs> RecognizedText;
        private void OnRecognizedText(string text)
        {
            RecognizedText?.Invoke
            (
                this, 
                new RecognizedTextArgs
                {
                    Text = text                  
                }
            );
        }

        public void ProcessImage(Bitmap image)
        {
            ThreadPool.QueueUserWorkItem((o) => doWork(new Params { Image = image }));
        }

        public void ProcessImages(IEnumerable<Bitmap> images)
        {
            foreach (Bitmap image in images)
            {
                ThreadPool.QueueUserWorkItem((o) => doWork(new Params { Image = image }));
            }          
        }

        private void doWork(object o)
        {
            var data = o as Params;

            var info = new IMG_INFO
            {
                DPI = new SIZE
                (
                      (int)data.Image.HorizontalResolution,
                      (int)data.Image.VerticalResolution
                ),
                Size = new SIZE
                (
                    data.Image.Width, 
                    data.Image.Height
                ),
                BitsPerPixel = 24
            };

            using (var Page = new Page(data.Image, info, _settings))
            {
                Page.Preprocess();

                Page.Recognize();

                var letters = Page[IMAGEINDEX.II_CURRENT].GetLetters();

                if (letters.Length == 0)
                {
                    return;
                }

                var text = string.Empty;

                foreach (LETTER letter in letters)
                {
                    if (char.IsLetter(letter.code))
                        text += letter.code;
                }

                OnRecognizedText(text);
            }
        }        
    }
}

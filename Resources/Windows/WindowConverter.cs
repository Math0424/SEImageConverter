using ImageMagick;
using Microsoft.Win32;
using SEImageConverter.Resources.Enums;
using System;
using System.IO;

namespace SEImageConverter.Resources.Windows
{
    abstract class WindowConverter
    {
        public const string GenericImageFiles = "Image files|*.png;*.jpeg;*.jpg;*.bmp";

        public abstract void SelectFile();

        public abstract void SetVisible(bool visible);

        public abstract void Reset();

        public abstract void Convert();
        public abstract string CanConvert();

        public virtual void ConvertImage(MagickImage image, double scale, PixelInterpolateMethod pixelInterpolate, DitherMode dither, BitMode bitMode)
        {
            image.InterpolativeResize(Math.Max(1, (int)(image.Width * scale)), Math.Max(1, (int)(image.Height * scale)), pixelInterpolate);

            if (dither == DitherMode.Riemersma || dither == DitherMode.FloydSteinberg || dither == DitherMode.None)
            {
                image.Quantize(new QuantizeSettings()
                {
                    DitherMethod = (DitherMethod)dither,
                    Colors = 256,
                    ColorSpace = ColorSpace.sRGB,
                    TreeDepth = 100
                });
            }
            else
            {
                image.OrderedDither(dither.ToString());
            }
        }

        protected FileInfo ImageFileSelector(string filter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Browse Image Files",

                CheckFileExists = true,
                CheckPathExists = true,

                Filter = filter,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                return new FileInfo(openFileDialog.FileName);
            }
            else
            {
                return null;
            }
        }

    }
}

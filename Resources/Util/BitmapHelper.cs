using ImageMagick;
using SEImageConverter.Resources.Enums;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SEImageConverter.Resources.Util
{
    public class BitmapHelper
    {

        public static string MagickImageToText(BitMode b)
        {

            return null;
        }

        public static BitmapSource ToBitmapSource(MagickImage imageMagick)
        {
            MagickImage image = imageMagick;

            var mapping = "RGB";
            PixelFormat format = PixelFormats.Rgb24;

            try
            {
                if (imageMagick.ColorSpace == ColorSpace.CMYK && !image.HasAlpha)
                {
                    mapping = "CMYK";
                    format = PixelFormats.Cmyk32;
                }
                else
                {
                    if (imageMagick.ColorSpace != ColorSpace.sRGB)
                    {
                        image = (MagickImage)imageMagick.Clone();
                        image.ColorSpace = ColorSpace.sRGB;
                    }

                    if (image.HasAlpha)
                    {
                        mapping = "BGRA";
                        format = PixelFormats.Bgra32;
                    }
                }

                var step = format.BitsPerPixel / 8;
                var stride = imageMagick.Width * step;

                using (var pixels = image.GetPixelsUnsafe())
                {
                    var bytes = pixels.ToByteArray(mapping);
                    var dpi = imageMagick.Density.ChangeUnits(DensityUnit.PixelsPerInch);
                    return BitmapSource.Create(imageMagick.Width, imageMagick.Height, dpi.X, dpi.Y, format, null, bytes, stride);
                }
            }
            finally
            {
                if (!ReferenceEquals(imageMagick, image))
                    image.Dispose();
            }
        }



    }
}

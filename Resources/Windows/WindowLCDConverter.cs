using ImageMagick;
using SEImageConverter.Resources.Enums;

namespace SEImageConverter.Resources.Windows
{
    abstract class WindowLCDConverter : WindowConverter
    {
        public BitMode Mode;
        public int X, Y;

        public override void Reset()
        {
            X = 1;
            Y = 1;
        }

        public override void ConvertImage(MagickImage image, double scale, PixelInterpolateMethod pixelInterpolate, DitherMode dither, BitMode bitMode)
        {
            Reset();

            Mode = bitMode;

            int[] size = Utils.LCDSizeToNum((LCDSize)Converter.Instance.LCDSizeSelection.SelectedItem);

            if (Converter.Instance.currentWindow is ConverterLCDImage && LCDSize.SquareLCD == (LCDSize)Converter.Instance.LCDSizeSelection.SelectedItem)
            {
                Converter.Instance.MakeBlueprintOfLCD.Visibility = System.Windows.Visibility.Visible;
            } 
            else
            {
                Converter.Instance.MakeBlueprintOfLCD.Visibility = System.Windows.Visibility.Hidden;
            }

            image.InterpolativeResize(size[1] * Y, size[0] * X, pixelInterpolate);

            image.Depth = bitMode == BitMode.Bit3 ? 3 : 5;
            image.Settings.Compression = CompressionMethod.NoCompression;

            if (dither == DitherMode.Riemersma || dither == DitherMode.FloydSteinberg || dither == DitherMode.None)
            {
                image.Quantize(new QuantizeSettings() { Colors = bitMode == BitMode.Bit3 ? 8 : 32, DitherMethod = (DitherMethod)dither, ColorSpace = ColorSpace.sRGB, TreeDepth = 100 });
            }
            else
            {
                image.Quantize(new QuantizeSettings() { Colors = bitMode == BitMode.Bit3 ? 8 : 32, DitherMethod = DitherMethod.No, ColorSpace = ColorSpace.sRGB, TreeDepth = 100 });
                image.OrderedDither(dither.ToString());
            }

        }


    }
}

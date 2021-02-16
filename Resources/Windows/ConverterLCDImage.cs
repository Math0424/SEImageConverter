using ImageMagick;
using SEImageConverter.Resources.Enums;
using SEImageConverter.Resources.Util;
using System.IO;
using System.Text;
using System.Windows;

namespace SEImageConverter.Resources.Windows
{
    class ConverterLCDImage : WindowLCDConverter
    {

        public string[,] ConvertedImage;
        

        public override string CanConvert()
        {
            string message = "";
            if (Converter.MyPreviewImage == null)
            {
                message += "Image, ";
            }
            if (X == 0 || Y == 0)
            {
                message += "Non zero X or Y, ";
            }
            return message.Length == 0 ? null : message;
        }

        public override void Convert()
        {
            if (ConvertedImage != null)
                return;

            ConvertedImage = new string[X, Y];

            MagickImage image = Converter.MyPreviewImage;

            int width = (image.Width / Y);
            int height = (image.Height / X);

            using var c = image.GetPixelsUnsafe();

            for (int y = 0; y < Y; y++)
            {
                for (int x = 0; x < X; x++)
                {
                    StringBuilder frame = new StringBuilder();
                    for (int y1 = x * height; y1 < (x * height) + height; y1++)
                    {
                        for (int x1 = y * width; x1 < (y * width) + width; x1++)
                        {
                            var p = c.GetPixel(x1, y1).ToColor();
                            if (p.A < 100)
                            {
                                frame.Append(Utils.ColorToChar(Mode, 0, 0, 0));
                            }
                            else
                            {
                                byte[] b = p.ToByteArray();
                                frame.Append(Utils.ColorToChar(Mode, b[0], b[1], b[2]));
                            }
                        }
                        frame.Append("\n");
                    }

                    ConvertedImage[x, y] = frame.ToString();

                }
            }

            c.Dispose();

        }

        public void MakeBlueprint()
        {
            string error = CanConvert();
            if (error == null)
            {
                Convert();
                string font = Mode == BitMode.Bit3 ? "Monospace" : "Mono Color";
                BlueprintGenerator gen = new BlueprintGenerator((MagickImage)Converter.MyPreviewImage.Clone(), "LCDArray_"+Path.GetFileNameWithoutExtension(Converter.MyPreviewImage.FileName));
                for (int y = 0; y < Y; y++)
                {
                    for (int x = 0; x < X; x++)
                    {
                        gen.AddLCDBlock(font, ConvertedImage[x, y], y, X-x, 0, 0, -0.8, 0.5);
                    }
                }
                if (!gen.Create())
                {
                    MessageBox.Show("Failed to generate BP");
                } 
                else
                {
                    MessageBox.Show("Created BP!");
                }
                gen.Dispose();
            }
            else
            {
                MessageBox.Show("Error, cannot convert.\nMissing: " + error);
            }
        }

        public override void SelectFile()
        {
            FileInfo f = ImageFileSelector(GenericImageFiles);
            if (f != null)
            {
                Converter.MyPreviewImageSource = new MagickImage(f.FullName);
                Converter.Instance.FilePathTxt.Text = f.FullName;
                Converter.Instance.UpdateImage();
            }
        }

        public override void SetVisible(bool visible)
        {
            Utils.SetVisibility(Converter.Instance.ImagePreviewGrid, visible);
            Utils.SetVisibility(Converter.Instance.ImageOptionsGrid, visible);
            Utils.SetVisibility(Converter.Instance.ImageAdvancedGrid, visible);
            Utils.SetVisibility(Converter.Instance.FileSelectGrid, visible);
            Utils.SetVisibility(Converter.Instance.Image2LCDGrid, visible);

            Utils.SetVisibility(Converter.Instance.ConvertBtn, visible);

            Utils.SetVisibility(Converter.Instance.MakeBlueprintOfLCD, false);
        }

        public override void Reset()
        {
            ConvertedImage = null;
        }

    }
}

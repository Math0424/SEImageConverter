using ImageMagick;
using SEImageConverter.Resources.Enums;
using SEImageConverter.Resources.Util;
using System;
using System.IO;

namespace SEImageConverter.Resources.Windows
{
    class ConverterImageBlueprint : WindowConverter
    {
        public string BlueprintName;
        public GridSize GridSize;

        public override string CanConvert()
        {
            string blueprintPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/SpaceEngineers/Blueprints/local/" + BlueprintName;
            string message = "";
            if (Converter.MyPreviewImage == null)
            {
                message += "Image, ";
            }
            if (BlueprintName.Length == 0)
            {
                message += "Invalid name, ";
            }
            if (Directory.Exists(blueprintPath))
            {
                message += "Blueprint already exsists, ";
            }
            return message.Length == 0 ? null : message;
        }

        public override void Convert()
        {
            MagickImage image = (MagickImage)Converter.MyPreviewImage.Clone();
            BlueprintGenerator gen = new BlueprintGenerator(image, BlueprintName);
            gen.GridSize = GridSize;

            using var c = image.GetPixelsUnsafe();
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    var p = c.GetPixel(x, y).ToColor();
                    double[] b = ConvertRGB2SE(p.ToByteArray());
                    if (p.A > 100)
                    {
                        gen.AddCubeBlock(x, y, 0, b[0], b[1], b[2]);
                    }
                }
            }
            c.Dispose();

            gen.Create();
            gen.Dispose();
        }

        private double[] ConvertRGB2SE(byte[] colors)
        {
            double[] list = new double[3];
            double r = colors[0] / 255.0;
            double g = colors[1] / 255.0;
            double b = colors[2] / 255.0;
            double mx = Math.Max(r, Math.Max(g, b));
            double mn = Math.Min(r, Math.Min(g, b));
            double df = mx - mn;
            double h = 0;
            double s;
            double v;
            if (mx == mn)
            {
                h = 0;
            }
            else if (mx == r)
            {
                h = (60 * ((g - b) / df) + 360) % 360;
            }
            else if (mx == g)
            {
                h = (60 * ((b - r) / df) + 120) % 360;
            }
            else if (mx == b)
            {
                h = (60 * ((r - g) / df) + 240) % 360;
            }
            if (mx == 0)
            {
                s = 0;
            }
            else
            {
                s = (df / mx) * 100;
            }
            v = mx * 100;
            list[0] = (h / 360);
            list[1] = ((s - 100) / 100);
            list[2] = ((v - 25) / 100);
            return list;
        }

        public override void SelectFile()
        {
            FileInfo f = ImageFileSelector(GenericImageFiles);
            if (f != null)
            {
                Converter.MyPreviewImageSource = new MagickImage(f.FullName);
                Converter.Instance.FilePathTxt.Text = f.FullName;
                Converter.Instance.BlueprintNameInput.Text = Path.GetFileNameWithoutExtension(f.FullName);
                Converter.Instance.UpdateImage();
            }
        }

        public override void SetVisible(bool visible)
        {
            Utils.SetVisibility(Converter.Instance.ImagePreviewGrid, visible);
            Utils.SetVisibility(Converter.Instance.ImageOptionsGrid, visible);
            Utils.SetVisibility(Converter.Instance.FileSelectGrid, visible);
            Utils.SetVisibility(Converter.Instance.ImageResizeGrid, visible);
            Utils.SetVisibility(Converter.Instance.Image2BlueprintGrid, visible);

            Utils.SetVisibility(Converter.Instance.ConvertBtn, visible);

        }

        public override void Reset()
        {
            Converter.Instance.BlueprintNameInput.Text = "BlueprintNameHere";
        }

    }
}

using ImageMagick;
using SEImageConverter.Resources.Enums;
using System;
using System.IO;

namespace SEImageConverter.Resources.Windows
{
    class ConverterDDSImage : WindowConverter
    {

        public bool GenerateMask;
        private string MyImagePath;

        public override string CanConvert()
        {
            string message = "";
            if (Converter.MyPreviewImage == null)
            {
                message += "Image, ";
            }
            if (File.Exists(MyImagePath + "_c.dds"))
            {
                message += "Converted color image already exsists, ";
            }
            if (File.Exists(MyImagePath + "_a.dds"))
            {
                message += "Converted alpha image already exsists, ";
            }
            return message.Length == 0 ? null : message;
        }

        public override void Convert()
        {
            MagickImage image = Converter.MyPreviewImage;

            using (MagickImage color = new MagickImage(image.Clone()))
            {
                int height = color.Height + 2;
                height -= height % 2;
                int width = color.Width + 2;
                width -= width % 2;

                color.Format = MagickFormat.Png32;
                color.Extent(width, height, Gravity.Center, new MagickColor(0, 0, 0, 0));

                color.ColorAlpha(MagickColors.None);

                color.Format = MagickFormat.Dds;
                color.Settings.SetDefine(MagickFormat.Dds, "compression", "dxt5"); //dxt1, dxt5, none
                color.Settings.SetDefine(MagickFormat.Dds, "fast-mipmaps", "true");
                color.Settings.SetDefine(MagickFormat.Dds, "mipmaps", "10");
                color.Settings.SetDefine(MagickFormat.Dds, "cluster-fit", "true");
                color.Write(MyImagePath + ".dds");

                color.Dispose();
            }

            if (GenerateMask)
            {
                using (MagickImage alpha = new MagickImage(image.Clone()))
                {
                    int height = alpha.Height + 2;
                    height -= height % 2;
                    int width = alpha.Width + 2;
                    width -= width % 2;

                    alpha.Format = MagickFormat.Png;
                    alpha.Extent(width, height, Gravity.Center, new MagickColor(0, 0, 0, 0));

                    alpha.Grayscale();

                    var pixels = alpha.GetPixels();
                    foreach (Pixel p in pixels)
                    {
                        MagickColor c = (MagickColor)p.ToColor();
                        p.SetChannel(0, c.A);
                        p.SetChannel(1, 0);
                    }
                    pixels.Dispose();

                    alpha.Format = MagickFormat.Dds;
                    alpha.Settings.SetDefine(MagickFormat.Dds, "compression", "dxt5"); //dxt1, dxt5, none
                    alpha.Settings.SetDefine(MagickFormat.Dds, "fast-mipmaps", "true");
                    alpha.Settings.SetDefine(MagickFormat.Dds, "mipmaps", "10");
                    alpha.Settings.SetDefine(MagickFormat.Dds, "cluster-fit", "true");
                    alpha.Write(MyImagePath + "_mask.dds");

                    alpha.Dispose();
                }
            }
            
        }

        public override void SelectFile()
        {
            FileInfo f = ImageFileSelector(GenericImageFiles);
            if (f != null)
            {
                Converter.MyPreviewImageSource = new MagickImage(f.FullName);
                Converter.Instance.FilePathTxt.Text = f.FullName;

                MyImagePath = f.DirectoryName + "/" + Path.GetFileNameWithoutExtension(f.Name);

                Converter.Instance.UpdateImage();
            }
        }

        public override void SetVisible(bool visible)
        {
            Utils.SetVisibility(Converter.Instance.ImagePreviewGrid, visible);
            Utils.SetVisibility(Converter.Instance.ImageOptionsGrid, visible);
            Utils.SetVisibility(Converter.Instance.FileSelectGrid, visible);
            Utils.SetVisibility(Converter.Instance.ImageResizeGrid, visible);

            Utils.SetVisibility(Converter.Instance.Image2DDSGrid, visible);

            Utils.SetVisibility(Converter.Instance.ConvertBtn, visible);
        }

        public override void Reset()
        {

        }

    }
}

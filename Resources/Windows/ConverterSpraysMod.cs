using ImageMagick;
using SEImageConverter.Resources.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SEImageConverter.Resources.Windows
{
    class ConverterSpraysMod : WindowConverter
    {
        private readonly Random r = new Random();

        private string ModPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/SpaceEngineers/Mods/";
        private string FolderPath = "";

        private List<MySpray> Sprays = new List<MySpray>();

        private const string MaterialHead = @"<?xml version=""1.0""?>
<Definitions xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
    <TransparentMaterials>";
        private const string MaterialBody = @"
        <TransparentMaterial>
	      <Id>
            <TypeId>TransparentMaterialDefinition</TypeId>
            <SubtypeId>{Id}</SubtypeId>
          </Id>
          <AlphaSaturation>1</AlphaSaturation>
          <CanBeAffectedByOtherLights>false</CanBeAffectedByOtherLights>
          <SoftParticleDistanceScale>0</SoftParticleDistanceScale>
          <Texture>Textures\MySprays\{ColorMaskName}</Texture>
	      <Reflectivity>1</Reflectivity>
        </TransparentMaterial>";
        private const string MaterialFoot = @"
    </TransparentMaterials>
</Definitions>";

        private const string DecalHead = @"<?xml version=""1.0""?>
<Definitions xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <Decals>";
        //<DecalType>NormalColorExtMap</DecalType>
        private const string DecalBody = @"
    <Decal>
      <Id>
        <TypeId>DecalDefinition</TypeId>
        <SubtypeId>{Name}_spray</SubtypeId>
      </Id>
      <Source>{Name}</Source>
	  <Target>Default</Target>
      <Material>
        <NormalmapTexture>Textures\MySprays\{NormalMapName}</NormalmapTexture>
        <ColorMetalTexture>Textures\MySprays\{ColorMaskName}</ColorMetalTexture>
        <AlphamaskTexture>Textures\MySprays\{AlphaMaskName}</AlphamaskTexture>
      </Material>
      <MinSize>1</MinSize>
      <MaxSize>1</MaxSize>
      <Rotation>0</Rotation>
      <RenderDistance>5000.0</RenderDistance>
      <Depth>0.3</Depth>
    </Decal>";
        private const string DecalFoot = @"
  </Decals>
</Definitions>";

        public override string CanConvert()
        {
            string message = "";

            if (FolderPath.Length == 0 || !Directory.Exists(FolderPath))
            {
                message += "Folder not found, ";
            }
            if (Sprays.Count == 0)
            {
                message += "No images found, ";
            }
            if (Sprays.Count > 2000)
            {
                message += "Maximum image count of 2000, ";
            }
            if (Converter.Instance.SprayModNameInput.Text.Trim().Equals(""))
            {
                message += "Mod must have a valid name, ";
            }
            if (Directory.Exists(ModPath + "[SpraysAddon]" + Converter.Instance.SprayModNameInput.Text))
            {
                message += "Mod with that name already exists, ";
            }
            return message.Length == 0 ? null : message;
        }

        public override void Convert()
        {
            string MyModPath = "";
            string InputName = "";

            Converter.Instance.Dispatcher.Invoke(() =>
            {
                Converter.Instance.SprayModGenProgress.Maximum = Sprays.Count;
                Converter.Instance.SprayModGenProgress.Value = 1;
                MyModPath = ModPath + "[SpraysAddon]" + Converter.Instance.SprayModNameInput.Text;
                InputName = Converter.Instance.SprayModNameInput.Text;
            });

            //Setup basic file structure
            Directory.CreateDirectory(MyModPath);
            Directory.CreateDirectory(MyModPath + "/Data");
            Directory.CreateDirectory(MyModPath + "/Textures");
            Directory.CreateDirectory(MyModPath + "/Textures/MySprays");

            //generate the images
            string ImagePath = MyModPath + "/Textures/MySprays/";
            
            List<string> materials = new List<string>();
            List<string> images = new List<string>();
            Parallel.ForEach(Sprays, new ParallelOptions() { MaxDegreeOfParallelism = -1 }, spray =>
            {
                Converter.Instance.Dispatcher.Invoke(() => { Converter.Instance.SprayModGenProgress.Value += 1; });
                spray.GenerateSpray(ImagePath, out string body, out string material);
            
                materials.Add(material);
                images.Add(body);
            
                spray.Dispose();
            });
            
            //generate materials
            StreamWriter file = File.CreateText(MyModPath + "/Data/spray_materials.sbc");
            file.Write(MaterialHead);
            foreach (var x in materials)
                file.Write(x);
            file.Write(MaterialFoot);
            file.Close();
            
            file = File.CreateText(MyModPath + "/Data/spray_textures.sbc");
            file.Write(DecalHead);
            foreach (var x in images)
                file.Write(x);
            file.Write(DecalFoot);
            file.Close();
            
            StreamWriter program = File.CreateText(MyModPath + "/Sprays.txt");
            foreach (var spray in Sprays)
            {
                string cleanName = spray.Name.Trim().Replace('[', '-').Replace(']', '-');
                program.WriteLine($"[{cleanName}]");
                program.WriteLine($"ID={spray.Id}");
                program.WriteLine($"Group={(spray.DirName.Equals(FolderPath) ? InputName : Path.GetFileName(spray.DirName))}");
                program.WriteLine($"Flags={spray.Flags}");
                program.WriteLine();
            }
            program.Close();

            //make the thumbnail
            using (MagickImage thumb = new MagickImage(MagickColors.Transparent, 960, 540))
            {
                List<string> randomImages = new List<string>();
                for (int i = 0; i < 10; i++)
                {
                    int random = r.Next(Sprays.Count);
                    var spray = Sprays[random];
                    var path = spray.BasePath;
                    if (!((ExtraValues)spray.Flags).HasFlag(ExtraValues.Hidden) && randomImages.Contains(path))
                    {
                        for (int ii = 0; ii < 10; ii++)
                        {
                            random = r.Next(Sprays.Count);
                            spray = Sprays[random];
                            if (!((ExtraValues)spray.Flags).HasFlag(ExtraValues.Hidden) && !randomImages.Contains(path))
                                break;
                        }
                    }
                    randomImages.Add(path);
                }

                for(int i = 0; i < 10; i++)
                {
                    string path = randomImages[i % randomImages.Count];
                    using (MagickImage image = new MagickImage(path))
                    {
                        if (image.Width > image.Height)
                            image.Resize(new MagickGeometry("x270"));
                        else
                            image.Resize(new MagickGeometry("270x"));
                        image.Crop(192, 270, Gravity.Center);
                        thumb.Composite(image, Gravity.West, 192 * (i >= 5 ? i - 5 : i), 135 * (i >= 5 ? 1 : -1), CompositeOperator.Over);
                    }
                }

                MagickColor mcolor = new MagickColor((ushort)r.Next(ushort.MaxValue), (ushort)r.Next(ushort.MaxValue), (ushort)r.Next(ushort.MaxValue));

                var readSettings = new MagickReadSettings()
                {
                    Height = 540,
                    Width = 600
                };
                readSettings.SetDefine("gradient:direction", "east");
                using (MagickImage y = new MagickImage($"gradient:{mcolor.ToHexString()}-none", readSettings))
                {
                    y.Evaluate(Channels.Alpha, EvaluateOperator.Multiply, 0.75);
                    thumb.Composite(y, Gravity.West, 0, 0, CompositeOperator.Atop);
                }

                new Drawables()
                    .FontPointSize(Math.Min(120, 1500 / InputName.Length))
                    .Font("Inter_FXH", FontStyleType.Normal, FontWeight.Bold, FontStretch.ExtraExpanded)
                    .FillColor(mcolor)
                    .StrokeColor(MagickColors.Black)
                    .BorderColor(MagickColors.Black)
                    .Text(10, 100, $"{InputName}")
                    .TextAlignment(TextAlignment.Left)
                    .Draw(thumb);

                new Drawables()
                    .FontPointSize(40)
                    .Font("Inter_FXH", FontStyleType.Normal, FontWeight.Bold, FontStretch.ExtraExpanded)
                    .FillColor(mcolor)
                    .StrokeColor(MagickColors.Black)
                    .BorderColor(MagickColors.Black)
                    .Text(10, 160, $"Sprays mod generator")
                    .TextAlignment(TextAlignment.Left)
                    .Draw(thumb);

                new Drawables()
                    .FontPointSize(50)
                    .Font("Inter_FXH", FontStyleType.Normal, FontWeight.Bold, FontStretch.ExtraExpanded)
                    .FillColor(mcolor)
                    .StrokeColor(MagickColors.Black)
                    .BorderColor(MagickColors.Black)
                    .Text(10, 520, $"Over {(Sprays.Count / 10) * 10} images")
                    .TextAlignment(TextAlignment.Left)
                    .Draw(thumb);

                thumb.Write(MyModPath + "/THUMB.jpg");
            }
            


        }

        private bool SupportedImageFormat(string file)
        {
            if (!Path.HasExtension(file))
                return false;

            string val = Path.GetExtension(file).Substring(1).ToLower();
            val = char.ToUpper(val[0]) + val.Substring(1);
            MagickFormat format;
            if (!Enum.TryParse(val, out format))
            {
                return false;
            }
            switch(format)
            {
                case MagickFormat.Png:
                case MagickFormat.Jpeg:
                case MagickFormat.Gif:
                case MagickFormat.Bmp:
                case MagickFormat.Jpg:
                case MagickFormat.Tif:
                case MagickFormat.Dds: 
                    return true;
                default: return false;
            }
        }

        private void UpdateImages()
        {
            foreach (MySpray s in Sprays)
                s.Dispose();
            Sprays.Clear();
            Converter.Instance.SprayModNameInput.Text = Path.GetFileName(FolderPath);
            Converter.Instance.RunningGrayout.Visibility = System.Windows.Visibility.Visible;
            string baseGroup = Converter.Instance.SprayModNameInput.Text;

            Thread thread = new Thread(() =>
            {
                try
                {
                    Dictionary<string, int> myImages = new Dictionary<string, int>();
                    foreach (string s in Directory.GetFiles(FolderPath, "*.*", SearchOption.AllDirectories).Where(s => SupportedImageFormat(s)))
                    {
                        Sprays.Add(new MySpray(s));

                        string group = Path.GetDirectoryName(s).Equals(FolderPath) ? baseGroup : Path.GetFileName(Path.GetDirectoryName(s));
                        if (!myImages.ContainsKey(group))
                            myImages[group] = 0;
                        myImages[group] += 1;
                    }

                    Converter.Instance.Dispatcher.Invoke(() =>
                    {
                        Converter.Instance.RunningGrayout.Visibility = System.Windows.Visibility.Hidden;
                        Converter.Instance.SprayModGenFolders.Text = $"{myImages.Keys.Count} folders and {Sprays.Count} images found. \nIn-Game file structure shown below... \n\n";
                        foreach (string s in myImages.Keys)
                        {
                            Converter.Instance.SprayModGenFolders.Text += $"'{myImages[s]}' images in '{s}'\n";
                        }
                    });

                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show("ERROR: " + e.ToString());
                }
            });
            thread.IsBackground = true;
            thread.Start();

        }

        public override void SelectFile()
        {
            FolderBrowserEx.FolderBrowserDialog openFileDialog = new FolderBrowserEx.FolderBrowserDialog
            {
                Title = "Browse folders",
                AllowMultiSelect = false,
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                FolderPath = openFileDialog.SelectedFolder;
                Converter.Instance.FilePathTxt.Text = openFileDialog.SelectedFolder;

                UpdateImages();
            }
        }

        public override void SetVisible(bool visible)
        {
            Utils.SetVisibility(Converter.Instance.FileSelectGrid, visible);
            Utils.SetVisibility(Converter.Instance.SpraysModGenGrid, visible);
            Utils.SetVisibility(Converter.Instance.ConvertBtn, visible);
        }

        public override void Reset()
        {
            foreach (MySpray s in Sprays)
            {
                s.Dispose();
            }
            Sprays.Clear();
            Sprays = new List<MySpray>();
            FolderPath = "";

            Converter.Instance.Dispatcher.Invoke(() =>
            {
                Converter.Instance.SprayModNameInput.Text = "MyFirstSpraysMod";
                Converter.Instance.SprayModGenFolders.Text = "Folders here";
                Converter.Instance.SprayModGenProgress.Value = 0;
            });
        }

        private class MySpray : IDisposable
        {
            public string Name;
            public string Id;
            public uint Flags { get; protected set; }
            public Shine Shine;

            public string DirName, BasePath;
            public int FrameCount { get; protected set; }

            public MagickImage[] Images;

            public MySpray(string path)
            {
                BasePath = path;

                uint flagOut;
                Name = TrimName(path, out flagOut, out Shine);
                Flags = flagOut;

                Id = GetFileID(path).ToString();
                DirName = Path.GetDirectoryName(path);
            }

            private void WriteImage(string path, int i, out string body)
            {
                using (MagickImage newImage = (MagickImage)Images[i].Clone())
                {
                    newImage.Format = MagickFormat.Dds;
                    newImage.Settings.SetDefine(MagickFormat.Dds, "compression", "dxt5"); //dxt1, dxt5, none
                    newImage.Settings.SetDefine(MagickFormat.Dds, "fast-mipmaps", "true"); //quickly do nothing
                    newImage.Settings.SetDefine(MagickFormat.Dds, "mipmaps", "0");
                    newImage.Settings.SetDefine(MagickFormat.Dds, "cluster-fit", "true");

                    body = DecalBody
                           .Replace("{NormalMapName}", Shine != Shine.NONE ? $"{Id}_{i}_n.dds" : "NoNormalMap")
                           .Replace("{AlphaMaskName}", $"{Id}_{i}_a.dds")
                           .Replace("{ColorMaskName}", $"{Id}_{i}_c.dds");

                    if (((ExtraValues)Flags).HasFlag(ExtraValues.Animated))
                        body = body.Replace("{Name}", $"{Id}_{i}");
                    else
                        body = body.Replace("{Name}", $"{Id}");

                    using var alphaImage = newImage.Clone();
                    alphaImage.Alpha(AlphaOption.Extract);

                    newImage.Composite(alphaImage, CompositeOperator.Multiply, Channels.RGB);
                    newImage.Write(Path.Combine(path, $"{Id}_{i}_c.dds"));

                    alphaImage.Write(Path.Combine(path, $"{Id}_{i}_a.dds"));
                }

                if (Shine != Shine.NONE)
                {
                    using (MagickImage shiny = (MagickImage)Images[i].Clone())
                    {
                        shiny.Evaluate(Channels.Red, EvaluateOperator.Set, 32767);
                        shiny.Evaluate(Channels.Green, EvaluateOperator.Set, 32767);
                        shiny.Evaluate(Channels.Blue, EvaluateOperator.Set, 65535);
                        shiny.Evaluate(Channels.Alpha, EvaluateOperator.Subtract, (ushort)Shine);

                        shiny.Format = MagickFormat.Dds;
                        shiny.Settings.SetDefine(MagickFormat.Dds, "compression", "dxt5"); //dxt1, dxt5, none
                        shiny.Settings.SetDefine(MagickFormat.Dds, "fast-mipmaps", "true"); //quickly do nothing
                        shiny.Settings.SetDefine(MagickFormat.Dds, "mipmaps", "0");
                        shiny.Settings.SetDefine(MagickFormat.Dds, "cluster-fit", "true");
                        shiny.Write(Path.Combine(path, $"{Id}_{i}_n.dds"));
                    }
                }
            }

            public void GenerateSpray(string path, out string decalBody, out string materialName)
            {
                if (Path.GetExtension(BasePath).ToLower().Equals(".gif"))
                {
                    MagickImageCollection collection = new MagickImageCollection(BasePath);
                    Images = new MagickImage[collection.Count];
                    for (int i = 0; i < Images.Length; i++)
                        Images[i] = collection[i] as MagickImage;

                    int speed = Images[0].AnimationDelay;
                    if (speed >= 60)
                        Flags |= (int)FPS.Fps_1 << 19;
                    else if (speed >= 12)
                        Flags |= (int)FPS.Fps_5 << 19;
                    else if (speed >= 4)
                        Flags |= (int)FPS.Fps_15 << 19;
                    else if (speed >= 3)
                        Flags |= (int)FPS.Fps_20 << 19;
                    else if (speed >= 2)
                        Flags |= (int)FPS.Fps_30 << 19;

                    if (((ExtraValues)Flags).HasFlag(ExtraValues.Animated) && Images.Length == 1)
                        Flags &= (int)ExtraValues.Animated;

                    if (((ExtraValues)Flags).HasFlag(ExtraValues.Animated))
                    {
                        FrameCount = Math.Min(Images.Length, 255);
                        Flags |= (uint)FrameCount << 24;
                    }
                }
                else
                {
                    Images = new MagickImage[1];
                    Images[0] = new MagickImage(BasePath);
                }

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < Images.Length; i++)
                {
                    MagickImage image = Images[i];

                    int length = Math.Max(image.Height, image.Width) + 2;
                    length -= (length % 4);

                    image.Format = MagickFormat.Png32;
                    image.Extent(length, length, Gravity.Center, new MagickColor(0, 0, 0, 0));
                    //image.ColorAlpha(MagickColors.None);

                    WriteImage(path, i, out string output);
                    builder.Append(output);
                }
                decalBody = builder.ToString();
                materialName = MaterialBody
                    .Replace("{ColorMaskName}", $"{Id}_{0}_c.dds")
                    .Replace("{Id}", $"{Id}_b");
            }

            public void Dispose()
            {
                if (Images != null)
                    foreach (MagickImage image in Images)
                        image.Dispose();
                Images = null;
            }
        }

        private static Guid GetFileID(string path)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(path))
                {
                    byte[] a = Encoding.ASCII.GetBytes(Path.GetFileNameWithoutExtension(path));
                    byte[] b = md5.ComputeHash(stream);

                    byte[] c = new byte[a.Length + b.Length];
                    Array.Copy(a, 0, c, 0, a.Length);
                    Array.Copy(b, 0, c, a.Length, b.Length);

                    return new Guid(md5.ComputeHash(c));
                }
            }
        }

        private static string TrimName(string s, out uint flags, out Shine shine)
        {
            ExtraValues myFlags = ExtraValues.None;
            if (s.Contains("_HIDDEN")) myFlags |= ExtraValues.Hidden;
            if (s.Contains("_ADMIN")) myFlags |= ExtraValues.AdminOnly;

            if (Path.GetExtension(s).ToLower().Equals(".gif"))
                myFlags |= ExtraValues.Animated;
            flags = (uint)myFlags;

            shine = Shine.NONE;
            if (s.Contains("_SHINYMAX")) shine = Shine.SHINYMAX;
            if (s.Contains("_SHINYMED")) shine = Shine.SHINYMED;
            if (s.Contains("_SHINYLITE")) shine = Shine.SHINYLITE;

            return Path.GetFileNameWithoutExtension(s)
                .Replace("_HIDDEN", "")
                .Replace("_ADMIN", "")
                .Replace("_SHINYMAX", "")
                .Replace("_SHINYMED", "")
                .Replace("_SHINYLITE", "")
                .Replace("§", "-");
        }

        private enum ExtraValues
        {
            None = 0,
            Hidden = 1,
            AdminOnly = 2,
            Animated = 4,
        }

        private enum FPS
        {
            Fps_30 = 1,
            Fps_20 = 2,
            Fps_15 = 4,
            Fps_5 = 8,
            Fps_1 = 16,
        }

        private enum Shine
        {
            NONE = 0,
            SHINYMAX = 100,
            SHINYMED = 5000,
            SHINYLITE = 13000,
        }

    }
}

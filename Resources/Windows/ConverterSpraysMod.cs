using ImageMagick;
using Microsoft.WindowsAPICodePack.Dialogs;
using SEImageConverter.Resources.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SEImageConverter.Resources.Windows
{
    class ConverterSpraysMod : WindowConverter
    {
        private readonly Random r = new Random();

        private string ModPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/SpaceEngineers/Mods/";
        private string FolderPath = "";

        private List<MySpray> Sprays = new List<MySpray>();

        private const int GlobalSizes = 3;
        private const string MaterialDef = @"<?xml version=""1.0""?>
<Definitions xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <TransparentMaterials>
    <TransparentMaterial>
	  <Id>
        <TypeId>TransparentMaterialDefinition</TypeId>
        <SubtypeId>{name}_b</SubtypeId>
      </Id>
      <AlphaSaturation>1</AlphaSaturation>
      <CanBeAffectedByOtherLights>false</CanBeAffectedByOtherLights>
      <SoftParticleDistanceScale>0</SoftParticleDistanceScale>
      <Texture>Textures\MySprays\{name}_c.dds</Texture>
	  <Reflectivity>1</Reflectivity>
    </TransparentMaterial>
  </TransparentMaterials>
</Definitions>";

        private const string DecalHead = @"<?xml version=""1.0""?>
<Definitions xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <Decals>";
        private const string DecalBody = @"
    <Decal>
      <Id>
        <TypeId>DecalDefinition</TypeId>
        <SubtypeId>{name}_{size}{extra}_spray</SubtypeId>
      </Id>
      <Source>{name}_{size}{extra}</Source>
	  <Target>Default</Target>
      <Material>
	    <DecalType>NormalColorExtMap</DecalType>
        <NormalmapTexture>Textures\MySprays\NoNormalMapHere</NormalmapTexture>
        <ColorMetalTexture>Textures\MySprays\{name}{extra}_c.dds</ColorMetalTexture>
        <AlphamaskTexture>Textures\MySprays\{name}{extra}_a.dds</AlphamaskTexture>
      </Material>
      <MinSize>{size}</MinSize>
      <MaxSize>{size}</MaxSize>
      <Rotation>0</Rotation>
      <RenderDistance>5000.0</RenderDistance>
      <Depth>0.3</Depth>
    </Decal>";
        private const string DecalFoot = @"
  </Decals>
</Definitions>";

        private const string ProgramHead = @"using Sandbox.ModAPI;
using VRage.Game;
using VRage.Game.Components;

namespace MySprayMod
{
    //Generated using SpraysModGenerator created by Math0424
    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    class MySprayMod : MySessionComponentBase
    {
        public override void Init(MyObjectBuilder_SessionComponent sessionComponent)
        {
";
        private const string ProgramBody = @"MyAPIGateway.Utilities.SendModMessage(35872945762398745, MyAPIGateway.Utilities.SerializeToBinary(@""{inner}""));";
        private const string ProgramInner = @"{guid}§{group}§{name}§{flags}§§
";
        private const string ProgramFoot = @"        
        }
    }
}";

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
                Converter.Instance.SprayModGenProgress.Maximum = ((Sprays.Count - 1) * 2) + 4;
                Converter.Instance.SprayModGenProgress.Value = 1;
                MyModPath = ModPath + "[SpraysAddon]" + Converter.Instance.SprayModNameInput.Text;
                InputName = Converter.Instance.SprayModNameInput.Text;
            });

            //Setup basic file structure
            Directory.CreateDirectory(MyModPath);
            Directory.CreateDirectory(MyModPath + "/Data");
            Directory.CreateDirectory(MyModPath + "/Data/Scripts");
            Directory.CreateDirectory(MyModPath + "/Textures");
            Directory.CreateDirectory(MyModPath + "/Textures/MySprays");

            StreamWriter program = File.CreateText(MyModPath + "/Data/Scripts/MySprayMod.cs");
            program.Write(ProgramHead);
            StringBuilder inner = new StringBuilder();
            foreach (var spray in Sprays)
            {
                inner.Append(ProgramInner.Replace("{guid}", spray.Id)
                    .Replace("{name}", spray.Name)
                    .Replace("{group}", spray.DirName.Equals(FolderPath) ? InputName : Path.GetFileName(spray.DirName))
                    .Replace("{flags}", spray.Flags.ToString()));
            }
            program.Write(ProgramBody.Replace("{inner}", inner.ToString()));
            program.Write(ProgramFoot);
            program.Close();

            //generate materials
            Converter.Instance.Dispatcher.Invoke(() => { Converter.Instance.SprayModGenProgress.Value += 1; });
            foreach (var spray in Sprays)
            {
                StreamWriter file = File.CreateText(MyModPath + "/Data/" + spray.Name + "_mat.sbc");
                file.Write(MaterialDef.Replace("{name}", spray.Id));
                file.Close();
            }

            //generate decals and corresponding sizes
            Converter.Instance.Dispatcher.Invoke(() => { Converter.Instance.SprayModGenProgress.Value += 1; });
            foreach (var spray in Sprays)
            {
                StreamWriter file = File.CreateText(MyModPath + "/Data/" + spray.Name + "_decal.sbc");
                file.Write(DecalHead);
                for (int i = 1; i <= GlobalSizes; i++)
                {
                    file.Write(DecalBody
                        .Replace("{name}", spray.Id)
                        .Replace("NoNormalMapHere", spray.Shine != Shine.NONE ? spray.Id + "_n.dds" : "NoNormalMapHere")
                        .Replace("{size}", i.ToString())
                        .Replace("{extra}", "")
                        .Replace("Textures", $"C:\\Users\\Math0424\\AppData\\Roaming\\SpaceEngineers\\Mods\\[SpraysAddon]{InputName}\\Textures")
                        );
                }

                if (((ExtraValues)spray.Flags).HasFlag(ExtraValues.Animated))
                {
                    for (int f = 0; f < spray.FrameCount; f++)
                    {
                        for (int i = 1; i <= GlobalSizes; i++)
                        {
                            file.Write(DecalBody
                                .Replace("{name}", spray.Id)
                                .Replace("NoNormalMapHere", spray.Shine != Shine.NONE ? spray.Id + "_n.dds" : "NoNormalMapHere")
                                .Replace("{size}", i.ToString())
                                .Replace("{extra}", "_" + f)
                                .Replace("Textures", $"C:\\Users\\Math0424\\AppData\\Roaming\\SpaceEngineers\\Mods\\[SpraysAddon]{InputName}\\Textures")
                                );
                        }
                    }
                }

                file.Write(DecalFoot);
                file.Close();
            }

            //make the thumbnail
            Converter.Instance.Dispatcher.Invoke(() => { Converter.Instance.SprayModGenProgress.Value += 1; });
            using (MagickImage thumb = new MagickImage(MagickColors.Transparent, 960, 540))
            {
                //backgroudn square
                new Drawables()
                .Rectangle(0, 85, 960, 355)
                .FillColor(new MagickColor(55555, 55555, 55555))
                .Draw(thumb);

                int[] prev = new int[9];
                for (int i = 0; i < prev.Length; i++)
                {
                    int random = r.Next(Sprays.Count);
                    var spray = Sprays[random];

                    if (!((ExtraValues)spray.Flags).HasFlag(ExtraValues.Hidden) && prev.Contains(random))
                    {
                        for (int ii = 0; ii < prev.Length; ii++)
                        {
                            random = r.Next(Sprays.Count);
                            spray = Sprays[random];
                            if (!((ExtraValues)spray.Flags).HasFlag(ExtraValues.Hidden) && !prev.Contains(random))
                                break;
                        }
                    }
                    prev[i] = random;

                    using (MagickImage overlay = (MagickImage)spray.Images[0].Clone())
                    {
                        overlay.Trim();
                        overlay.Resize(150, 150);
                        overlay.Colorize(MagickColors.Black, new Percentage(1));
                        thumb.Composite(overlay, (i * 100) + 5, 145 + (i % 2 == 0 ? 50 : -50), CompositeOperator.Over);
                    }
                }

                thumb.ColorAlpha(new MagickColor((ushort)r.Next(ushort.MaxValue), (ushort)r.Next(ushort.MaxValue), (ushort)r.Next(ushort.MaxValue)));

                new Drawables()
                .FontPointSize(60)
                .Font("Inter_FXH", FontStyleType.Normal, FontWeight.Bold, FontStretch.ExtraExpanded)
                .StrokeColor(MagickColors.Black)
                .FillColor(MagickColors.Black)
                .Text(480, 60, InputName)
                .TextAlignment(TextAlignment.Center)
                .Draw(thumb);

                new Drawables()
                .FontPointSize(60)
                .Font("Inter_FXH", FontStyleType.Normal, FontWeight.Bold, FontStretch.ExtraExpanded)
                .StrokeColor(MagickColors.Black)
                .FillColor(MagickColors.Black)
                .Text(480, 420, "Made using\nsprays mod generator")
                .TextAlignment(TextAlignment.Center)
                .Draw(thumb);

                new Drawables()
                .FontPointSize(20)
                .Font("Inter_FXH", FontStyleType.Normal, FontWeight.Normal, FontStretch.ExtraExpanded)
                .StrokeColor(MagickColors.Black)
                .FillColor(MagickColors.Black)
                .Text(950, 530, $"Over {(Sprays.Count / 10) * 10} images")
                .TextAlignment(TextAlignment.Right)
                .Draw(thumb);

                thumb.Write(MyModPath + "/THUMB.jpg");
            }

            //generate the images
            string ImagePath = MyModPath + "/Textures/MySprays/";
            Parallel.ForEach(Sprays, new ParallelOptions() { MaxDegreeOfParallelism = 10 }, spray =>
            {
                string imagePath = ImagePath + spray.Id;

                Converter.Instance.Dispatcher.Invoke(() => { Converter.Instance.SprayModGenProgress.Value += 1; });

                SaveImageToFile(spray.Images[0], imagePath, spray.Shine);
                if (((ExtraValues)spray.Flags).HasFlag(ExtraValues.Animated))
                {
                    Converter.Instance.Dispatcher.Invoke(() => { Converter.Instance.SprayModGenProgress.Maximum += spray.FrameCount; });
                    for (int i = 0; i < spray.FrameCount; i++)
                    {
                        SaveImageToFile(spray.Images[i], imagePath + "_" + i, spray.Shine);
                        Converter.Instance.Dispatcher.Invoke(() => { Converter.Instance.SprayModGenProgress.Value += 1; });
                    }
                }

                Converter.Instance.Dispatcher.Invoke(() => { Converter.Instance.SprayModGenProgress.Value += 1; });

                spray.Dispose();

            });

        }

        private void SaveImageToFile(MagickImage image, string name, Shine shine)
        {

            using (MagickImage newImage = (MagickImage)image.Clone())
            {
                newImage.Format = MagickFormat.Dds;
                newImage.Settings.SetDefine(MagickFormat.Dds, "compression", "dxt5"); //dxt1, dxt5, none
                newImage.Settings.SetDefine(MagickFormat.Dds, "fast-mipmaps", "true"); //quickly do nothing
                newImage.Settings.SetDefine(MagickFormat.Dds, "mipmaps", "0");
                newImage.Settings.SetDefine(MagickFormat.Dds, "cluster-fit", "true");

                newImage.Write(name + "_c.dds");

                newImage.Grayscale();
                foreach (Pixel p in newImage.GetPixels())
                {
                    MagickColor c = (MagickColor)p.ToColor();
                    p.SetChannel(0, c.A);
                    p.SetChannel(1, 0);
                }
                newImage.Write(name + "_a.dds");
            }

            if (shine != Shine.NONE)
            {

                using (MagickImage shiny = (MagickImage)image.Clone())
                {
                    shiny.Evaluate(Channels.Red, EvaluateOperator.Set, 32767);
                    shiny.Evaluate(Channels.Green, EvaluateOperator.Set, 32767);
                    shiny.Evaluate(Channels.Blue, EvaluateOperator.Set, 65535);
                    shiny.Evaluate(Channels.Alpha, EvaluateOperator.Subtract, (ushort)shine);

                    shiny.Format = MagickFormat.Dds;
                    shiny.Settings.SetDefine(MagickFormat.Dds, "compression", "dxt5"); //dxt1, dxt5, none
                    shiny.Settings.SetDefine(MagickFormat.Dds, "fast-mipmaps", "true"); //quickly do nothing
                    shiny.Settings.SetDefine(MagickFormat.Dds, "mipmaps", "0");
                    shiny.Settings.SetDefine(MagickFormat.Dds, "cluster-fit", "true");
                    shiny.Write(name + "_n.dds");
                }
            }
        }

        private void UpdateImages()
        {
            foreach (MySpray s in Sprays)
            {
                s.Dispose();
            }
            Sprays.Clear();
            Converter.Instance.SprayModNameInput.Text = Path.GetFileName(FolderPath);
            Converter.Instance.RunningGrayout.Visibility = System.Windows.Visibility.Visible;
            string baseGroup = Converter.Instance.SprayModNameInput.Text;

            Thread thread = new Thread(() =>
            {
                try
                {
                    Dictionary<string, int> myImages = new Dictionary<string, int>();
                    foreach (string s in Directory.GetFiles(FolderPath, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".png") || s.EndsWith(".gif")))
                    {
                        Sprays.Add(new MySpray(s));

                        string group = Path.GetDirectoryName(s).Equals(FolderPath) ? baseGroup : Path.GetFileName(Path.GetDirectoryName(s));
                        if (!myImages.ContainsKey(group))
                        {
                            myImages[group] = 0;
                        }
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
            CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog
            {
                Title = "Browse folders",
                Multiselect = false,
                EnsurePathExists = true,
                EnsureValidNames = true,
                IsFolderPicker = true,
            };

            if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                FolderPath = openFileDialog.FileName;
                Converter.Instance.FilePathTxt.Text = openFileDialog.FileName;

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

                Images = GetBaseImage(path, out int speed);

                if (((ExtraValues)Flags).HasFlag(ExtraValues.Animated) && Images.Length == 1)
                {
                    Flags &= (int)ExtraValues.Animated;
                }

                if (((ExtraValues)Flags).HasFlag(ExtraValues.Animated))
                {
                    FrameCount = Math.Min(Images.Length, 255);
                    Flags |= (uint)FrameCount << 24;

                    if (speed >= 60)
                    {
                        Flags |= (int)FPS.Fps_1 << 19;
                    }
                    else if (speed >= 20)
                    {
                        Flags |= (int)FPS.Fps_5 << 19;
                    }
                    else if (speed >= 6)
                    {
                        Flags |= (int)FPS.Fps_15 << 19;
                    }
                    else if (speed >= 3)
                    {
                        Flags |= (int)FPS.Fps_30 << 19;
                    }
                    else if (speed >= 0)
                    {
                        Flags |= (int)FPS.Fps_60 << 19;
                    }
                }

            }

            public void Dispose()
            {
                if (Images != null)
                {
                    foreach (MagickImage image in Images)
                    {
                        image.Dispose();
                    }
                }
                Images = null;
            }
        }

        private static MagickImage[] GetBaseImage(string s, out int speed)
        {
            MagickImage[] images;

            if (Path.GetExtension(s).ToLower().Equals(".gif"))
            {
                MagickImageCollection collection = new MagickImageCollection(s);
                images = new MagickImage[collection.Count];
                for (int i = 0; i < images.Length; i++)
                {
                    images[i] = collection[i] as MagickImage;
                }

                using (MagickImage temp = new MagickImage(s))
                {
                    speed = temp.AnimationDelay;
                }
            }
            else
            {
                images = new MagickImage[1];
                images[0] = new MagickImage(s);
                speed = 0;
            }

            for (int i = 0; i < images.Length; i++)
            {
                MagickImage image = images[i];

                int length = Math.Max(image.Height, image.Width) + 2;
                length -= (length % 4);

                image.Format = MagickFormat.Png32;
                image.Extent(length, length, Gravity.Center, new MagickColor(0, 0, 0, 0));

                image.ColorAlpha(MagickColors.None);
            }

            return images;
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
            {
                myFlags |= ExtraValues.Animated;
            }
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
            Fps_60 = 1,
            Fps_30 = 2,
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

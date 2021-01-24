using ImageMagick;
using SEImageConverter.Resources.Enums;
using System;
using System.IO;

namespace SEImageConverter.Resources.Windows
{
    class ConverterImageBlueprint : WindowConverter
    {
        private const string Header = @"<?xml version=""1.0""?>
<Definitions xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <ShipBlueprints>
    <ShipBlueprint xsi:type=""MyObjectBuilder_ShipBlueprintDefinition"">
      <Id Type=""MyObjectBuilder_ShipBlueprintDefinition"" Subtype=""{name}""/>
      <DisplayName>Math0424</DisplayName>
      <CubeGrids>
        <CubeGrid>
          <SubtypeName/>
          <EntityId>118773863687514751</EntityId>
          <PersistentFlags>CastShadows InScene</PersistentFlags>
          <PositionAndOrientation>
            <Position x =""0"" y=""0"" z=""0"" />
            <Forward x=""0"" y=""0"" z=""0"" />
            <Up x=""0"" y=""0"" z=""0"" />
            <Orientation >
              <X>0</X>
              <Y>0</Y>
              <Z>0</Z>
              <W>0</W>
            </Orientation>
          </PositionAndOrientation>
          <GridSizeEnum>{size}</GridSizeEnum>
          <CubeBlocks>";

        private const string Footer = @"
            </CubeBlocks>
          <DisplayName>{name}</DisplayName>
          <DestructibleBlocks>true</DestructibleBlocks>
          <IsRespawnGrid>false</IsRespawnGrid>
          <LocalCoordSys>0</LocalCoordSys>
          <TargetingTargets />
        </CubeGrid>
      </CubeGrids>
      <WorkshopId>0</WorkshopId>
      <OwnerSteamId>76561198161316860</OwnerSteamId>
      <Points>0</Points>
    </ShipBlueprint>
  </ShipBlueprints>
</Definitions>";

        private const string Cube = @"
            <MyObjectBuilder_CubeBlock xsi:type=""MyObjectBuilder_CubeBlock"">
              <SubtypeName>{size}BlockArmorBlock</SubtypeName>
              <Min x = ""{x}"" y=""{y}"" z=""0"" />
              <BlockOrientation Forward = ""Down"" Up=""Forward"" />
              <SkinSubtypeId>Clean_Armor</SkinSubtypeId>
              <ColorMaskHSV x = ""{h}"" y=""{s}"" z=""{v}"" />
            </MyObjectBuilder_CubeBlock>";

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
            MagickImage image = Converter.MyPreviewImage;
            string blueprintPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/SpaceEngineers/Blueprints/local/" + BlueprintName;

            Directory.CreateDirectory(blueprintPath);
            image.Write(blueprintPath + "/thumb.png");
            StreamWriter text = File.CreateText(blueprintPath + "/bp.sbc");
            text.Write(Header.Replace("{size}", GridSize.ToString()).Replace("{name}", BlueprintName));

            using var c = image.GetPixelsUnsafe();
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    var p = c.GetPixel(x, y).ToColor();
                    double[] b = ConvertRGB2SE(p.ToByteArray());
                    if (p.A > 100)
                    {
                        text.Write(Cube
                            .Replace("{size}", GridSize.ToString())
                            .Replace("{x}", x.ToString())
                            .Replace("{y}", "-"+y.ToString())
                            .Replace("{h}", b[0].ToString())
                            .Replace("{s}", b[1].ToString())
                            .Replace("{v}", b[2].ToString())
                            .Replace(",", "."));
                    }
                }
            }
            c.Dispose();

            text.Write(Footer.Replace("{name}", BlueprintName));
            text.Close();
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

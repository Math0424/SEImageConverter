using ImageMagick;
using SEImageConverter.Resources.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SEImageConverter.Resources.Util
{
    class BlueprintGenerator : IDisposable
    {
        public GridSize GridSize;

        private StringBuilder builder;
        private MagickImage image;
        private string name;

        public BlueprintGenerator(MagickImage image, string name)
        {
            this.name = name;
            this.image = image;
            builder = new StringBuilder();
            GridSize = GridSize.Large;
        }

        public void AddCubeBlock(int x, int y, int z, double h, double s, double v)
        {
            builder.Append(
            $@"
            <MyObjectBuilder_CubeBlock xsi:type=""MyObjectBuilder_CubeBlock"">
              <SubtypeName>{GridSize}BlockArmorBlock</SubtypeName>
              <Min x=""{x}"" y=""{y}"" z=""{z}"" />
              <BlockOrientation Forward=""Down"" Up=""Forward"" />
              <SkinSubtypeId>Clean_Armor</SkinSubtypeId>
              <ColorMaskHSV x=""{h}"" y=""{s}"" z=""{v}"" />
            </MyObjectBuilder_CubeBlock>".Replace(',', '.')
            );
        }

        public void AddLCDBlock(string font, string data, int x, int y, int z, double h, double s, double v)
        {
            //Mono Color
            //Monospace

            builder.Append(
            $@"
            <MyObjectBuilder_CubeBlock xsi:type=""MyObjectBuilder_TextPanel"">
              <SubtypeName>LargeLCDPanel</SubtypeName>
              <Min x=""{x}"" y=""{y}"" z=""{z}"" />
              <ColorMaskHSV x=""{h}"" y=""{s}"" z=""{v}"" />
              <SkinSubtypeId>Clean_Armor</SkinSubtypeId>
              <Owner>144115188075855895</Owner>
              <BuiltBy>144115188075855895</BuiltBy>
              <ShareMode>Faction</ShareMode>
              <CustomName>LCD Panel</CustomName>
              <ShowOnHUD>false</ShowOnHUD>
              <ShowInTerminal>false</ShowInTerminal>
              <ShowInToolbarConfig>false</ShowInToolbarConfig>
              <ShowInInventory>false</ShowInInventory>
              <Enabled>true</Enabled>
              <Description />
              <Title>Title</Title>
              <AccessFlag>READ_AND_WRITE_FACTION</AccessFlag>
              <ChangeInterval>0</ChangeInterval>
              <Font Type=""MyObjectBuilder_FontDefinition"" Subtype=""{font}"" />
              <FontSize>0.1</FontSize>
              <PublicDescription>{data}</PublicDescription>
              <PublicTitle>Public title</PublicTitle>
              <ShowText>NONE</ShowText>
              <FontColor>
                <PackedValue>4294967295</PackedValue>
                <X>255</X>
                <Y>255</Y>
                <Z>255</Z>
                <R>255</R>
                <G>255</G>
                <B>255</B>
                <A>255</A>
              </FontColor>
              <BackgroundColor>
                <PackedValue>4278190080</PackedValue>
                <X>0</X>
                <Y>0</Y>
                <Z>0</Z>
                <R>0</R>
                <G>0</G>
                <B>0</B>
                <A>255</A>
              </BackgroundColor>
              <CurrentShownTexture>0</CurrentShownTexture>
              <Alignment>Align_Center</Alignment>
              <ContentType>TEXT_AND_IMAGE</ContentType>
              <SelectedScript />
              <Version>1</Version>
              <ScriptBackgroundColor>
                <PackedValue>4288108544</PackedValue>
                <X>0</X>
                <Y>88</Y>
                <Z>151</Z>
                <R>0</R>
                <G>88</G>
                <B>151</B>
                <A>255</A>
              </ScriptBackgroundColor>
              <ScriptForegroundColor>
                <PackedValue>4294962611</PackedValue>
                <X>179</X>
                <Y>237</Y>
                <Z>255</Z>
                <R>179</R>
                <G>237</G>
                <B>255</B>
                <A>255</A>
              </ScriptForegroundColor>
              <Sprites>
                <Length>0</Length>
              </Sprites>
              <SelectedRotationIndex>0</SelectedRotationIndex>
            </MyObjectBuilder_CubeBlock>".Replace(',', '.')
            );
        }

        public bool Create()
        {
            string blueprintPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/SpaceEngineers/Blueprints/local/" + name;

            if (!Directory.Exists(blueprintPath))
            {
                Directory.CreateDirectory(blueprintPath);
                image.Write(blueprintPath + "/thumb.png");
                StreamWriter text = File.CreateText(blueprintPath + "/bp.sbc");
                text.Write(BPHeader.Replace("{size}", GridSize.ToString()).Replace("{name}", name));

                text.Write(builder);

                text.Write(BPFooter.Replace("{name}", name));
                text.Close();

                return true;
            }

            return false;
        }

        public void Dispose()
        {
            image.Dispose();
            builder.Clear();
            builder = null;
            image = null;
        }

        private static string BPHeader = @"<?xml version=""1.0""?>
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

        private static string BPFooter = @"
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
        
    }
}

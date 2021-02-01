using ImageMagick;
using SEImageConverter.Resources.Enums;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SEImageConverter.Resources.Windows
{
    class ConverterGIFImage : WindowLCDConverter
    {
        private const string BaseBody = @"<?xml version=""1.0""?>
<Definitions xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <ShipBlueprints>
    <ShipBlueprint xsi:type=""MyObjectBuilder_ShipBlueprintDefinition"">
      <Id Type=""MyObjectBuilder_ShipBlueprintDefinition"" Subtype=""{name}"" />
      <DisplayName>Math0424</DisplayName>
      <CubeGrids>
        <CubeGrid>
          <SubtypeName />
          <EntityId>122728957330135645</EntityId>
          <PersistentFlags>CastShadows InScene</PersistentFlags>
          <PositionAndOrientation>
            <Position x=""13.969420336436087"" y=""18.380873227599295"" z=""-11.591902711606963"" />
            <Forward x=""-0"" y=""-0"" z=""-1"" />
            <Up x=""0"" y=""1"" z=""0"" />
            <Orientation>
              <X>0</X>
              <Y>0</Y>
              <Z>0</Z>
              <W>1</W>
            </Orientation>
          </PositionAndOrientation>
          <LocalPositionAndOrientation xsi:nil=""true"" />
          <GridSizeEnum>{size}</GridSizeEnum>
          <CubeBlocks>
            <MyObjectBuilder_CubeBlock xsi:type=""MyObjectBuilder_MyProgrammableBlock"">
              <SubtypeName>{size}ProgrammableBlock</SubtypeName>
              <EntityId>80955140279512656</EntityId>
              <Min x=""0"" y=""1"" z=""2"" />
              <BlockOrientation Forward=""Right"" Up=""Up"" />
              <Owner>144115188075855895</Owner>
              <BuiltBy>144115188075855895</BuiltBy>
              <ComponentContainer>
                <Components>
                  <ComponentData>
                    <TypeId>MyModStorageComponentBase</TypeId>
                    <Component xsi:type=""MyObjectBuilder_ModStorageComponent"">
                      <Storage>
                        <dictionary>
                          <item>
                            <Key>74de02b3-27f9-4960-b1c4-27351f2b06d1</Key>
                            <Value>{customData}</Value>
                          </item>
                        </dictionary>
                      </Storage>
                    </Component>
                  </ComponentData>
                </Components>
              </ComponentContainer>
              <ShareMode>Faction</ShareMode>
              <CustomName>{name}</CustomName>
              <ShowOnHUD>false</ShowOnHUD>
              <ShowInTerminal>{visible}</ShowInTerminal>
              <ShowInToolbarConfig>{visible}</ShowInToolbarConfig>
              <ShowInInventory>false</ShowInInventory>
              <Enabled>true</Enabled>
              <Program>bool loop = {loop}; int width = {width}; int height = {height}; string data, animateName; double currentDelay = 0, fps; int currentFrame, totalFrames, frameCharacterCount; public Program() { data = Storage; frameCharacterCount = (width * height) + height; totalFrames = data.Length / frameCharacterCount; } public void Main(string argument, UpdateType updateSource) { string[] args = argument.Split(':'); if (updateSource != UpdateType.Update1) { if (args.Length &gt;= 4 &amp;&amp; args[0].ToLower() == ""animate"") { Runtime.UpdateFrequency = UpdateFrequency.Update1; fps = double.Parse(args[1]); currentFrame = int.Parse(args[2]); animateName = args[3]; } if (args.Length &gt;= 1 &amp;&amp; args[0].ToLower() == ""stop"") { Runtime.UpdateFrequency = UpdateFrequency.None; return; } else { Echo(""Unknown argument!""); return; } } if (!loop &amp;&amp; currentFrame == totalFrames-1) { Runtime.UpdateFrequency = UpdateFrequency.None; return; } currentDelay -= 1; if (currentDelay &lt;= 0) { currentDelay = 60.0/fps; if (currentFrame &gt;= totalFrames) { currentFrame = 0; } DisplayText(data.Substring(currentFrame * frameCharacterCount, frameCharacterCount)); currentFrame++; } } public void DisplayText(string text) { List&lt;IMyTextPanel&gt; allText = new List&lt;IMyTextPanel&gt;(); GridTerminalSystem.GetBlocksOfType(allText); if (allText.Count &gt; 0) { int x = 0; for (int i = 0; i &lt; allText.Count; i++) { if (allText[i].CustomName.IndexOf(animateName) &gt; -1 &amp;&amp; data.Length &gt;= frameCharacterCount * 2) { x++; if (x &lt; 100) { allText[i].WritePublicText(text); } } } if (x &gt; 10) { currentDelay += (x - 10); } Echo(""Playing frame "" + (currentFrame + 1) + ""/"" + totalFrames + "" on "" + x + "" screens""); } } public void Save() { Storage = Storage; }</Program>
              <Storage>{data}</Storage>
              <DefaultRunArgument>{args}</DefaultRunArgument>
              <TextPanels>
                <MySerializedTextPanelData>
                  <ChangeInterval>0</ChangeInterval>
                  <Font Type=""MyObjectBuilder_FontDefinition"" Subtype=""Debug"" />
                  <FontSize>1</FontSize>
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
                  <SelectedScript/>
                  <TextPadding>2</TextPadding>
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
                </MySerializedTextPanelData>
                <MySerializedTextPanelData>
                  <ChangeInterval>0</ChangeInterval>
                  <Font Type=""MyObjectBuilder_FontDefinition"" Subtype=""Debug"" />
                  <FontSize>1</FontSize>
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
                  <SelectedScript />
                  <TextPadding>2</TextPadding>
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
                </MySerializedTextPanelData>
              </TextPanels>
            </MyObjectBuilder_CubeBlock>
          </CubeBlocks>
          <DisplayName>{name}</DisplayName>
          <DestructibleBlocks>true</DestructibleBlocks>
          <IsRespawnGrid>false</IsRespawnGrid>
          <LocalCoordSys>0</LocalCoordSys>
          <TargetingTargets />
        </CubeGrid>
      </CubeGrids>
      <EnvironmentType>None</EnvironmentType>
      <WorkshopId>0</WorkshopId>
      <OwnerSteamId>76561198161316860</OwnerSteamId>
      <Points>0</Points>
    </ShipBlueprint>
  </ShipBlueprints>
</Definitions>";

        private const string MultiBlockControllerBody = @"<?xml version=""1.0""?>
<Definitions xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <ShipBlueprints>
    <ShipBlueprint xsi:type=""MyObjectBuilder_ShipBlueprintDefinition"">
      <Id Type=""MyObjectBuilder_ShipBlueprintDefinition"" Subtype=""{name}"" />
      <DisplayName>Math0424</DisplayName>
      <CubeGrids>
        <CubeGrid>
          <SubtypeName />
          <EntityId>122728957330135645</EntityId>
          <PersistentFlags>CastShadows InScene</PersistentFlags>
          <PositionAndOrientation>
            <Position x=""13.969420336436087"" y=""18.380873227599295"" z=""-11.591902711606963"" />
            <Forward x=""-0"" y=""-0"" z=""-1"" />
            <Up x=""0"" y=""1"" z=""0"" />
            <Orientation>
              <X>0</X>
              <Y>0</Y>
              <Z>0</Z>
              <W>1</W>
            </Orientation>
          </PositionAndOrientation>
          <LocalPositionAndOrientation xsi:nil=""true"" />
          <GridSizeEnum>{size}</GridSizeEnum>
          <CubeBlocks>
            <MyObjectBuilder_CubeBlock xsi:type=""MyObjectBuilder_MyProgrammableBlock"">
              <SubtypeName>{size}ProgrammableBlock</SubtypeName>
              <EntityId>80955140279512656</EntityId>
              <Min x=""0"" y=""1"" z=""2"" />
              <BlockOrientation Forward=""Right"" Up=""Up"" />
              <Owner>144115188075855895</Owner>
              <BuiltBy>144115188075855895</BuiltBy>
              <ComponentContainer>
                <Components>
                  <ComponentData>
                    <TypeId>MyModStorageComponentBase</TypeId>
                    <Component xsi:type=""MyObjectBuilder_ModStorageComponent"">
                      <Storage>
                        <dictionary>
                          <item>
                            <Key>74de02b3-27f9-4960-b1c4-27351f2b06d1</Key>
                            <Value>{customData}</Value>
                          </item>
                        </dictionary>
                      </Storage>
                    </Component>
                  </ComponentData>
                </Components>
              </ComponentContainer>
              <ShareMode>Faction</ShareMode>
              <CustomName>{name}</CustomName>
              <ShowOnHUD>false</ShowOnHUD>
              <ShowInTerminal>true</ShowInTerminal>
              <ShowInToolbarConfig>true</ShowInToolbarConfig>
              <ShowInInventory>false</ShowInInventory>
              <Enabled>true</Enabled>
              <Program>bool loop = false; int chainCount = {chainCount}; int multiBlockFrameSize = {frameSize}; int totalFrames = {totalFrames}; string displayName, name; double currentFrameDelay = 0, currentBlockDelay = 0, fps; int currentBlock, currentFrame; public Program() { name = Me.CustomData; StopAll(); } public void Main(string argument, UpdateType updateSource) { string[] args = argument.Split(':'); if (updateSource != UpdateType.Update1) { if (args.Length &gt;= 4 &amp;&amp; args[0].ToLower() == ""animate"") { StopAll(); Runtime.UpdateFrequency = UpdateFrequency.Update1; fps = double.Parse(args[1]); currentFrame = int.Parse(args[2]); currentBlock = currentFrame / multiBlockFrameSize; displayName = args[3]; return; } if (args.Length &gt;= 1 &amp;&amp; args[0].ToLower() == ""stop"") { Runtime.UpdateFrequency = UpdateFrequency.None; StopAll(); return; } else { Echo(""Unknown argument!""); return; } } currentFrameDelay -= 1; if (currentFrameDelay &lt;= 0) { currentFrameDelay = (60.0/fps); if (currentFrame &gt;= totalFrames) { currentFrame = 0; StopAll(); if (!loop) { Runtime.UpdateFrequency = UpdateFrequency.None; return; } } Echo(""Playing frame "" + currentFrame + ""/"" + totalFrames + "" on part "" + currentBlock + ""/"" + chainCount); currentBlockDelay -= 1; if (currentBlockDelay &lt;= 0) { currentBlockDelay = multiBlockFrameSize; int frame = currentFrame - currentBlock * multiBlockFrameSize; List&lt;IMyProgrammableBlock&gt; allBlocks = new List&lt;IMyProgrammableBlock&gt;(); GridTerminalSystem.GetBlocksOfType(allBlocks); if (allBlocks.Count &gt; 0) { for (int i = 0; i &lt; allBlocks.Count; i++) { if (allBlocks[i] != Me &amp;&amp; allBlocks[i].CustomData.Equals(name + ""_"" + currentBlock.ToString())) { allBlocks[i].TryRun(""animate:"" + fps + "":"" + frame + "":"" + displayName); break; } } } currentBlock++; } currentFrame++; } } void StopAll() { currentBlock = 0; currentFrame = 0; currentBlockDelay = 0; List&lt;IMyProgrammableBlock&gt; allBlocks = new List&lt;IMyProgrammableBlock&gt;(); GridTerminalSystem.GetBlocksOfType(allBlocks); if (allBlocks.Count &gt; 0) { for (int i = 0; i &lt; allBlocks.Count; i++) { if (allBlocks[i] != Me &amp;&amp; allBlocks[i].CustomData.Contains(name)) { allBlocks[i].TryRun(""stop""); } } } }</Program>
              <Storage></Storage>
              <DefaultRunArgument>animate:{fps}:0:{name}</DefaultRunArgument>
              <TextPanels>
                <MySerializedTextPanelData>
                  <ChangeInterval>0</ChangeInterval>
                  <Font Type=""MyObjectBuilder_FontDefinition"" Subtype=""Debug"" />
                  <FontSize>1</FontSize>
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
                  <SelectedScript/>
                  <TextPadding>2</TextPadding>
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
                </MySerializedTextPanelData>
                <MySerializedTextPanelData>
                  <ChangeInterval>0</ChangeInterval>
                  <Font Type=""MyObjectBuilder_FontDefinition"" Subtype=""Debug"" />
                  <FontSize>1</FontSize>
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
                  <SelectedScript />
                  <TextPadding>2</TextPadding>
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
                </MySerializedTextPanelData>
              </TextPanels>
            </MyObjectBuilder_CubeBlock>
          </CubeBlocks>
          <DisplayName>{name}</DisplayName>
          <DestructibleBlocks>true</DestructibleBlocks>
          <IsRespawnGrid>false</IsRespawnGrid>
          <LocalCoordSys>0</LocalCoordSys>
          <TargetingTargets />
        </CubeGrid>
      </CubeGrids>
      <EnvironmentType>None</EnvironmentType>
      <WorkshopId>0</WorkshopId>
      <OwnerSteamId>76561198161316860</OwnerSteamId>
      <Points>0</Points>
    </ShipBlueprint>
  </ShipBlueprints>
</Definitions>";

        private const int MultiBlockFrameCount = 1000;

        public string BlueprintName;
        public bool Multiblock;
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
            MagickImage staticImage = Converter.MyPreviewImage;
            MagickImageCollection collection = new MagickImageCollection(staticImage.FileName);
            string blueprintPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/SpaceEngineers/Blueprints/local/" + BlueprintName;

            string[] outputFrames = new string[collection.Count];

            Converter.Instance.GifConvertProgressOne.Dispatcher.Invoke(() => { Converter.Instance.GifConvertProgressOne.Maximum = collection.Count - 1; });

            int count = 0;
            Parallel.For(0, collection.Count, (i) =>
            {
                MagickImage frame = (MagickImage)collection[i];

                Converter.Instance.Dispatcher.Invoke(() => Converter.Instance.ConvertImage(frame));

                using var c = frame.GetPixelsUnsafe();
                StringBuilder myFrame = new StringBuilder();
                for (int y = 0; y < frame.Height; y++)
                {
                    for (int x = 0; x < frame.Width; x++)
                    {
                        var p = c.GetPixel(x, y).ToColor();
                        if (p.A < 100)
                        {
                            myFrame.Append(Utils.ColorToChar(Mode, 0, 0, 0));
                        }
                        else
                        {
                            byte[] b = p.ToByteArray();
                            myFrame.Append(Utils.ColorToChar(Mode, b[0], b[1], b[2]));
                        }
                    }
                    myFrame.Append("\n");
                }

                outputFrames[i] = myFrame.ToString();
                c.Dispose();

                count++;
                Converter.Instance.GifConvertProgressOne.Dispatcher.Invoke(() => { Converter.Instance.GifConvertProgressOne.Value = count; });
            });

            int fps = Converter.MyPreviewImage.AnimationDelay;

            if (Multiblock && outputFrames.Length > MultiBlockFrameCount)
            {
                int blockCount = (int)Math.Ceiling(outputFrames.Length / (MultiBlockFrameCount + 0.0));

                Converter.Instance.GifConvertProgressTwo.Dispatcher.Invoke(() => { Converter.Instance.GifConvertProgressTwo.Maximum = blockCount; });

                for (int i = 0; i <= blockCount; i++)
                {
                    Converter.Instance.GifConvertProgressTwo.Dispatcher.Invoke(() => { Converter.Instance.GifConvertProgressTwo.Value = i; });

                    string newName = BlueprintName + "_" + i.ToString();
                    if (i == blockCount)
                    {
                        newName = BlueprintName + "_MASTER";
                    }

                    string newDir = blueprintPath + "/" + newName;
                    Directory.CreateDirectory(newDir);
                    collection[Math.Min(outputFrames.Length - 1, (i + 1) * MultiBlockFrameCount)].Write(newDir + "/thumb.png");
                    StreamWriter text = File.CreateText(newDir + "/bp.sbc");

                    if (i == blockCount)
                    {
                        text.Write(GetProgrammableBlockController(newName, BlueprintName, fps, outputFrames.Length, blockCount));
                    }
                    else
                    {

                        StringBuilder finalOutputFrames = new StringBuilder();
                        for (int x = i * MultiBlockFrameCount; x < Math.Min(outputFrames.Length, (i + 1) * MultiBlockFrameCount); x++)
                        {
                            finalOutputFrames.Append(outputFrames[x]);
                        }

                        text.Write(GetProgrammableBlock(finalOutputFrames.ToString(), newName, staticImage.Width, staticImage.Height, fps));
                    }

                    text.Close();
                }

            }
            else
            {

                Converter.Instance.GifConvertProgressTwo.Dispatcher.Invoke(() => { Converter.Instance.GifConvertProgressTwo.Maximum = collection.Count - 1; });

                Directory.CreateDirectory(blueprintPath);
                collection[collection.Count / 2].Write(blueprintPath + "/thumb.png");
                StreamWriter text = File.CreateText(blueprintPath + "/bp.sbc");

                StringBuilder finalOutputFrames = new StringBuilder();
                for (int i = 0; i < outputFrames.Length; i++)
                {
                    finalOutputFrames.Append(outputFrames[i]);
                    Converter.Instance.GifConvertProgressTwo.Dispatcher.Invoke(() => { Converter.Instance.GifConvertProgressTwo.Value = i; });
                }

                text.Write(GetProgrammableBlock(finalOutputFrames.ToString(), BlueprintName, staticImage.Width, staticImage.Height, fps));
                text.Close();
            }


            outputFrames = null;
            Converter.Instance.GifConvertProgressOne.Dispatcher.Invoke(() => { Converter.Instance.GifConvertProgressOne.Value = 0; });
            Converter.Instance.GifConvertProgressTwo.Dispatcher.Invoke(() => { Converter.Instance.GifConvertProgressTwo.Value = 0; });
        }

        private string GetProgrammableBlock(string data, string name, int width, int height, int fps, bool multiblockMode = false)
        {
            return BaseBody
                .Replace("{args}", multiblockMode ? "" : "animate:{fps}:0:{name}")
                .Replace("{name}", name)
                .Replace("{width}", width.ToString())
                .Replace("{height}", height.ToString())
                .Replace("{fps}", fps.ToString())
                .Replace("{size}", GridSize.ToString())
                .Replace("{customData}", name)
                .Replace("{loop}", (!multiblockMode).ToString().ToLower())
                .Replace("{visible}", (!multiblockMode).ToString().ToLower())
                .Replace("{customData}", name)
                .Replace("{data}", data);
        }

        private string GetProgrammableBlockController(string name, string controlling, int fps, int totalFrames, int chainCount)
        {
            return MultiBlockControllerBody
                .Replace("{name}", name.Replace("_MASTER", ""))
                .Replace("{size}", GridSize.ToString())
                .Replace("{customData}", controlling)
                .Replace("{totalFrames}", totalFrames.ToString())
                .Replace("{chainCount}", chainCount.ToString())
                .Replace("{frameSize}", MultiBlockFrameCount.ToString())
                .Replace("{fps}", fps.ToString());
        }

        public override void SelectFile()
        {
            FileInfo f = ImageFileSelector("Gif files|*.gif");
            if (f != null)
            {
                Converter.MyPreviewImageSource = new MagickImage(f.FullName);
                Converter.Instance.FilePathTxt.Text = f.FullName;
                Converter.Instance.GifNameInput.Text = Path.GetFileNameWithoutExtension(f.FullName) + "_Gif";
                Converter.Instance.UpdateImage();
            }
        }

        public override void Reset()
        {
            base.Reset();

            if (Converter.Instance.Gif2LCDGrid.Visibility != Visibility.Visible)
            {
                Converter.Instance.GifMultiBlockModeBtn.Content = "Disabled";
                Multiblock = false;
                Converter.Instance.GifGridSizeSelection.SelectedIndex = 0;
            }
        }

        public override void SetVisible(bool visible)
        {
            Utils.SetVisibility(Converter.Instance.ImagePreviewGrid, visible);
            Utils.SetVisibility(Converter.Instance.ImageOptionsGrid, visible);
            Utils.SetVisibility(Converter.Instance.ImageAdvancedGrid, visible);
            Utils.SetVisibility(Converter.Instance.FileSelectGrid, visible);
            Utils.SetVisibility(Converter.Instance.Gif2LCDGrid, visible);

            Utils.SetVisibility(Converter.Instance.ConvertBtn, visible);

            Utils.SetVisibility(Converter.Instance.RotateImageBtn, !visible);
        }

    }
}

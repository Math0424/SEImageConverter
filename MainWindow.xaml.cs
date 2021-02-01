using ImageMagick;
using SEImageConverter.Resources.Enums;
using SEImageConverter.Resources.Util;
using SEImageConverter.Resources.Windows;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SEImageConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Converter : Window
    {
        public static Converter Instance { get; private set; }
        public static MagickImage MyPreviewImageSource { private get; set; }
        public static MagickImage MyPreviewImage { private set; get; }


        private bool IsRunning = false;
        private WindowConverter[] converters;
        private WindowConverter currentWindow;

        public Converter()
        {
            Instance = this;
            ResourceLimits.Memory = 500000000;

            converters = new WindowConverter[Enum.GetValues(typeof(ConvertMode)).Length];


            converters[(int)ConvertMode.Image2LCD] = new ConverterLCDImage();
            converters[(int)ConvertMode.GIF2LCD] = new ConverterGIFImage();
            converters[(int)ConvertMode.Image2Blueprint] = new ConverterImageBlueprint();
            converters[(int)ConvertMode.SpraysModGenerator] = new ConverterSpraysMod();
            converters[(int)ConvertMode.Image2DDS] = new ConverterDDSImage();

            InitializeComponent();
            foreach (WindowConverter c in converters)
            {
                c?.SetVisible(false);
            }

            Utils.SetVisibility(ImageResizeGrid, false);

            ImageLCDInputChanged(null, null);
            IsRunning = true;

            SelectionMenu_SelectionChanged(null, null);
            ResetState();
        }


        public void ConvertImage(MagickImage image)
        {
            double scale = ImageScaleSlider.Value;

            PixelInterpolateMethod method = (PixelInterpolateMethod)InterpolationSelection.SelectedItem;
            DitherMode dither = (DitherMode)DitherModeSelection.SelectedItem;
            BitMode mode = (BitMode)BitModeSelection.SelectedItem;

            currentWindow?.ConvertImage(image, scale, method, dither, mode);
        }

        public void UpdateImage()
        {
            if (MyPreviewImageSource != null)
            {
                MyPreviewImage = (MagickImage)MyPreviewImageSource.Clone();
                ConvertImage(MyPreviewImage);

                using (MagickImage newImage = (MagickImage)MyPreviewImage.Clone())
                {
                    if (newImage.Width < 200 || newImage.Height < 200)
                    {
                        newImage.Scale(200, 200);
                    }
                    PreviewImage.Source = BitmapHelper.ToBitmapSource(newImage);
                    newImage.Dispose();
                }

                ImageSizeLbl.Content = "Size: " + MyPreviewImage.Width + "x" + MyPreviewImage.Height;
            }
        }

        public void ResetState()
        {
            if (!IsRunning)
                return;

            MyPreviewImage = null;
            MyPreviewImageSource = null;
            PreviewImage.Source = null;
            ImageSizeLbl.Content = "Size: 0x0";
            FilePathTxt.Text = "ImagePath";

            ImageScaleSlider.Value = 1;
            DitherModeSelection.SelectedIndex = 0;
            InterpolationSelection.SelectedIndex = 0;
            BitModeSelection.SelectedIndex = 0;
            LCDSizeSelection.SelectedIndex = 0;
        }


        //UI CODE BELOW
        private void SelectionMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MyPreviewImage?.Dispose();
            ResetState();
            foreach (WindowConverter c in converters)
            {
                c?.SetVisible(false);
            }
            currentWindow?.Reset();
            currentWindow = converters[(int)((ConvertMode)SelectionMenu.SelectedItem)];
            currentWindow?.SetVisible(true);
        }

        private void ImageSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SizeSliderValueLbl != null)
                SizeSliderValueLbl.Content = "x" + Math.Round(ImageScaleSlider.Value, 2);

            UpdateImage();
        }

        private void RotateImageBtn_Click(object sender, RoutedEventArgs e)
        {
            MyPreviewImageSource?.Rotate(90);
            UpdateImage();
        }

        private void ImageOptionsChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateImage();
        }
        private void SelectFileBtn_Click(object sender, RoutedEventArgs e)
        {
            currentWindow?.SelectFile();
        }

        private void ConvertBtn_Click(object sender, RoutedEventArgs e)
        {
            string error = currentWindow?.CanConvert();
            if (error == null)
            {
                RunningGrayout.Visibility = Visibility.Visible;
                Thread thread = new Thread(() =>
                {
                    try
                    {
                        currentWindow?.Convert();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("ERROR: " + e.ToString());
                    }
                    RunningGrayout.Dispatcher.Invoke(() => { RunningGrayout.Visibility = Visibility.Hidden; });
                });
                thread.IsBackground = true;
                thread.Start();
            }
            else
            {
                MessageBox.Show("Error, cannot convert.\nMissing: " + error);
            }
        }

        private void ImageLCDInputChanged(object sender, TextChangedEventArgs e)
        {
            if (currentWindow is ConverterLCDImage w && Image2LCDBtnArry != null &&
                int.TryParse(ImageXInput?.Text, out int x) && int.TryParse(ImageYInput?.Text, out int y))
            {
                w.Reset();

                w.X = x;
                w.Y = y;

                Image2LCDBtnArry.Children.Clear();
                Image2LCDBtnArry.Rows = x;
                Image2LCDBtnArry.Columns = y;

                for (int X = 0; X < x; X++)
                {
                    for (int Y = 0; Y < y; Y++)
                    {
                        Button b = new Button();
                        b.Content = $"{X}x{Y}";
                        b.Name = $"Btn_{X}x{Y}";

                        b.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(114, 137, 218));
                        b.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(35, 39, 42));
                        b.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));

                        int myX = X, myY = Y;
                        b.Click += (e, m) => GetAndCopyToClipboard(b, myX, myY);

                        Grid.SetColumn(b, X);
                        Grid.SetRow(b, Y);

                        Image2LCDBtnArry.Children.Add(b);

                    }
                }
            }
            UpdateImage();
        }

        private void GetAndCopyToClipboard(Button b, int x, int y)
        {
            if (currentWindow is ConverterLCDImage w)
            {
                string s = w.ConvertedImage?[x, y];

                ToolTip tool = new ToolTip();
                tool.IsOpen = true;
                tool.StaysOpen = false;

                if (s != null)
                {
                    try
                    {
                        Clipboard.SetText(s);
                        tool.Content = "Copied to clipboard";
                        b.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(114, 137, 218));
                    }
                    catch
                    {
                        tool.Content = "Couldnt copy to clipboard, please try again";
                        b.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 137, 218));
                    }
                }
                else
                {
                    tool.Content = "Nothing to copy...";
                    b.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(250, 150, 90));
                }
                b.ToolTip = tool;
                b.ToolTip = null;
            }
        }


        private void GifNameInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((ConverterGIFImage)converters[(int)ConvertMode.GIF2LCD]).BlueprintName = ((TextBox)sender).Text;
        }

        private void GifGridSizeSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((ConverterGIFImage)converters[(int)ConvertMode.GIF2LCD]).GridSize = (GridSize)((ComboBox)sender).SelectedItem;
        }

        private void GifMultiBlockModeBtn_Click(object sender, RoutedEventArgs e)
        {
            bool multi = ((ConverterGIFImage)converters[(int)ConvertMode.GIF2LCD]).Multiblock;
            ((ConverterGIFImage)converters[(int)ConvertMode.GIF2LCD]).Multiblock = !multi;
            if (multi)
            {
                ((Button)sender).Content = "Disabled";
            }
            else
            {
                ((Button)sender).Content = "Enabled";
            }
        }

        private void BlueprintGridSizeSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((ConverterImageBlueprint)converters[(int)ConvertMode.Image2Blueprint]).GridSize = (GridSize)((ComboBox)sender).SelectedItem;
        }

        private void BlueprintNameInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((ConverterImageBlueprint)converters[(int)ConvertMode.Image2Blueprint]).BlueprintName = ((TextBox)sender).Text;
        }

        private void Image2DDSMaskCbx_Checked(object sender, RoutedEventArgs e)
        {
            ((ConverterDDSImage)converters[(int)ConvertMode.Image2DDS]).GenerateMask = ((CheckBox)sender).IsChecked.Value;
        }

    }
}

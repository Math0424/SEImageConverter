using System.Windows;

namespace SEImageConverter.Resources.Enums
{
    public static class Utils
    {
        public static char ColorToChar(BitMode mode, byte r, byte g, byte b)
        {
            return mode switch
            {
                BitMode.Bit3 => ColorToChar3Bit(r, g, b),
                BitMode.Bit5 => ColorToChar5Bit(r, g, b),
                _ => '0',
            };
        }

        private static char ColorToChar3Bit(byte r, byte g, byte b)
        {
            return (char)((0xe100) + ((r >> 5) << 6) + ((g >> 5) << 3) + (b >> 5));
        }

        private static char ColorToChar5Bit(byte r, byte g, byte b)
        {
            return (char)((uint)0x3000 + ((r >> 3) << 10) + ((g >> 3) << 5) + (b >> 3));
        }

        public static void SetVisibility(UIElement element, bool visible)
        {
            if (element != null)
                element.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
        }

        public static int[] LCDSizeToNum(LCDSize lcd)
        {
            int[] i = new int[2];

            string desc = lcd.GetDescription();

            string[] ints = desc.Split('-');
            i[0] = int.Parse(ints[0]);
            i[1] = int.Parse(ints[1]);
            return i;
        }


    }
}

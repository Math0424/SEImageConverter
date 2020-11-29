using ImageMagick;
using SEImageConverter.Resources.Enums;
using System.IO;
using System.Text;

namespace SEImageConverter.Resources.Windows
{
    class ConverterSpraysMod : WindowConverter
    {


        public override string CanConvert()
        {
            return "Not implemented";
        }

        public override void Convert()
        {
            
        }

        public override void SelectFile()
        {
            
        }

        public override void SetVisible(bool visible)
        {
            Utils.SetVisibility(Converter.Instance.SpraysModGenGrid, visible);
        }

        public override void Reset()
        {

        }

    }
}

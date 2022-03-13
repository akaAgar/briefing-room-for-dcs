using WkHtmlWrapper.Core.Attributes;
using WkHtmlWrapper.Core.Options.Interfaces;

namespace WkHtmlWrapper.Image.Options
{
    public class GeneralImageOptions : IOptions
    {
        [ConsoleLineParameter("--crop-h")]
        public int CropH { get; set; }

        [ConsoleLineParameter("--crop-w")]
        public int CropW { get; set; }

        [ConsoleLineParameter("--crop-x")]
        public int CropX { get; set; }

        [ConsoleLineParameter("--crop-y")]
        public int CropY { get; set; }
        
        [ConsoleLineParameter("--height")]
        public int Height { get; set; }

        [ConsoleLineParameter("--quality")]
        public int Quality { get; set; } = 96;

        [ConsoleLineParameter("--transparent")]
        public bool Transparent { get; set; }

        [ConsoleLineParameter("--width")]
        public int Width { get; set; } = 1024;

        [ConsoleLineParameter("--zoom")]
        public float Zoom { get; set; } = 1;
    }
}
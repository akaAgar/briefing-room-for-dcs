using System.Runtime.CompilerServices;
using WkHtmlWrapper.Core.Converters;
using WkHtmlWrapper.Core.Enums;
using WkHtmlWrapper.Core.Services.Interfaces;
using WkHtmlWrapper.Image.Converters.Interfaces;
using WkHtmlWrapper.Image.Options;

[assembly: InternalsVisibleTo("WkHtmlWrapper")]
[assembly: InternalsVisibleTo("WkHtmlWrapper.UnitTests")]
namespace WkHtmlWrapper.Image.Converters
{
    public class HtmlToImageConverter : Converter<GeneralImageOptions>, IHtmlToImageConverter
    {
        public HtmlToImageConverter()
        {
            ConverterType = ConverterType.Image;
        }

        internal HtmlToImageConverter(IProcessService processService) : base(processService)
        {
            ConverterType = ConverterType.Image;
        }
    }
}

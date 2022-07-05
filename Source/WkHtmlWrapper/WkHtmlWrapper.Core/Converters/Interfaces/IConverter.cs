using System.IO;
using System.Threading.Tasks;
using WkHtmlWrapper.Core.Options.Interfaces;

namespace WkHtmlWrapper.Core.Converters.Interfaces
{
    public interface IConverter<TOptions> where TOptions : IOptions
    {
        Task<string> ConvertAsync(string html, string outputFile);

        Task<string> ConvertAsync(string html, string outputFile, TOptions options);

        Task<string> ConvertAsync(Stream html, string outputFile);

        Task<string> ConvertAsync(Stream html, string outputFile, TOptions options);
    }
}
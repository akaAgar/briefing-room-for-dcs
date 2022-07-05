using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WkHtmlWrapper.Core.Converters.Interfaces;
using WkHtmlWrapper.Core.Enums;
using WkHtmlWrapper.Core.Extensions;
using WkHtmlWrapper.Core.Options.Interfaces;
using WkHtmlWrapper.Core.Services;
using WkHtmlWrapper.Core.Services.Interfaces;

namespace WkHtmlWrapper.Core.Converters
{
    public abstract class Converter<TOptions> : IConverter<TOptions> where TOptions : IOptions, new()
    {
        private readonly IProcessService processService;

        protected ConverterType ConverterType;

        protected Converter()
        {
            processService = new ProcessService();
        }

        internal Converter(IProcessService processService)
        {
            this.processService = processService;
        }

        public async Task<string> ConvertAsync(string html, string outputFile) =>
            await ConvertAsync(html, outputFile, new TOptions());

        public async Task<string> ConvertAsync(string html, string outputFile, TOptions options) =>
            await ConvertAsync(new MemoryStream(Encoding.UTF8.GetBytes(html)), outputFile, options);

        public async Task<string> ConvertAsync(Stream html, string outputFile) =>
            await ConvertAsync(html, outputFile, new TOptions());

        public async Task<string> ConvertAsync(Stream html, string outputFile, TOptions options)
        {
            var inputFilePath = SaveFile(html);
            var arguments = $"{options.OptionsToCommandLineParameters()} \"{inputFilePath}\" \"{outputFile}\"";
            var logs = await processService.StartAsync(GetExecutablePath(), arguments);
            File.Delete(inputFilePath);
            return logs;
        }

        private string GetExecutablePath() =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Executables", GetExecutableName());

        private string GetExecutableName() =>
            $"{ConverterType}.{GetOsDependentExecutableNamePart()}".ToLower();

        private string GetOsDependentExecutableNamePart()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "win.exe";
            }
            throw new NotImplementedException();
        }

        private string SaveFile(Stream inputStream)
        {
            var temporaryFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.html");
            using (var fileStream = new FileStream(temporaryFilePath, FileMode.CreateNew))
            {
                inputStream.CopyTo(fileStream);
            }
            return temporaryFilePath;
        }
    }
}
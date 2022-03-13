using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WkHtmlWrapper.Core.Services.Interfaces;

namespace WkHtmlWrapper.Core.Services
{
    internal class ProcessService : IProcessService
    {
        public async Task<string> StartAsync(string filename, string arguments)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo(filename, arguments)
                {
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardError = true,
                }
            };

            process.Start();
            process.EnableRaisingEvents = true;
            await process.WaitForExitAsync();
            return await process.StandardError.ReadToEndAsync();
        }
    }
}
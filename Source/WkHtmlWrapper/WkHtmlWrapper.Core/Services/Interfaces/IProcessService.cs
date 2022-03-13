using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("WkHtmlWrapper")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace WkHtmlWrapper.Core.Services.Interfaces
{
    internal interface IProcessService
    {
        Task<string> StartAsync(string filename, string arguments);
    }
}
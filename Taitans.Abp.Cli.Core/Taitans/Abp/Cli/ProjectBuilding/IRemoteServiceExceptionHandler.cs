using System.Net.Http;
using System.Threading.Tasks;

namespace Taitans.Abp.Cli.ProjectBuilding
{
    public interface IRemoteServiceExceptionHandler
    {
        Task EnsureSuccessfulHttpResponseAsync(HttpResponseMessage responseMessage);

        Task<string> GetAbpRemoteServiceErrorAsync(HttpResponseMessage responseMessage);
    }
}
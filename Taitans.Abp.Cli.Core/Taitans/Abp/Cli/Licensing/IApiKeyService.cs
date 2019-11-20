using System.Threading.Tasks;

namespace Taitans.Abp.Cli.Licensing
{
    public interface IApiKeyService
    {
        Task<DeveloperApiKeyResult> GetApiKeyOrNullAsync();
    }
}
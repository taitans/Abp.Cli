using System.Threading.Tasks;
using Taitans.Abp.Cli.ProjectBuilding.Building;

namespace Taitans.Abp.Cli.ProjectBuilding
{
    public interface IModuleInfoProvider
    {
        Task<ModuleInfo> GetAsync(string name);
    }
}

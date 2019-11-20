using System.Threading.Tasks;

namespace Taitans.Abp.Cli.ProjectBuilding
{
    public interface IProjectBuilder
    {
        Task<ProjectBuildResult> BuildAsync(ProjectBuildArgs args);
    }
}
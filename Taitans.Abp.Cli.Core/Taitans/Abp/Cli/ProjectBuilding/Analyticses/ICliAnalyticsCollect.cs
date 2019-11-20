using System.Threading.Tasks;

namespace Taitans.Abp.Cli.ProjectBuilding.Analyticses
{
    public interface ICliAnalyticsCollect
    {
        Task CollectAsync(CliAnalyticsCollectInputDto input);
    }
}

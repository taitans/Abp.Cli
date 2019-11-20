using Taitans.Abp.Cli.ProjectBuilding.Building;

namespace Taitans.Abp.Cli.ProjectBuilding
{
    public interface ITemplateInfoProvider
    {
        TemplateInfo GetDefault();

        TemplateInfo Get(string name);
    }
}
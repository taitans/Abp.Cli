using Taitans.Abp.Cli;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Taitans.Abp.Cli
{
    [DependsOn(
        typeof(AbpCliCoreModule),
        typeof(AbpAutofacModule)
    )]
    public class AbpCliModule : AbpModule
    {

    }
}

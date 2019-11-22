using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using Taitans.Abp.Cli.Commands;
using Taitans.Abp.Cli.ProjectBuilding.Analyticses;
using Taitans.Abp.Cli.ProjectBuilding.Building;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Json;

namespace Taitans.Abp.Cli.ProjectBuilding
{
    public class ModuleProjectBuilder : IProjectBuilder, ITransientDependency
    {
        public ILogger<ModuleProjectBuilder> Logger { get; set; }

        protected ISourceCodeStore SourceCodeStore { get; }
        protected IModuleInfoProvider ModuleInfoProvider { get; }
        protected ICliAnalyticsCollect CliAnalyticsCollect { get; }
        protected AbpCliOptions Options { get; }
        protected IJsonSerializer JsonSerializer { get; }

        public ModuleProjectBuilder(ISourceCodeStore sourceCodeStore,
            IModuleInfoProvider moduleInfoProvider,
            ICliAnalyticsCollect cliAnalyticsCollect,
            IOptions<AbpCliOptions> options,
            IJsonSerializer jsonSerializer)
        {
            SourceCodeStore = sourceCodeStore;
            ModuleInfoProvider = moduleInfoProvider;
            CliAnalyticsCollect = cliAnalyticsCollect;
            Options = options.Value;
            JsonSerializer = jsonSerializer;

            Logger = NullLogger<ModuleProjectBuilder>.Instance;
        }

        public async Task<ProjectBuildResult> BuildAsync(ProjectBuildArgs args)
        {
            var moduleInfo = await GetModuleInfoAsync(args);

            var templateFile = await SourceCodeStore.GetAsync(
                args.TemplateName,
                SourceCodeTypes.Module,
                args.Version
            );

            var context = new ProjectBuildContext(
                null,
                moduleInfo,
                templateFile,
                args
            );

            ModuleProjectBuildPipelineBuilder.Build(context).Execute();

            if (!moduleInfo.DocumentUrl.IsNullOrEmpty())
            {
                Logger.LogInformation("Check out the documents at " + moduleInfo.DocumentUrl);
            }

            // Exclude unwanted or known options.
            var options = args.ExtraProperties
                .Where(x => !x.Key.Equals(CliConsts.Command, StringComparison.InvariantCultureIgnoreCase))
                .Where(x => !x.Key.Equals(NewCommand.Options.OutputFolder.Long, StringComparison.InvariantCultureIgnoreCase) &&
                            !x.Key.Equals(NewCommand.Options.OutputFolder.Short, StringComparison.InvariantCultureIgnoreCase))
                .Where(x => !x.Key.Equals(NewCommand.Options.Version.Long, StringComparison.InvariantCultureIgnoreCase) &&
                            !x.Key.Equals(NewCommand.Options.Version.Short, StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.Key).ToList();

            await CliAnalyticsCollect.CollectAsync(new CliAnalyticsCollectInputDto
            {
                Tool = Options.ToolName,
                Command = args.ExtraProperties.ContainsKey(CliConsts.Command) ? args.ExtraProperties[CliConsts.Command] : "",
                DatabaseProvider = null,
                IsTiered = null,
                UiFramework = null,
                Options = JsonSerializer.Serialize(options),
                ProjectName = null,
                TemplateName = args.TemplateName,
                TemplateVersion = templateFile.Version
            });

            return new ProjectBuildResult(context.Result.ZipContent, args.TemplateName);
        }

        private async Task<ModuleInfo> GetModuleInfoAsync(ProjectBuildArgs args)
        {
            return await ModuleInfoProvider.GetAsync(args.TemplateName);
        }
    }
}

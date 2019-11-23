using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using Taitans.Abp.Cli.Commands;
using Taitans.Abp.Cli.ProjectBuilding.Analyticses;
using Taitans.Abp.Cli.ProjectBuilding.Building;
using Taitans.Abp.Cli.ProjectBuilding.Templates.MvcModule;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Json;

namespace Taitans.Abp.Cli.ProjectBuilding
{
    public class TemplateProjectBuilder : IProjectBuilder, ITransientDependency
    {
        public ILogger<TemplateProjectBuilder> Logger { get; set; }

        protected ISourceCodeStore SourceCodeStore { get; }
        protected ICliAnalyticsCollect CliAnalyticsCollect { get; }
        protected AbpCliOptions Options { get; }
        protected IJsonSerializer JsonSerializer { get; }

        public TemplateProjectBuilder(ISourceCodeStore sourceCodeStore,
            ICliAnalyticsCollect cliAnalyticsCollect, 
            IOptions<AbpCliOptions> options,
            IJsonSerializer jsonSerializer)
        {
            SourceCodeStore = sourceCodeStore;
            CliAnalyticsCollect = cliAnalyticsCollect;
            Options = options.Value;
            JsonSerializer = jsonSerializer;

            Logger = NullLogger<TemplateProjectBuilder>.Instance;
        }
        
        public async Task<ProjectBuildResult> BuildAsync(ProjectBuildArgs args)
        {
            var templateInfo = GetTemplateInfo();

            NormalizeArgs(args, templateInfo);

            var templateFile = await SourceCodeStore.GetAsync(
                args.TemplateName,
                SourceCodeTypes.Template,
                args.Version
            );


            // 项目构建上下文
            var context = new ProjectBuildContext(
                templateInfo,
                null,
                templateFile,
                args
            );

            TemplateProjectBuildPipelineBuilder.Build(context).Execute();

            if (!templateInfo.DocumentUrl.IsNullOrEmpty())
            {
                Logger.LogInformation("Check out the documents at " + templateInfo.DocumentUrl);
            }

            // Exclude unwanted or known options.
            var options = args.ExtraProperties
                .Where(x => !x.Key.Equals(CliConsts.Command, StringComparison.InvariantCultureIgnoreCase))
                .Where(x => !x.Key.Equals("tiered", StringComparison.InvariantCultureIgnoreCase))
                .Where(x => !x.Key.Equals(NewCommand.Options.DatabaseProvider.Long, StringComparison.InvariantCultureIgnoreCase) && 
                            !x.Key.Equals(NewCommand.Options.DatabaseProvider.Short, StringComparison.InvariantCultureIgnoreCase))
                .Where(x => !x.Key.Equals(NewCommand.Options.OutputFolder.Long, StringComparison.InvariantCultureIgnoreCase) &&
                            !x.Key.Equals(NewCommand.Options.OutputFolder.Short, StringComparison.InvariantCultureIgnoreCase))
                .Where(x => !x.Key.Equals(NewCommand.Options.UiFramework.Long, StringComparison.InvariantCultureIgnoreCase) &&
                            !x.Key.Equals(NewCommand.Options.UiFramework.Short, StringComparison.InvariantCultureIgnoreCase))
                .Where(x => !x.Key.Equals(NewCommand.Options.Version.Long, StringComparison.InvariantCultureIgnoreCase) &&
                            !x.Key.Equals(NewCommand.Options.Version.Short, StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.Key).ToList();

            return new ProjectBuildResult(context.Result.ZipContent, args.SolutionName.ProjectName);
        }

        private static void NormalizeArgs(ProjectBuildArgs args, TemplateInfo templateInfo)
        {
            if (args.TemplateName.IsNullOrEmpty())
            {
                args.TemplateName = templateInfo.Name;
            }
        }

        private TemplateInfo GetTemplateInfo()
        {
            return new ModuleTemplate();
        }
    }
}

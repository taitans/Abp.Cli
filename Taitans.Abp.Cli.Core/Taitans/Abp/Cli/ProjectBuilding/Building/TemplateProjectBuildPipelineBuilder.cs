using System;
using Taitans.Abp.Cli.ProjectBuilding.Building.Steps;

namespace Taitans.Abp.Cli.ProjectBuilding.Building
{
    public static class TemplateProjectBuildPipelineBuilder
    {
        /// <summary>
        /// 模版构建步骤
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ProjectBuildPipeline Build(ProjectBuildContext context)
        {
            var pipeline = new ProjectBuildPipeline(context); // 构建项目步骤管道

            pipeline.Steps.Add(new FileEntryListReadStep());

            pipeline.Steps.AddRange(context.Template.GetCustomSteps(context));

            pipeline.Steps.Add(new ProjectReferenceReplaceStep()); // 项目引用替换步骤
            pipeline.Steps.Add(new TemplateCodeDeleteStep());  // 模版代码删除步骤
            pipeline.Steps.Add(new SolutionRenameStep()); // 方案重命名步骤

            pipeline.Steps.Add(new CreateProjectResultZipStep()); // 创建项目输出资源步骤

            return pipeline;
        }
    }
}

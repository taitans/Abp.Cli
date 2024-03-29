﻿using System.Collections.Generic;
using JetBrains.Annotations;
using Taitans.Abp.Cli.ProjectBuilding.Building;
using Taitans.Abp.Cli.ProjectBuilding.Building.Steps;

namespace Taitans.Abp.Cli.ProjectBuilding.Templates.Module
{
    public abstract class ModuleTemplateBase : TemplateInfo
    {
        protected ModuleTemplateBase([NotNull] string name)
            : base(name)
        {
        }

        public override IEnumerable<ProjectBuildPipelineStep> GetCustomSteps(ProjectBuildContext context)
        {
            var steps = new List<ProjectBuildPipelineStep>();

            DeleteUnrelatedProjects(context, steps); // 
            RandomizeSslPorts(context, steps);
            CleanupFolderHierarchy(context, steps);

            return steps;
        }

        private void DeleteUnrelatedProjects(ProjectBuildContext context, List<ProjectBuildPipelineStep> steps)
        {
            steps.Add(new RemoveProjectFromSolutionStep(
                "MyCompanyName.MyProjectName.Web"
            ));

            steps.Add(new RemoveProjectFromSolutionStep(
                "MyCompanyName.MyProjectName.Web.Host",
                projectFolderPath: "/aspnet-core/host/MyCompanyName.MyProjectName.Web.Host"
            ));

            steps.Add(new RemoveProjectFromSolutionStep(
                "MyCompanyName.MyProjectName.Web.Unified",
                projectFolderPath: "/aspnet-core/host/MyCompanyName.MyProjectName.Web.Unified"
            ));

            steps.Add(new RemoveProjectFromSolutionStep(
                "MyCompanyName.MyProjectName.IdentityServer",
                projectFolderPath: "/aspnet-core/host/MyCompanyName.MyProjectName.IdentityServer"
            ));
        }

        private void RandomizeSslPorts(ProjectBuildContext context, List<ProjectBuildPipelineStep> steps)
        {
            steps.Add(new TemplateRandomSslPortStep(new List<string>
            {
                "https://localhost:44300",
                "https://localhost:44301",
                "https://localhost:44302",
                "https://localhost:44303"
            }));
        }

        private void CleanupFolderHierarchy(ProjectBuildContext context, List<ProjectBuildPipelineStep> steps)
        {
            steps.Add(new MoveFolderStep("/aspnet-core/", "/"));
        }
    }
}

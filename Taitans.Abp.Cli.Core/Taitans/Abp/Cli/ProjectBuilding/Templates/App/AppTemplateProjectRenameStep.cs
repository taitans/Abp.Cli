﻿using Taitans.Abp.Cli.ProjectBuilding.Building;
using Taitans.Abp.Cli.ProjectBuilding.Building.Steps;
using Taitans.Abp.Cli.ProjectBuilding.Files;

namespace Taitans.Abp.Cli.ProjectBuilding.Templates.App
{
    public class AppTemplateProjectRenameStep : ProjectBuildPipelineStep
    {
        private readonly string _oldProjectName;
        private readonly string _newProjectName;

        public AppTemplateProjectRenameStep(
            string oldProjectName,
            string newProjectName)
        {
            _oldProjectName = oldProjectName;
            _newProjectName = newProjectName;
        }

        public override void Execute(ProjectBuildContext context)
        {
            context
                .GetFile("/aspnet-core/MyCompanyName.MyProjectName.sln")
                .ReplaceText(_oldProjectName, _newProjectName);
            
            RenameHelper.RenameAll(context.Files, _oldProjectName, _newProjectName);
        }
    }
}

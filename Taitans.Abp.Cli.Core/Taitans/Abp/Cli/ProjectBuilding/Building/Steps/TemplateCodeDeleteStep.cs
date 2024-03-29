﻿using Taitans.Abp.Cli.ProjectBuilding.Files;

namespace Taitans.Abp.Cli.ProjectBuilding.Building.Steps
{
    public class TemplateCodeDeleteStep : ProjectBuildPipelineStep
    {
        public override void Execute(ProjectBuildContext context)
        {
            foreach (var file in context.Files)
            {
                if (file.Name.EndsWith(".cs")) //TODO: Why only cs!
                {
                    file.RemoveTemplateCode();
                    file.RemoveTemplateCodeMarkers();
                }
            }
        }
    }
}
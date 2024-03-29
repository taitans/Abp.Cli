﻿using System.Collections.Generic;
using JetBrains.Annotations;
using Volo.Abp;
using Taitans.Abp.Cli.ProjectBuilding.Building;

namespace Taitans.Abp.Cli.ProjectBuilding
{
    public class ProjectBuildArgs
    {
        [NotNull]
        public SolutionName SolutionName { get; }

        [CanBeNull]
        public string TemplateName { get; set; }

        [CanBeNull]
        public string Version { get; set; }

        [CanBeNull]
        public string AbpGitHubLocalRepositoryPath { get; set; }

        [NotNull]
        public Dictionary<string, string> ExtraProperties { get; set; }

        public ProjectBuildArgs(
            [NotNull] SolutionName solutionName, 
            [CanBeNull] string templateName = null,
            [CanBeNull] string version = null,
            [CanBeNull] string abpGitHubLocalRepositoryPath = null,
            Dictionary<string, string> extraProperties = null)
        {
            SolutionName = Check.NotNull(solutionName, nameof(solutionName));
            TemplateName = templateName;
            Version = version;
            AbpGitHubLocalRepositoryPath = abpGitHubLocalRepositoryPath;
            ExtraProperties = extraProperties ?? new Dictionary<string, string>();
        }
    }
}
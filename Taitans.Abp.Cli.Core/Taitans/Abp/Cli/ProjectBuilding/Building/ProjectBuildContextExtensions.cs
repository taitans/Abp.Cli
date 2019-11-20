using System;
using System.Linq;
using Taitans.Abp.Cli.ProjectBuilding.Files;

namespace Taitans.Abp.Cli.ProjectBuilding.Building
{
    public static class ProjectBuildContextExtensions
    {
        public static FileEntry GetFile(this ProjectBuildContext context, string filePath)
        {
            var file = context.Files.FirstOrDefault(f => f.Name == filePath);
            if (file == null)
            {
                throw new ApplicationException("Could not find file: " + filePath);
            }

            return file;
        }
    }
}
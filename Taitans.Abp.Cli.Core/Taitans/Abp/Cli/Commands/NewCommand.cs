using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Taitans.Abp.Cli.Args;
using Taitans.Abp.Cli.ProjectBuilding;
using Taitans.Abp.Cli.ProjectBuilding.Building;
using Volo.Abp.DependencyInjection;

namespace Taitans.Abp.Cli.Commands
{
    public class NewCommand : IConsoleCommand, ITransientDependency
    {
        public ILogger<NewCommand> Logger { get; set; }

        protected TemplateProjectBuilder TemplateProjectBuilder { get; }

        public NewCommand(TemplateProjectBuilder templateProjectBuilder)
        {
            TemplateProjectBuilder = templateProjectBuilder;

            Logger = NullLogger<NewCommand>.Instance;
        }

        public async Task ExecuteAsync(CommandLineArgs commandLineArgs)
        {
            if (commandLineArgs.Target == null)
            {
                throw new CliUsageException(
                    "Project name is missing!" +
                    Environment.NewLine + Environment.NewLine +
                    GetUsageInfo()
                );
            }

            Logger.LogInformation("Creating your project...");
            Logger.LogInformation("Project name: " + commandLineArgs.Target);

            var version = commandLineArgs.Options.GetOrNull(Options.Version.Short, Options.Version.Long);
            if (version != null)
            {
                Logger.LogInformation("Version: " + version);
            }


            var gitHubLocalRepositoryPath = commandLineArgs.Options.GetOrNull(Options.GitHubLocalRepositoryPath.Long);
            if (gitHubLocalRepositoryPath != null)
            {
                Logger.LogInformation("GitHub Local Repository Path: " + gitHubLocalRepositoryPath);
            }

            var outputFolder = commandLineArgs.Options.GetOrNull(Options.OutputFolder.Short, Options.OutputFolder.Long);

            outputFolder = Path.Combine(outputFolder != null ? Path.GetFullPath(outputFolder) : Directory.GetCurrentDirectory(),
                    SolutionName.Parse(commandLineArgs.Target).FullName);

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            Logger.LogInformation("Output folder: " + outputFolder);

            commandLineArgs.Options.Add(CliConsts.Command, commandLineArgs.Command);

            var result = await TemplateProjectBuilder.BuildAsync(
                new ProjectBuildArgs(
                    SolutionName.Parse(commandLineArgs.Target),
                    "module",
                    version,
                    gitHubLocalRepositoryPath,
                    commandLineArgs.Options
                )
            );

            using (var templateFileStream = new MemoryStream(result.ZipContent))
            {
                using (var zipInputStream = new ZipInputStream(templateFileStream))
                {
                    var zipEntry = zipInputStream.GetNextEntry();
                    while (zipEntry != null)
                    {
                        var fullZipToPath = Path.Combine(outputFolder, zipEntry.Name);
                        var directoryName = Path.GetDirectoryName(fullZipToPath);

                        if (!string.IsNullOrEmpty(directoryName))
                        {
                            Directory.CreateDirectory(directoryName);
                        }

                        var fileName = Path.GetFileName(fullZipToPath);
                        if (fileName.Length == 0)
                        {
                            zipEntry = zipInputStream.GetNextEntry();
                            continue;
                        }

                        var buffer = new byte[4096]; // 4K is optimum
                        using (var streamWriter = File.Create(fullZipToPath))
                        {
                            StreamUtils.Copy(zipInputStream, streamWriter, buffer);
                        }

                        zipEntry = zipInputStream.GetNextEntry();
                    }
                }
            }

            Logger.LogInformation($"'{commandLineArgs.Target}' has been successfully created to '{outputFolder}'");
        }

        public string GetUsageInfo()
        {
            var sb = new StringBuilder();

            sb.AppendLine("");
            sb.AppendLine("Usage:");
            sb.AppendLine("");
            sb.AppendLine("  abp new <project-name> [options]");
            sb.AppendLine("");
            sb.AppendLine("Options:");
            sb.AppendLine("");
            sb.AppendLine("-o|--output-folder <output-folder>          (default: current folder)");
            sb.AppendLine("-v|--version <version>                      (default: latest version)");
            sb.AppendLine("--local-framework-ref --abp-path <your-local-abp-repo-path>  (keeps local references to projects instead of replacing with NuGet package references)");
            sb.AppendLine("");
            sb.AppendLine("Examples:");
            sb.AppendLine("");
            sb.AppendLine("  abp new Acme.BookStore");
            sb.AppendLine("  abp new Acme.BookStore --local-framework-ref --abp-path \"D:\\github\\abp\"");

            return sb.ToString();
        }

        public string GetShortDescription()
        {
            return "Generates a new solution based on the ABP startup templates.";
        }

        protected virtual DatabaseProvider GetDatabaseProvider(CommandLineArgs commandLineArgs)
        {
            var optionValue = commandLineArgs.Options.GetOrNull(Options.DatabaseProvider.Short, Options.DatabaseProvider.Long);
            switch (optionValue)
            {
                case "ef":
                    return DatabaseProvider.EntityFrameworkCore;
                case "mongodb":
                    return DatabaseProvider.MongoDb;
                default:
                    return DatabaseProvider.NotSpecified;
            }
        }

        private UiFramework GetUiFramework(CommandLineArgs commandLineArgs)
        {
            var optionValue = commandLineArgs.Options.GetOrNull(Options.UiFramework.Short, Options.UiFramework.Long);
            switch (optionValue)
            {
                case "mvc":
                    return UiFramework.Mvc;
                case "angular":
                    return UiFramework.Angular;
                default:
                    return UiFramework.NotSpecified;
            }
        }

        public static class Options
        {
            public static class Template
            {
                public const string Short = "t";
                public const string Long = "template";
            }

            public static class DatabaseProvider
            {
                public const string Short = "d";
                public const string Long = "database-provider";
            }

            public static class OutputFolder
            {
                public const string Short = "o";
                public const string Long = "output-folder";
            }

            public static class GitHubLocalRepositoryPath
            {
                public const string Long = "abp-path";
            }

            public static class Version
            {
                public const string Short = "v";
                public const string Long = "version";
            }

            public static class UiFramework
            {
                public const string Short = "u";
                public const string Long = "ui";
            }
        }
    }
}
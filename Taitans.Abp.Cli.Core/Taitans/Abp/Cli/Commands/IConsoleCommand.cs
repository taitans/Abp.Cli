using System.Threading.Tasks;
using Taitans.Abp.Cli.Args;

namespace Taitans.Abp.Cli.Commands
{
    public interface IConsoleCommand
    {
        Task ExecuteAsync(CommandLineArgs commandLineArgs);

        string GetUsageInfo();

        string GetShortDescription();
    }
}
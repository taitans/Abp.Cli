using System;
using Taitans.Abp.Cli.Args;

namespace Taitans.Abp.Cli.Commands
{
    public interface ICommandSelector
    {
        Type Select(CommandLineArgs commandLineArgs);
    }
}
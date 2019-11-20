namespace Taitans.Abp.Cli.Args
{
    public interface ICommandLineArgumentParser
    {
        CommandLineArgs Parse(string[] args);
    }
}
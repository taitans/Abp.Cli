using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Volo.Abp;
using Volo.Abp.Threading;

namespace Taitans.Abp.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Path.Combine(CliPaths.Log, "abp-cli-logs.txt"));
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Taitans.Abp", LogEventLevel.Warning)
#if DEBUG
                .MinimumLevel.Override("Taitans.Abp.Cli", LogEventLevel.Debug)
#else
                .MinimumLevel.Override("Volo.Abp.Cli", LogEventLevel.Information)
#endif
                .Enrich.FromLogContext()
                .WriteTo.File(Path.Combine(CliPaths.Log, "abp-cli-logs.txt"))
                .WriteTo.Console()
                .CreateLogger();

            using (var application = AbpApplicationFactory.Create<AbpCliModule>(
                options =>
                {
                    options.UseAutofac();
                    options.Services.AddLogging(c => c.AddSerilog());
                }))
            {
                application.Initialize();

                AsyncHelper.RunSync(
                    () => application.ServiceProvider
                        .GetRequiredService<CliService>()
                        .RunAsync(args)
                );

                application.Shutdown();
            }
        }
    }
}

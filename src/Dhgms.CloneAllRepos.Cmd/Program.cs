using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dhgms.CloneAllRepos.Cmd
{

    // https://docs.microsoft.com/en-us/dotnet/standard/commandline/get-started-tutorial
    public sealed class ConsoleResultTracker
    {
        public int ResultCode { get; set; }
    }

    /// <summary>
    /// Application Entry point.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Application Entry point.
        /// </summary>
        /// <param name="args">Command line arguments from the operating system.</param>
        /// <returns>Status code indicating success or failure to the operating system.</returns>
        public static Task<int> Main(string[] args)
        {
            return RunConsoleApp(args);
        }

        private static async Task<int> RunConsoleApp(string[] args)
        {
            try
            {
                var resultTracker = new ConsoleResultTracker();

                var builder = GetHostBuilder(
                    args,
                    resultTracker);

                await builder.RunConsoleAsync()
                    .ConfigureAwait(false);

                return resultTracker.ResultCode;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                Console.WriteLine(e);

                return e.HResult != 0 ? e.HResult : -1;
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        private static IHostBuilder GetHostBuilder(string[] args, ConsoleResultTracker consoleResultTracker)
        {
            var consoleApplicationInitialization = new ConsoleApplicationInitialization();
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostBuilder, config) =>
                {
                    consoleApplicationInitialization.DoApplicationConfiguration(hostBuilder, config);
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    consoleApplicationInitialization.ConfigureLogging(
                        hostingContext,
                        logging);
                })
                .ConfigureServices(serviceCollection =>
                {
                    serviceCollection.AddSingleton(consoleResultTracker);

                    consoleApplicationInitialization.DoConfigureServices(
                        serviceCollection,
                        args);
                });

            return builder;
        }
    }

    /*
    internal static class Program
    {
        public static Task<int> Main(string[] args)
        {
            using (var loggerFactory = new LoggerFactory())
            {
                ConfigureLogging(loggerFactory);

                // todo : update to dotnetcore and use an async main
                var consoleAppJob = new ConsoleAppJob(loggerFactory);
                return consoleAppJob.Handle(args);
            }
        }

        private static void ConfigureLogging(ILoggerFactory loggerFactory)
        {
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties =true });
            var config = new LoggingConfiguration();
            var consoleTarget = new ColoredConsoleTarget();
            config.AddTarget("console", consoleTarget);
            var fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            const string defaultLayout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";
            consoleTarget.Layout = defaultLayout;
            var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            fileTarget.FileName = $"{localAppDataPath}/dhgms solutions/cloneallrepos.cmd/nlogfile.txt";
            fileTarget.Layout = defaultLayout;

            var consoleLogLevel = Debugger.IsAttached ? NLog.LogLevel.Debug : NLog.LogLevel.Info;
            var rule1 = new LoggingRule("*", consoleLogLevel, consoleTarget);
            config.LoggingRules.Add(rule1);

            var rule2 = new LoggingRule("*", NLog.LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(rule2);

            loggerFactory.ConfigureNLog(config);
        }
    }
    */
}
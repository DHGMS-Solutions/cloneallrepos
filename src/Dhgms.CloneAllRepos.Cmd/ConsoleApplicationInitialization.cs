using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;

namespace Dhgms.CloneAllRepos.Cmd
{
    public sealed class ConsoleApplicationInitialization
    {
        public void ConfigureLogging(HostBuilderContext hostingContext, ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
            loggingBuilder.AddConsole();
            loggingBuilder.AddNLog(new NLogProviderOptions
            {
                CaptureMessageTemplates = true,
                CaptureMessageProperties = true
            });

            var config = new LoggingConfiguration();

            var fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            const string defaultLayout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";
            var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            fileTarget.FileName = $"{localAppDataPath}/dhgms solutions/cloneallrepos.cmd/nlogfile.txt";
            fileTarget.Layout = defaultLayout;

            var rule2 = new LoggingRule("*", NLog.LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(rule2);

            NLog.LogManager.Configuration = config;
        }

        public void DoConfigureServices(IServiceCollection serviceCollection, string[] args)
        {
            throw new NotImplementedException();
        }

        public void DoApplicationConfiguration(HostBuilderContext hostBuilder, IConfigurationBuilder config)
        {
            config.AddJsonFile("appsettings.json", optional: true);
            config.AddEnvironmentVariables();
        }
    }
}

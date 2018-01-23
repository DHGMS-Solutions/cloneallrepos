using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using SystemWrapper.IO;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx.Synchronous;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;

namespace Dhgms.CloneAllRepos.Cmd
{
    internal static class Program
    {
        public static int Main(string[] args)
        {
            using (var loggerFactory = new LoggerFactory())
            {
                ConfigureLogging(loggerFactory);

                // todo : update to dotnetcore and use an async main
                var consoleAppJob = new ConsoleAppJob(loggerFactory);
                return consoleAppJob.Handle(args).Result;
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
}
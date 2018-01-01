using System.Linq;
using System.Threading;
using SystemWrapper.IO;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx.Synchronous;
using NLog.Extensions.Logging;

namespace Dhgms.CloneAllRepos.Cmd
{
    internal static class Program
    {
        public static int Main(string[] args)
        {
            using (var loggerFactory = new LoggerFactory())
            {
                loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties =true });
                loggerFactory.ConfigureNLog("nlog.config");

                // todo : update to dotnetcore and use an async main
                var consoleAppJob = new ConsoleAppJob(loggerFactory);
                return consoleAppJob.Handle(args).Result;
            }
        }
    }
}

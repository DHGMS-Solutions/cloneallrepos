using System.Linq;
using System.Threading;
using SystemWrapper.IO;
using Nito.AsyncEx.Synchronous;

namespace Dhgms.CloneAllRepos.Cmd
{
    internal static class Program
    {
        public static int Main(string[] args)
        {
            // todo : update to dotnetcore and use an async main
            var consoleAppJob = new ConsoleAppJob();
            return consoleAppJob.Handle(args).Result;
        }
    }
}

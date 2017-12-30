using System.Linq;
using SystemWrapper.IO;

namespace Dhgms.CloneAllRepos.Cmd
{
    internal static class Program
    {
        public static int Main(string[] args)
        {
            var consoleAppJob = new ConsoleAppJob();
            return consoleAppJob.Handle(args).Result;
        }
    }
}

using System.Linq;
using SystemWrapper.IO;
using Dhgms.CloneAllRepos.Cmd.Errands;

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

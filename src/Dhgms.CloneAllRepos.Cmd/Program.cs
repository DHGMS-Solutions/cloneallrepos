using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dhgms.CloneAllRepos.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get settings from command line
            var commandLineArgs = new CommandLineArguments();
            if (!CommandLine.Parser.Default.ParseArguments(args, commandLineArgs))
            {
                return;
            }

            var jobHandler = new Job();
            jobHandler.ExecuteAsync(commandLineArgs).Wait();
        }
    }
}

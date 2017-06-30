using System.Linq;
using SystemWrapper.IO;

namespace Dhgms.CloneAllRepos.Cmd
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var commandLineArgs = new CommandLineArguments();

            // if no command line args passed, look in environment variables
            var argsToParse = args.Length == 0 ? args : GetArgumentsFromEnvironmentVariables();

            if (!CommandLine.Parser.Default.ParseArgumentsStrict(argsToParse, commandLineArgs))
            {
                return;
            }

            var directory = new DirectoryWrap();
            var jobHandler = new Job(directory);
            jobHandler.ExecuteAsync(commandLineArgs).Wait();
        }

        private static string[] GetArgumentsFromEnvironmentVariables()
        {
            //System.Environment.GetEnvironmentVariables().Where(x => x.Key.StartsWith("Dhgms.CloneAllRepos."));
            return new string[] { };
        }
    }
}

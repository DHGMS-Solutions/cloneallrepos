namespace Dhgms.CloneAllRepos.Cmd
{
    internal static class Program
    {
        public static void Main(string[] args)
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

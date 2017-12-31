using CommandLine;

namespace Dhgms.CloneAllRepos.Cmd.CommandLineVerbs
{
    public class BitBucketCommandLineVerb : BaseCommandLineVerb
    {
        [Option('a', "apiKey", Required = true, HelpText = "The Github API Key to be used.")]
        public string ApiKey { get; set; }
    }
}
using CommandLine;
using Dhgms.CloneAllRepos.Cmd.Settings;

namespace Dhgms.CloneAllRepos.Cmd.CommandLineVerbs
{
    public class BitBucketCommandLineVerb : BaseCommandLineVerb, ICloneBitBucketJobSettings
    {
        [Option('a', "apiKey", Required = true, HelpText = "The Github API Key to be used.")]
        public string ApiKey { get; set; }
    }
}
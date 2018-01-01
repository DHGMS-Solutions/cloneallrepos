using CommandLine;
using Dhgms.CloneAllRepos.Cmd.Settings;

namespace Dhgms.CloneAllRepos.Cmd.CommandLineVerbs
{
    [Verb("bitbucket", HelpText = "Clones a bitbucket account")]
    public class BitBucketCommandLineVerb : BaseCommandLineVerb, ICloneBitBucketJobSettings
    {
        [Option('a', "apikey", Required = true, HelpText = "The Github API Key to be used.")]
        public string ApiKey { get; set; }
    }
}
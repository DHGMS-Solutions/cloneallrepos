using Dhgms.CloneAllRepos.Cmd.Settings;

namespace Dhgms.CloneAllRepos.Cmd.CommandLineVerbs
{
    //[Verb("github", HelpText = "Clones a github account")]
    public sealed record GitHubCommandLineVerb(string ApiKey, string RootDirectory, bool WhatIf) : BaseCommandLineVerb(RootDirectory, WhatIf), ICloneGitHubJobSettings
    {
        //[Option('a', "apikey", Required = true, HelpText = "The Github API Key to be used.")]
        //public string ApiKey { get; set; }
    }
}

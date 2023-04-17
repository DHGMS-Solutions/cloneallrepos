using Dhgms.CloneAllRepos.Cmd.Settings;

namespace Dhgms.CloneAllRepos.Cmd.CommandLineVerbs
{
    //[Verb("bitbucket", HelpText = "Clones a bitbucket account")]
    public record BitBucketCommandLineVerb(string BaseUrl, string Base64AuthToken, string RootDirectory, bool WhatIf) : BaseCommandLineVerb(RootDirectory, WhatIf), ICloneBitBucketJobSettings
    {
    }
}
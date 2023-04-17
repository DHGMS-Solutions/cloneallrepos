using Dhgms.CloneAllRepos.Cmd.Settings;

namespace Dhgms.CloneAllRepos.Cmd.CommandLineVerbs
{
    //[Verb("tfs", HelpText = "Clones a TFS account")]
    public sealed record TeamFoundationServerCommandLineVerb(string RootDirectory, bool WhatIf) : BaseCommandLineVerb(RootDirectory, WhatIf), ICloneTeamFoundationServerJobSettings
    {
    }
}

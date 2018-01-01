using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using Dhgms.CloneAllRepos.Cmd.Settings;

namespace Dhgms.CloneAllRepos.Cmd.CommandLineVerbs
{
    [Verb("tfs")]
    public sealed class TeamFoundationServerCommandLineVerb : BaseCommandLineVerb, ICloneTeamFoundationServerJobSettings
    {
    }
}

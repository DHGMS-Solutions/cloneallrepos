using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using Dhgms.CloneAllRepos.Cmd.Settings;
using MediatR;

namespace Dhgms.CloneAllRepos.Cmd.CommandLineVerbs
{
    [Verb("github")]
    public sealed class GitHubCommandLineVerb : BaseCommandLineVerb, ICloneGitHubJobSettings
    {
        [Option('a', "apiKey", Required = true, HelpText = "The Github API Key to be used.")]
        public string ApiKey { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using MediatR;

namespace Dhgms.CloneAllRepos.Cmd.CommandLineVerbs
{
    [Verb("github")]
    public sealed class GitHubCommandLineVerb : BaseCommandLineVerb
    {
        [Option('a', "apiKey", Required = true, HelpText = "The Github API Key to be used.")]
        public string ApiKey { get; set; }
    }
}

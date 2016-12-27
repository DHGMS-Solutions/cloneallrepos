using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Dhgms.CloneAllRepos.Cmd
{
    public sealed class CommandLineArguments : IJobSettings
    {
        [Option('k', "apikey", Required = true, HelpText = "The Github API Key to be used")]
        public string ApiKey { get; set; }

        [Option('w', "whatif", Required = false, DefaultValue = false, HelpText = "Simulates what will be carried out, without making changes to the local system")]
        public bool WhatIf { get; set; }
    }
}

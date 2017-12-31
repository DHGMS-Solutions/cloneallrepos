using CommandLine;

namespace Dhgms.CloneAllRepos.Cmd.CommandLineVerbs
{
    public class BaseCommandLineVerb : IJobSettings
    {
        [Option('r', "rootdir", Required = true, HelpText = "Root directory in which to place the repositories.")]
        public string RootDir { get; set; }

        [Option('w', "whatif", Required = false, Default = false, HelpText = "Simulates what will be carried out, without making changes to the local system")]
        public bool WhatIf { get; set; }
    }
}
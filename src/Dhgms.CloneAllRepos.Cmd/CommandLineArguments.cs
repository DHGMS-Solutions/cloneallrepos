namespace Dhgms.CloneAllRepos.Cmd
{
    using CommandLine;

    public sealed class CommandLineArguments : IJobSettings
    {
        [Option('g', "githubApiKey", Required = true, HelpText = "The Github API Key to be used.")]
        public string GitHubApiKey { get; set; }

        [Option('r', "rootdir", Required = true, HelpText = "Root directory in which to place the repositories.")]
        public string RootDir { get; set; }

        [Option('w', "whatif", Required = false, DefaultValue = false, HelpText = "Simulates what will be carried out, without making changes to the local system")]
        public bool WhatIf { get; set; }
    }
}

using System;
using System.Threading.Tasks;
using Dhgms.CloneAllRepos.Cmd.CommandLineVerbs;
using Dhgms.CloneAllRepos.Cmd.RequestHandlers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Octokit;

namespace Dhgms.CloneAllRepos.Cmd
{
    using SystemWrapper.IO;

    public sealed class ConsoleAppJob : BaseVerbBasedConsoleAppJob<BitBucketCommandLineVerb, CloneFromBitBucketRequestHandler, GitHubCommandLineVerb, CloneFromGithubRequestHandler, TeamFoundationServerCommandLineVerb, CloneFromTeamFoundationServerRequestHandler>
    {
        protected override CloneFromBitBucketRequestHandler GetT1Job(BitBucketCommandLineVerb opts)
        {
            throw new System.NotImplementedException();
        }

        protected override CloneFromGithubRequestHandler GetT2Job(GitHubCommandLineVerb opts)
        {
            var directory = new DirectoryWrap();
            var pathSystem = new PathWrap();
            var logFactory = new NullLoggerFactory();
            var logger = logFactory.CreateLogger<CloneFromGithubRequestHandler>();
            //Func<Repository, string, ILogger, Task> cloneAction = opts.WhatIf ? SimulateCloneAsync : DoActualCloneAsync;
            return new CloneFromGithubRequestHandler(logger, directory, pathSystem, SimulateCloneAsync);
        }

        protected override CloneFromTeamFoundationServerRequestHandler GetT3Job(TeamFoundationServerCommandLineVerb opts)
        {
            throw new System.NotImplementedException();
        }

        private async Task SimulateCloneAsync(Repository repository, string targetDirectory, ILogger logger)
        {
            await Task.Run(() => logger.LogInformation($"WHATIF: Would have cloned {repository.Url} to {targetDirectory}"));
        }

        private async Task DoActualCloneAsync(Repository repository, string targetDirectory, ILogger logger)
        {
            logger.LogInformation($"Cloning {repository.Url} to {targetDirectory}");
            LibGit2Sharp.Repository.Clone(repository.CloneUrl, targetDirectory);
        }
    }
}

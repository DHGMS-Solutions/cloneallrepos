﻿using System;
using System.Threading.Tasks;
using Dhgms.CloneAllRepos.Cmd.CommandLineVerbs;
using Dhgms.CloneAllRepos.Cmd.RequestHandlers;
using Dhgms.CloneAllRepos.Cmd.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MediatR;
using Octokit;

namespace Dhgms.CloneAllRepos.Cmd
{
    public sealed class ConsoleAppJob : BaseVerbBasedConsoleAppJob<
        BitBucketCommandLineVerb,
        ICloneBitBucketJobSettings,
        GitHubCommandLineVerb,
        ICloneGitHubJobSettings,
        TeamFoundationServerCommandLineVerb,
        ICloneTeamFoundationServerJobSettings,
        ConsoleAppJob>
    {
        public ConsoleAppJob(LoggerFactory loggerFactory)
            : base(loggerFactory)
        {
        }

        protected override IRequestHandler<ICloneBitBucketJobSettings> GetT1Job(ICloneBitBucketJobSettings opts)
        {
            throw new System.NotImplementedException();
        }

        protected override IRequestHandler<ICloneGitHubJobSettings> GetT2Job(ICloneGitHubJobSettings opts)
        {
            var cloneAction = opts.WhatIf
                ? (Func<Repository, string, ILogger, Task>)this.SimulateCloneAsync
                : this.DoActualCloneAsync;
            return new CloneFromGithubRequestHandler(this.LoggerFactory.CreateLogger<CloneFromGithubRequestHandler>(), cloneAction);
        }

        protected override IRequestHandler<ICloneTeamFoundationServerJobSettings> GetT3Job(ICloneTeamFoundationServerJobSettings opts)
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

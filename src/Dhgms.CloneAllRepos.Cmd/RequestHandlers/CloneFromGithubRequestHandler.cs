﻿using Dhgms.CloneAllRepos.Cmd.CommandLineVerbs;
using Dhgms.CloneAllRepos.Cmd.Settings;

namespace Dhgms.CloneAllRepos.Cmd.RequestHandlers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Dhgms.CloneAllRepos.Cmd.Exceptions;
    using Foundatio.Utility;
    using JetBrains.Annotations;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using Octokit;
    using Octokit.Internal;
    using SystemInterface.IO;

    public sealed class CloneFromGithubRequestHandler : IRequestHandler<ICloneGitHubJobSettings>
    {
        private readonly ILogger _logger;
        private readonly IDirectory _directorySystem;
        private readonly IPath _pathSystem;
        private Func<Repository, string, ILogger, Task> _cloneAction;

        public CloneFromGithubRequestHandler(
            ILogger logger,
            IDirectory directorySystem,
            IPath pathSystem,
            Func<Repository, string, ILogger, Task> cloneAction)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._directorySystem = directorySystem ?? throw new ArgumentNullException(nameof(directorySystem));
            this._pathSystem = pathSystem ?? throw new ArgumentNullException(nameof(pathSystem));
            this._cloneAction = cloneAction ?? throw new ArgumentNullException(nameof(cloneAction));
        }

        public async Task Handle([NotNull]ICloneGitHubJobSettings jobSettings, CancellationToken cancellationToken)
        {
            if (jobSettings == null)
            {
                throw new ArgumentNullException(nameof(jobSettings));
            }

            var apiKey = jobSettings.ApiKey;

            var rootDir = jobSettings.RootDir;

            // validate home directory
            if (!this._directorySystem.Exists(rootDir))
            {
                throw new DirectoryNotFoundException(rootDir);
            }

            var gitHubClient = await this.GetGitHubClientWithApiKeyAsync(apiKey);

            // check user
            await CloneAllRepositoriesForUser(rootDir, gitHubClient);

            // check all organisations
            await CloneAllOrganisationsForUser(rootDir, gitHubClient);

            // check stars
            await CloneAllStarsForUser(rootDir, gitHubClient);
        }

        private async Task CloneAllStarsForUser(string rootDir, GitHubClient gitHubClient)
        {
            var currentUser = await gitHubClient.User.Current();
            var orgs = await gitHubClient.Organization.GetAllForCurrent();

            var orgsAndUserToSkipStarsOn = orgs.Select(o => o.Name).ToList();
            orgsAndUserToSkipStarsOn.Add(currentUser.Name);

            await FetchListAndLoopIfNotEmptyAsync(
                gitHubClient.Activity.Starring.GetAllForCurrent,
                OnNoStarsForUser,
                repository => CloneStarForUser(rootDir, repository, orgsAndUserToSkipStarsOn),
                () => EnsureStarFolderExists(rootDir));
        }

        private async Task EnsureStarFolderExists(string rootDir)
        {
            var starsFolder = this._pathSystem.Combine(rootDir, "stars");
            this.EnsureDirectoryExists(starsFolder);
        }

        private void EnsureDirectoryExists(string path)
        {
            if (this._directorySystem.Exists(path))
            {
                return;
            }

            this._directorySystem.CreateDirectory(path);
        }

        private async Task OnNoStarsForUser()
        {
            this.GetLogger().LogInformation("No stars for user.");
        }

        private async Task CloneAllOrganisationsForUser(string rootDir, GitHubClient gitHubClient)
        {
            await FetchListAndLoopIfNotEmptyAsync(
                gitHubClient.Organization.GetAllForCurrent,
                async () => { },
                organization => this.DoOrganisationAsync(organization, gitHubClient));
        }

        private async Task CloneAllRepositoriesForUser(string rootDir, GitHubClient gitHubClient)
        {
            await FetchListAndLoopIfNotEmptyAsync(
                gitHubClient.Repository.GetAllForCurrent,
                async () => { },
                repository => CloneRepositoryForUser(rootDir, repository));
        }

        private async Task CloneStarForUser(
            string rootDir,
            Repository repository,
            IEnumerable<string> organisationsAndUsersToSkipStarsOn)
        {
            // check the owner folder exists
            // already done stars pre loop
            var ownerName = repository.Owner.Name;

            if (organisationsAndUsersToSkipStarsOn.Any(skip => skip.Equals(ownerName, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            var targetDirectory = this._pathSystem.Combine(rootDir, "stars", ownerName);
            this.EnsureDirectoryExists(targetDirectory);

            // check if the repo folder exists
            var repositoryName = repository.Name;
            targetDirectory = this._pathSystem.Combine(targetDirectory, repositoryName);

            await CloneRepository(repository, targetDirectory);
        }

        private async Task CloneRepositoryForUser(
            string rootDir,
            Repository repository)
        {
            // check if the repo folder exists
            // already done organisation pre-loop
            var ownerName = repository.Owner.Name;
            var repositoryName = repository.Name;
            var targetDirectory = this._pathSystem.Combine(rootDir, ownerName, repositoryName);

            await CloneRepository(repository, targetDirectory);
        }

        private async Task CloneRepository(
            [NotNull]Repository repository,
            [NotNull]string targetDirectory)
        {
            if (this._directorySystem.Exists(targetDirectory))
            {
                if (LibGit2Sharp.Repository.IsValid(targetDirectory))
                {
                    return;
                }

                var filesInDirectory = this._directorySystem.EnumerateFiles(targetDirectory).Any();

                if (filesInDirectory)
                {
                    throw new TargetDirectoryNotEmptyException(targetDirectory);
                }
            }

            await this._cloneAction(repository, targetDirectory, this._logger);
        }

        private static async Task FetchListAndLoopIfNotEmptyAsync<TItem>(
            [NotNull]Func<Task<IReadOnlyList<TItem>>> collectionProducerTask,
            Func<Task> emptyListAction,
            [NotNull]Func<TItem, Task> itemConsumerTask,
            Func<Task> preLoopTask = null,
            Func<Task> postLoopTask = null)
        {
            if (collectionProducerTask == null)
            {
                throw new ArgumentNullException(nameof(collectionProducerTask));
            }

            if (itemConsumerTask == null)
            {
                throw new ArgumentNullException(nameof(itemConsumerTask));
            }

            var collection = await collectionProducerTask();

            if (collection.Count < 1)
            {
                if (emptyListAction != null)
                {
                    await emptyListAction();
                }

                return;
            }

            if (preLoopTask != null)
            {
                await preLoopTask();
            }

            foreach (var item in collection)
            {
                await itemConsumerTask(item);
            }

            if (postLoopTask != null)
            {
                await postLoopTask();
            }
        }

        private async Task DoOrganisationAsync(Organization organization, GitHubClient gitHubClient)
        {
            await FetchListAndLoopIfNotEmptyAsync(
                () => gitHubClient.Repository.GetAllForOrg(organization.Login),
                null,
                item => this.DoRepositoryAsync(item, gitHubClient));
        }

        private async Task DoRepositoryAsync(Repository repository, GitHubClient gitHubClient)
        {

        }

        private async Task<GitHubClient> GetGitHubClientWithApiKeyAsync([NotNull]string apiKey)
        {
            return await TaskEx.FromResult(this.GetGitHubClientWithApiKey(apiKey));
            //{
            //new Octokit.GitHubClient(productInformation, credentialStore)
            //});
        }

        private GitHubClient GetGitHubClientWithApiKey([NotNull]string apiKey)
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            var productInformation = new Octokit.ProductHeaderValue("DHGMS.CloneAllRepos", version);
            var credentials = new Credentials(apiKey);
            var credentialStore = new InMemoryCredentialStore(credentials);
            return new Octokit.GitHubClient(productInformation, credentialStore);
        }
    }
}

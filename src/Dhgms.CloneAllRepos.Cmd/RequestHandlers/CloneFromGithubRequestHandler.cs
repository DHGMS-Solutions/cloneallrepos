using Dhgms.CloneAllRepos.Cmd.CommandLineVerbs;
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

    public sealed class CloneFromGithubRequestHandler : IRequestHandler<ICloneGitHubJobSettings>
    {
        private readonly ILogger<CloneFromGithubRequestHandler> _logger;
        private Func<Repository, string, ILogger, Task> _cloneAction;

        public CloneFromGithubRequestHandler(
            ILogger<CloneFromGithubRequestHandler> logger,
            Func<Repository, string, ILogger, Task> cloneAction)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._cloneAction = cloneAction ?? throw new ArgumentNullException(nameof(cloneAction));
        }

        public async Task Handle([NotNull]ICloneGitHubJobSettings jobSettings, CancellationToken cancellationToken)
        {
            if (jobSettings == null)
            {
                throw new ArgumentNullException(nameof(jobSettings));
            }

            var apiKey = jobSettings.ApiKey;

            var rootDir = jobSettings.RootDirectory;

            // validate home directory
            if (!System.IO.Directory.Exists(rootDir))
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
            var starsFolder = Path.Combine(rootDir, "stars");
            this.EnsureDirectoryExists(starsFolder);
        }

        private void EnsureDirectoryExists(string path)
        {
            if (Directory.Exists(path))
            {
                return;
            }

            Directory.CreateDirectory(path);
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

            var targetDirectory = Path.Combine(rootDir, "stars", ownerName);
            this.EnsureDirectoryExists(targetDirectory);

            // check if the repo folder exists
            var repositoryName = repository.Name;
            targetDirectory = Path.Combine(targetDirectory, repositoryName);

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
            var targetDirectory = Path.Combine(rootDir, ownerName, repositoryName);

            await CloneRepository(repository, targetDirectory);
        }

        private async Task CloneRepository(
            [NotNull]Repository repository,
            [NotNull]string targetDirectory)
        {
            if (Directory.Exists(targetDirectory))
            {
                if (LibGit2Sharp.Repository.IsValid(targetDirectory))
                {
                    return;
                }

                var filesInDirectory = Directory.EnumerateFiles(targetDirectory).Any();

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
            return await Task.FromResult(this.GetGitHubClientWithApiKey(apiKey));
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

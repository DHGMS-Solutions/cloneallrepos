using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SystemInterface.IO;
using Foundatio.Utility;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.Logging;
using Octokit;
using Octokit.Internal;

namespace Dhgms.CloneAllRepos.Cmd
{
    public sealed class CloneFromGithubRequestHandler : IRequestHandler<IJobSettings>
    {
        private readonly IDirectory _directory;

        public CloneFromGithubRequestHandler(IDirectory directory)
        {
            this._directory = directory ?? throw new ArgumentNullException(nameof(directory));
        }

        public async Task Handle([NotNull]IJobSettings jobSettings, CancellationToken cancellationToken)
        {
            if (jobSettings == null)
            {
                throw new ArgumentNullException(nameof(jobSettings));
            }

            var apiKey = jobSettings.GitHubApiKey;

            var rootDir = jobSettings.RootDir;

            // validate home directory
            if (!_directory.Exists(rootDir))
            {
                throw new DirectoryNotFoundException(rootDir);
            }

            var gitHubClient = await this.GetGitHubClientWithApiKeyAsync(apiKey);

            // check user
            await CloneAllRepositoriesForUser(gitHubClient);

            // check all organisations
            await CloneAllOrganisationsForUser(gitHubClient);

            // check stars
            await CloneAllStarsForUser(gitHubClient);
        }

        private async Task CloneAllStarsForUser(GitHubClient gitHubClient)
        {
            await FetchListAndLoopIfNotEmptyAsync(
                gitHubClient.Activity.Starring.GetAllForCurrent,
                OnNoStarsForUser,
                CloneStarForUser);
        }

        private Task CloneStarForUser(Repository arg)
        {
        }


        private async Task OnNoStarsForUser()
        {
            this.GetLogger().LogInformation("No stars for user.");
        }

        private async Task CloneAllOrganisationsForUser(GitHubClient gitHubClient)
        {
            await FetchListAndLoopIfNotEmptyAsync(
                gitHubClient.Organization.GetAllForCurrent,
                async () => { },
                organization => this.DoOrganisationAsync(organization, gitHubClient));
        }

        private async Task CloneAllRepositoriesForUser(GitHubClient gitHubClient)
        {
            await FetchListAndLoopIfNotEmptyAsync(
                gitHubClient.Repository.GetAllForCurrent,
                async () => { },
                repository => { });
        }

        private static async Task FetchListAndLoopIfNotEmptyAsync<TItem>(
            [NotNull]Func<Task<IReadOnlyList<TItem>>> collectionProducerTask,
            Func<Task> emptyListAction,
            [NotNull]Func<TItem, Task> itemConsumerTask)
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

            foreach (var item in collection)
            {
                await itemConsumerTask(item);
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

        internal async Task<GitHubClient> GetGitHubClientWithApiKeyAsync([NotNull]string apiKey)
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

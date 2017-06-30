using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemInterface.IO;
using JetBrains.Annotations;
using Octokit;
using Octokit.Internal;

namespace Dhgms.CloneAllRepos.Cmd
{
    public sealed class Job
    {
        private readonly IDirectory _directory;

        public Job(IDirectory directory)
        {
            this._directory = directory ?? throw new ArgumentNullException(nameof(directory));
        }

        /// <summary>
        /// Executes the job
        /// </summary>
        /// <param name="jobSettings">The settings for the job</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">No job settings were passed.</exception>
        public async Task ExecuteAsync([NotNull]IJobSettings jobSettings)
        {
            if (jobSettings == null)
            {
                throw new ArgumentNullException(nameof(jobSettings));
            }

            var apiKey = jobSettings.ApiKey;

            var gitHubClient = await this.GetGitHubClientWithApiKeyAsync(apiKey);

            var rootDir = jobSettings.RootDir;

            // validate home directory
            if (!_directory.Exists(rootDir))
            {
                throw new DirectoryNotFoundException(rootDir);
            }

            // check user

            // check organizations of user
            await FetchListAndLoopIfNotEmptyAsync(
                gitHubClient.Organization.GetAllForCurrent,
                async () =>  { },
                organization => this.DoOrganisationAsync(organization, gitHubClient));
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
            var productInformation = new Octokit.ProductHeaderValue("DHGMS-Gitstronomy", version);
            var credentials = new Credentials(apiKey);
            var credentialStore = new InMemoryCredentialStore(credentials);
            return new Octokit.GitHubClient(productInformation, credentialStore);
        }
    }
}

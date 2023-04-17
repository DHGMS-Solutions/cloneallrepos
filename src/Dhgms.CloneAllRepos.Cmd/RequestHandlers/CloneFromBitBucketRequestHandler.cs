using System;
using System.Threading;
using System.Threading.Tasks;
using Atlassian.Stash;
using Dhgms.CloneAllRepos.Cmd.CommandLineVerbs;
using Dhgms.CloneAllRepos.Cmd.Settings;
using MediatR;
using Microsoft.Extensions.Logging;
using Octokit;
using Project = Atlassian.Stash.Entities.Project;

namespace Dhgms.CloneAllRepos.Cmd.RequestHandlers
{
    public sealed class CloneFromBitBucketRequestHandler : IRequestHandler<ICloneBitBucketJobSettings>
    {
        private readonly ILogger<CloneFromBitBucketRequestHandler> _logger;
        private Func<Repository, string, ILogger, Task> _cloneAction;

        public CloneFromBitBucketRequestHandler(
            ILogger<CloneFromBitBucketRequestHandler> logger,
            Func<Repository, string, ILogger, Task> cloneAction)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._cloneAction = cloneAction ?? throw new ArgumentNullException(nameof(cloneAction));
        }

        public async Task Handle(
            ICloneBitBucketJobSettings message,
            CancellationToken cancellationToken)
        {
            var stashClient = new StashClient(message.BaseUrl, message.Base64AuthToken, true);

            var projectWrapper = await stashClient.Projects.Get()
                .ConfigureAwait(false);

            foreach (var project in projectWrapper.Values)
            {
                await ProcessProject(stashClient, project).ConfigureAwait(false);
            }
        }

        private async Task ProcessProject(StashClient stashClient, Project project)
        {
            var repoWrapper = await stashClient.Repositories.Get(project.Key)
                .ConfigureAwait(false);

            foreach (var repository in repoWrapper.Values)
            {
            }
        }
    }
}

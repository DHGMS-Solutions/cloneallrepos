using System;
using System.Threading;
using System.Threading.Tasks;
using Dhgms.CloneAllRepos.Cmd.Requests;
using MediatR;

namespace Dhgms.CloneAllRepos.Cmd.RequestHandlers
{
    public sealed class CloneGitRepositoryRequestHandler : IRequestHandler<CloneGitRepositoryRequest>
    {
        public Task Handle(CloneGitRepositoryRequest message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

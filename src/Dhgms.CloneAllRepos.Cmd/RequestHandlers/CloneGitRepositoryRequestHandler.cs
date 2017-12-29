using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dhgms.CloneAllRepos.Cmd.Requests;
using MediatR;

namespace Dhgms.CloneAllRepos.Cmd.Jobs
{
    public sealed class CloneGitRepositoryRequestHandler : IRequestHandler<CloneGitRepositoryRequest>
    {
        public Task Handle(CloneGitRepositoryRequest message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

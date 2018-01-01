using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dhgms.CloneAllRepos.Cmd.Settings;
using MediatR;

namespace Dhgms.CloneAllRepos.Cmd.RequestHandlers
{
    public sealed class CloneFromTeamFoundationServerRequestHandler : IRequestHandler<ICloneTeamFoundationServerJobSettings>
    {
        public Task Handle(ICloneTeamFoundationServerJobSettings message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

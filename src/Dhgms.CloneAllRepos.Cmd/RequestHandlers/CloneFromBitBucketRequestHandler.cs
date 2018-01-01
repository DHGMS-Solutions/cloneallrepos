using System.Threading;
using System.Threading.Tasks;
using Dhgms.CloneAllRepos.Cmd.CommandLineVerbs;
using Dhgms.CloneAllRepos.Cmd.Settings;
using MediatR;

namespace Dhgms.CloneAllRepos.Cmd.RequestHandlers
{
    public sealed class CloneFromBitBucketRequestHandler : IRequestHandler<ICloneBitBucketJobSettings>
    {
        public Task Handle(ICloneBitBucketJobSettings message, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}

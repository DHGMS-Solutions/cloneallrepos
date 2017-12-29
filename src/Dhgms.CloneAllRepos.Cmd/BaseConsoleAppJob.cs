using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dhgms.CloneAllRepos.Cmd.Errands;
using Dhgms.CloneAllRepos.Cmd.Requests;
using MediatR;

namespace Dhgms.CloneAllRepos.Cmd
{
    public abstract class BaseConsoleAppJob<TActualJob> : IRequestHandler<StringArrayRequest<int>, int>
        where TActualJob : IRequestHandler<IJobSettings>
    {
        public async Task<int> Handle(string[] args)
        {
            var stringArrayRequest = new StringArrayRequest<int>(args);
            return await this.Handle(stringArrayRequest, CancellationToken.None);
        }

        public async Task<int> Handle(StringArrayRequest<int> args, CancellationToken cancellationToken)
        {
            var getJobSettingsErrand = new GetJobSettingsRequestHandler();

            IJobSettings jobSettings;

            try
            {
                jobSettings = await getJobSettingsErrand.Handle(new StringArrayRequest<IJobSettings>(args.Data), cancellationToken);
            }
#pragma warning disable CC0003 // Your catch should include an Exception
            catch
            {
                return 1;
            }
#pragma warning restore CC0003 // Your catch should include an Exception

            var jobHandler = this.GetActualJob();
            await jobHandler.Handle(jobSettings, cancellationToken);

            return 0;
        }

        protected abstract TActualJob GetActualJob();
    }
}

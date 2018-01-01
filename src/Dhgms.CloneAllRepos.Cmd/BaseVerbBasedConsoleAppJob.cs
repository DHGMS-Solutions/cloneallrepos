using Foundatio.AsyncEx.Synchronous;

namespace Dhgms.CloneAllRepos.Cmd
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using CommandLine;
    using Dhgms.CloneAllRepos.Cmd.CommandLineVerbs;
    using Dhgms.CloneAllRepos.Cmd.Requests;
    using MediatR;

    public abstract class BaseVerbBasedConsoleAppJob<T1Verb, T1Job, T2Verb, T2Job, T3Verb, T3Job> : IRequestHandler<StringArrayRequest<int>, int>
        where T1Job : IRequestHandler<T1Verb>
        where T1Verb : IRequest
        where T2Job : IRequestHandler<T2Verb>
        where T2Verb : IRequest
        where T3Job : IRequestHandler<T3Verb>
        where T3Verb : IRequest
    {
        public async Task<int> Handle(StringArrayRequest<int> request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(Parser.Default.ParseArguments<T1Verb, T2Verb, T3Verb>(request.Data)
                .MapResult(
                    (T1Verb opts) => RunJob(GetT1Job, opts, 2, cancellationToken),
                    (T2Verb opts) => RunJob(GetT2Job, opts, 3, cancellationToken),
                    (T3Verb opts) => RunJob(GetT3Job, opts, 4, cancellationToken),
                    _ => 1));
        }

        protected abstract T1Job GetT1Job();

        protected abstract T2Job GetT2Job();

        protected abstract T3Job GetT3Job();

        private int RunJob<TJob, TVerb>(
            Func<TJob> getJob,
            TVerb opts,
            int failureCode,
            CancellationToken cancellationToken)
            where TJob : IRequestHandler<TVerb>
            where TVerb : IRequest
        {
            try
            {
                var job = getJob();
                job.Handle(opts, cancellationToken).WaitAndUnwrapException(cancellationToken);
            }
            catch (Exception e)
            {
                return failureCode;
            }

            return 0;
        }
    }
}

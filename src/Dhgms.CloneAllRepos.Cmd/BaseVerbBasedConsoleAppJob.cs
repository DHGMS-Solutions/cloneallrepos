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

    public abstract class BaseVerbBasedConsoleAppJob<T1Verb, T1Job, T1Settings, T2Verb, T2Job, T2Settings, T3Verb, T3Job, T3Settings> : IRequestHandler<StringArrayRequest<int>, int>
        where T1Job : IRequestHandler<T1Settings>
        where T1Verb : T1Settings
        where T1Settings : IRequest
        where T2Job : IRequestHandler<T2Settings>
        where T2Verb : T2Settings
        where T2Settings : IRequest
        where T3Job : IRequestHandler<T3Settings>
        where T3Verb : T3Settings
        where T3Settings : IRequest
    {
        public async Task<int> Handle(string[] args)
        {
            var stringArrayRequest = new StringArrayRequest<int>(args);
            return await this.Handle(stringArrayRequest, CancellationToken.None);
        }

        public async Task<int> Handle(StringArrayRequest<int> request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(Parser.Default.ParseArguments<T1Verb, T2Verb, T3Verb>(request.Data)
                .MapResult(
                    (T1Verb opts) => this.RunJob<T1Job, T1Settings>(this.GetT1Job, opts, 2, cancellationToken),
                    (T2Verb opts) => this.RunJob<T2Job, T2Settings>(this.GetT2Job, opts, 3, cancellationToken),
                    (T3Verb opts) => this.RunJob<T3Job, T3Settings>(this.GetT3Job, opts, 4, cancellationToken),
                    _ => 1));
        }

        protected abstract T1Job GetT1Job(T1Settings settings);

        protected abstract T2Job GetT2Job(T2Settings settings);

        protected abstract T3Job GetT3Job(T3Settings settings);

        private int RunJob<TJob, TSettings>(
            Func<TSettings, TJob> getJob,
            TSettings opts,
            int failureCode,
            CancellationToken cancellationToken)
            where TJob : IRequestHandler<TSettings>
            where TSettings : IRequest
        {
            try
            {
                var job = getJob(opts);
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

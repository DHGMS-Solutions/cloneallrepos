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
        where T1Job : IRequestHandler<T1Verb, int>
        where T1Verb : IRequest<int>
        where T2Job : IRequestHandler<T2Verb, int>
        where T2Verb : IRequest<int>
        where T3Job : IRequestHandler<T3Verb, int>
        where T3Verb : IRequest<int>
    {
        public async Task<int> Handle(StringArrayRequest<int> request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(Parser.Default.ParseArguments<T1Verb, T2Verb, T3Verb>(request.Data)
                .MapResult(
                    (T1Verb opts) => GetT1Job().Handle(opts, cancellationToken).Result,
                    (T2Verb opts) => GetT2Job().Handle(opts, cancellationToken).Result,
                    (T3Verb opts) => GetT3Job().Handle(opts, cancellationToken).Result,
                    _ => 1));
        }

        protected abstract T1Job GetT1Job();

        protected abstract T2Job GetT2Job();

        protected abstract T3Job GetT3Job();
    }
}

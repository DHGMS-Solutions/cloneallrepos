using Foundatio.AsyncEx.Synchronous;
using Microsoft.Extensions.Logging;

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

    public abstract class BaseVerbBasedConsoleAppJob<T1Verb, T1Settings, T2Verb, T2Settings, T3Verb, T3Settings, TInheritingType> : IRequestHandler<StringArrayRequest<int>, int>
        where T1Verb : T1Settings
        where T1Settings : class, IRequest
        where T2Verb : T2Settings
        where T2Settings : class, IRequest
        where T3Verb : T3Settings
        where T3Settings : class, IRequest
        where TInheritingType : BaseVerbBasedConsoleAppJob<T1Verb, T1Settings, T2Verb, T2Settings, T3Verb, T3Settings, TInheritingType>
    {
        private readonly LoggerFactory _loggerFactory;

        protected BaseVerbBasedConsoleAppJob(LoggerFactory loggerFactory)
        {
            this._loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            this.Logger = this._loggerFactory.CreateLogger<TInheritingType>();
        }

        protected ILogger Logger { get; }

        public async Task<int> Handle(string[] args)
        {
            this.Logger.LogInformation($"Starting Job with Console Command Line: {Environment.CommandLine}");
            var stringArrayRequest = new StringArrayRequest<int>(args);
            var result = await this.Handle(stringArrayRequest, CancellationToken.None);
            this.Logger.LogInformation($"Finished Job. Result {result}");
            return result;
        }

        public async Task<int> Handle(StringArrayRequest<int> request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(Parser.Default.ParseArguments<T1Verb, T2Verb, T3Verb>(request.Data)
                .MapResult(
                    (T1Verb opts) => this.RunJob<IRequestHandler<T1Settings>, T1Settings>(this.GetT1Job, opts, 2, cancellationToken),
                    (T2Verb opts) => this.RunJob<IRequestHandler<T2Settings>, T2Settings>(this.GetT2Job, opts, 3, cancellationToken),
                    (T3Verb opts) => this.RunJob<IRequestHandler<T3Settings>, T3Settings>(this.GetT3Job, opts, 4, cancellationToken),
                    this.OnCommandLineArgumentErrors));
        }

        protected abstract IRequestHandler<T1Settings> GetT1Job(T1Settings settings);

        protected abstract IRequestHandler<T2Settings> GetT2Job(T2Settings settings);

        protected abstract IRequestHandler<T3Settings> GetT3Job(T3Settings settings);

        private int OnCommandLineArgumentErrors(IEnumerable<Error> arg)
        {
            this.Logger.LogError("Problem parsing command line");

            return 1;
        }

        private int RunJob<TJob, TSettings>(
            Func<TSettings, TJob> getJob,
            TSettings opts,
            int failureCode,
            CancellationToken cancellationToken)
            where TJob : IRequestHandler<TSettings>
            where TSettings : class, IRequest
        {
            try
            {
                if (getJob == null)
                {
                    throw new ArgumentNullException(nameof(getJob));
                }

                if (opts == null)
                {
                    throw new ArgumentNullException(nameof(opts));
                }

                this.Logger.LogInformation("Starting actual job");
                var job = getJob(opts);

                if (job == null)
                {
                    throw new InvalidOperationException("GetJob handler failed to return a job");
                }

                job.Handle(opts, cancellationToken).WaitAndUnwrapException(cancellationToken);
            }
            catch (Exception e)
            {
                this.Logger.LogError(e, "Job Run Failed");
                return failureCode;
            }

            return 0;
        }
    }
}
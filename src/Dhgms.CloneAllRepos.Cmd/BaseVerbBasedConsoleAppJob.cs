using System;
using System.CommandLine;
using System.Threading;
using System.Threading.Tasks;
using Dhgms.CloneAllRepos.Cmd.CommandLineVerbs;
using Dhgms.CloneAllRepos.Cmd.RequestHandlers;
using Dhgms.CloneAllRepos.Cmd.Requests;
using MediatR;
using Foundatio.AsyncEx.Synchronous;
using Microsoft.Extensions.Logging;
using Octokit;

namespace Dhgms.CloneAllRepos.Cmd
{

    public abstract class BaseVerbBasedConsoleAppJob<T1Verb, T1Settings, T2Verb, T2Settings, T3Verb, T3Settings, TInheritingType>
        : IRequestHandler<StringArrayRequest<int>, int>
        where T1Verb : T1Settings
        where T1Settings : class, IRequest
        where T2Verb : T2Settings
        where T2Settings : class, IRequest
        where T3Verb : T3Settings
        where T3Settings : class, IRequest
        where TInheritingType : BaseVerbBasedConsoleAppJob<T1Verb, T1Settings, T2Verb, T2Settings, T3Verb, T3Settings, TInheritingType>
    {
        protected BaseVerbBasedConsoleAppJob(LoggerFactory loggerFactory)
        {
            this.LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            this.Logger = this.LoggerFactory.CreateLogger<TInheritingType>();
        }

        protected LoggerFactory LoggerFactory { get; }

        protected ILogger Logger { get; }

        public async Task<int> Handle(StringArrayRequest<int> request, CancellationToken cancellationToken)
        {
            this.Logger.LogInformation($"Starting Job with Console Command Line: {Environment.CommandLine}");

            var apiKeyOption = new Option<string>("apiKey");
            var baseUrlOption = new Option<string>("baseUrlOption");
            var base64AuthTokenOption = new Option<string>("base64AuthTokenOption");
            var rootDirectory = new Option<string>("rootDir");
            var whatIf = new Option<bool>("whatIf");

            var bitbucketCommand = new Command("bitbucket", "Clones a Bitbucket Account")
            {
                baseUrlOption,
                base64AuthTokenOption,
                rootDirectory,
                whatIf
            };

            bitbucketCommand.SetHandler(
                async (BitBucketCommandLineVerb verb) =>
                {
                    await new CloneFromBitBucketRequestHandler(
                        this.LoggerFactory.CreateLogger<CloneFromBitBucketRequestHandler>(),
                        CloneAction).Handle(verb, CancellationToken.None);
                },
                new BitBucketCommandLineVerbBinder(
                    baseUrlOption,
                    base64AuthTokenOption,
                    rootDirectory,
                    whatIf));

            var githubCommand = new Command("github", "Clones a Github Account")
            {
                apiKeyOption,
                rootDirectory,
                whatIf
            };

            /*
            githubCommand.SetHandler(
                (GitHubCommandLineVerb verb) => { },
                new GithubCommandLineVerbBinder(
                    apiKeyOption,
                    rootDirectory,
                    whatIf));
            */

            var rootCommand = new RootCommand("Source Control Cloning Tool");
            rootCommand.AddCommand(bitbucketCommand);
            rootCommand.AddCommand(githubCommand);

            return await rootCommand.InvokeAsync(request.Data)
                .ConfigureAwait(false);
        }

        private Task CloneAction(Repository arg1, string arg2, ILogger arg3)
        {
            throw new NotImplementedException();
        }


        protected abstract IRequestHandler<T1Settings> GetT1Job(T1Settings settings);

        protected abstract IRequestHandler<T2Settings> GetT2Job(T2Settings settings);

        protected abstract IRequestHandler<T3Settings> GetT3Job(T3Settings settings);

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
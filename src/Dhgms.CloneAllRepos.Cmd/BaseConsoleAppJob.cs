//namespace Dhgms.CloneAllRepos.Cmd
//{
//    using System.Threading;
//    using System.Threading.Tasks;
//    using Dhgms.CloneAllRepos.Cmd.RequestHandlers;
//    using Dhgms.CloneAllRepos.Cmd.Requests;
//    using MediatR;

//    /// <summary>
//    /// Acts as an entry point for executing a job from a console application
//    /// </summary>
//    /// <typeparam name="TActualJob">The actual job that will be executed</typeparam>
//    public abstract class BaseConsoleAppJob<TActualJob> : IRequestHandler<StringArrayRequest<int>, int>
//        where TActualJob : IRequestHandler<IJobSettings>
//    {
//        public async Task<int> Handle(string[] args)
//        {
//            var stringArrayRequest = new StringArrayRequest<int>(args);
//            return await this.Handle(stringArrayRequest, CancellationToken.None);
//        }

//        public async Task<int> Handle(StringArrayRequest<int> args, CancellationToken cancellationToken)
//        {
//            var getJobSettingsErrand = new GetJobSettingsRequestHandler();

//            IJobSettings jobSettings;

//            try
//            {
//                jobSettings = await getJobSettingsErrand.Handle(new StringArrayRequest<IJobSettings>(args.Data), cancellationToken);
//            }
//            catch
//            {
//                return 1;
//            }

//            var jobHandler = this.GetActualJob();
//            await jobHandler.Handle(jobSettings, cancellationToken);

//            return 0;
//        }

//        protected abstract TActualJob GetActualJob();
//    }
//}

using System;
using MediatR;

namespace Dhgms.CloneAllRepos.Cmd
{
    public class FuncConsoleAppJob<TActualJob> : BaseConsoleAppJob<TActualJob>
        where TActualJob : IRequestHandler<IJobSettings>
    {
        private readonly Func<TActualJob> _actualJobFactory;

        public FuncConsoleAppJob(Func<TActualJob> actualJobFactory)
        {
            this._actualJobFactory = actualJobFactory ?? throw new ArgumentNullException(nameof(actualJobFactory));
        }

        protected override TActualJob GetActualJob() => this._actualJobFactory();
    }
}
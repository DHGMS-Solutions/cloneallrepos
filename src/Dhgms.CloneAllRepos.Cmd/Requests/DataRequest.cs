using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Dhgms.CloneAllRepos.Cmd.Requests
{
    public class DataRequest<TData, TResponse> : IRequest<TResponse>
        where TData : class
    {
        public DataRequest(TData data)
        {
            this.Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public TData Data { get; }
    }
}

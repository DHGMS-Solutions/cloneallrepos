using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Dhgms.CloneAllRepos.Cmd.Requests
{
    public sealed class StringArrayRequest<TResponse> : DataRequest<string[], TResponse>
    {
        public StringArrayRequest(string[] data)
            : base(data)
        {
        }
    }
}

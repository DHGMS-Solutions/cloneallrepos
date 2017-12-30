using System;
using System.Threading;
using System.Threading.Tasks;
using SystemWrapper.IO;
using Dhgms.CloneAllRepos.Cmd.Requests;
using MediatR;

namespace Dhgms.CloneAllRepos.Cmd
{
    public sealed class ConsoleAppJob : BaseConsoleAppJob<CloneFromGithubRequestHandler>
    {
        protected override CloneFromGithubRequestHandler GetActualJob()
        {
            var directory = new DirectoryWrap();
            return new CloneFromGithubRequestHandler(directory);
        }
    }
}

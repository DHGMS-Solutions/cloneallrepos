using Dhgms.CloneAllRepos.Cmd.RequestHandlers;

namespace Dhgms.CloneAllRepos.Cmd
{
    using SystemWrapper.IO;

    public sealed class ConsoleAppJob : BaseConsoleAppJob<CloneFromGithubRequestHandler>
    {
        protected override CloneFromGithubRequestHandler GetActualJob()
        {
            var directory = new DirectoryWrap();
            var pathSystem = new PathWrap();
            return new CloneFromGithubRequestHandler(directory, pathSystem);
        }
    }
}

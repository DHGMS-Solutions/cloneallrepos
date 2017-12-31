using Dhgms.CloneAllRepos.Cmd.CommandLineVerbs;
using Dhgms.CloneAllRepos.Cmd.RequestHandlers;

namespace Dhgms.CloneAllRepos.Cmd
{
    using SystemWrapper.IO;

    public sealed class ConsoleAppJob : BaseVerbBasedConsoleAppJob<BitBucketCommandLineVerb, CloneFromBitBucketRequestHandler, GitHubCommandLineVerb, CloneFromGithubRequestHandler, TeamFoundationServerCommandLineVerb, CloneFromTeamFoundationServerRequestHandler>
    {
        protected override CloneFromBitBucketRequestHandler GetT1Job(BitBucketCommandLineVerb opts)
        {
            throw new System.NotImplementedException();
        }

        protected override CloneFromGithubRequestHandler GetT2Job(GitHubCommandLineVerb opts)
        {
            var directory = new DirectoryWrap();
            var pathSystem = new PathWrap();
            return new CloneFromGithubRequestHandler(directory, pathSystem);
        }

        protected override CloneFromTeamFoundationServerRequestHandler GetT3Job(TeamFoundationServerCommandLineVerb opts)
        {
            throw new System.NotImplementedException();
        }
    }
}

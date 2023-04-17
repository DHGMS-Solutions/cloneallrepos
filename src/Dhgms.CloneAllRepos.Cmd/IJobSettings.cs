using MediatR;

namespace Dhgms.CloneAllRepos.Cmd
{
    public interface IJobSettings : IRequest
    {
        //string GitHubApiKey { get; }

        string RootDirectory { get; }

        bool WhatIf { get; }
    }
}
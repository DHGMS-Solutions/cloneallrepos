using MediatR;

namespace Dhgms.CloneAllRepos.Cmd
{
    public interface IJobSettings : IRequest
    {
        //string GitHubApiKey { get; }

        string RootDir { get; }

        bool WhatIf { get; }
    }
}
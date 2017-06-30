namespace Dhgms.CloneAllRepos.Cmd
{
    public interface IJobSettings
    {
        string ApiKey { get; }

        string RootDir { get; }

        bool WhatIf { get; }
    }
}
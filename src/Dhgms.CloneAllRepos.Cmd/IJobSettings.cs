namespace Dhgms.CloneAllRepos.Cmd
{
    public interface IJobSettings
    {
        string ApiKey { get; }

        bool WhatIf { get; }
    }
}
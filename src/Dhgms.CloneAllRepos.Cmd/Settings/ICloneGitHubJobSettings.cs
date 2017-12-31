namespace Dhgms.CloneAllRepos.Cmd.Settings
{
    public interface ICloneGitHubJobSettings : IJobSettings
    {
        string ApiKey { get; }
    }
}
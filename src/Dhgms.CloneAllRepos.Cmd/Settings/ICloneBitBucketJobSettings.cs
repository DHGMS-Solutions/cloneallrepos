namespace Dhgms.CloneAllRepos.Cmd.Settings
{
    public interface ICloneBitBucketJobSettings : IJobSettings
    {
        string BaseUrl { get; }

        string Base64AuthToken { get; }
    }
}
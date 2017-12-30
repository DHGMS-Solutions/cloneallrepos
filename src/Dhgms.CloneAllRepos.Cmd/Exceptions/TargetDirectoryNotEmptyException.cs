using System;

namespace Dhgms.CloneAllRepos.Cmd.Exceptions
{
    public class TargetDirectoryNotEmptyException : Exception
    {
        public TargetDirectoryNotEmptyException(string targetDirectory)
            : base(targetDirectory)
        {
        }
    }
}
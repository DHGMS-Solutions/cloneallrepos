using System;
using StoryTeller;

namespace Dhgms.CloneAllRepos.StoryTeller.Cmd
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // If you do not need a custom ISystem
            StorytellerAgent.Run(args);
        }
    }
}

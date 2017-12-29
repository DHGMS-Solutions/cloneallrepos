namespace Dhgms.CloneAllRepos.UnitTests
{
    using Dhgms.CloneAllRepos.Cmd;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public static class CommandLineArgumentsTest
    {
        public sealed class ParseArgumentsMethod
        {
            public static string testApiKey = "TESTAPIKEY";

            public static IEnumerable<object[]> ApiKeyTestData => new List<object[]>
            {
                new object[]
                {
                    "k",
                    testApiKey
                },

                new object[]
                {
                    "apikey",
                    testApiKey
                }
            };

            public static IEnumerable<object[]> WhatIfTestData => new List<object[]>
            {
                new object[]
                {
                    "w"
                },

                new object[]
                {
                    "whatif"
                }
            };

            [Theory]
            [MemberData(nameof(ApiKeyTestData))]
            public void ApiKey(string[] args)
            {
                var commandLineArgs = new CommandLineArguments();
                var success = CommandLine.Parser.Default.ParseArguments(args, commandLineArgs);

                Assert.True(success);
                Assert.Equal(testApiKey, commandLineArgs.GitHubApiKey);
                Assert.False(commandLineArgs.WhatIf);
            }

            [Theory]
            [MemberData(nameof(WhatIfTestData))]
            public void WhatIfSpecified(string[] args)
            {
                var commandLineArgs = new CommandLineArguments();
                var success = CommandLine.Parser.Default.ParseArguments(args, commandLineArgs);

                Assert.True(success);
                Assert.Null(commandLineArgs.GitHubApiKey);
                Assert.False(commandLineArgs.WhatIf);
            }
        }
    }
}

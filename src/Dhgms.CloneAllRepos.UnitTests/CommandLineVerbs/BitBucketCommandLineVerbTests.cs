using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using Dhgms.CloneAllRepos.Cmd.CommandLineVerbs;
using Xunit;

namespace Dhgms.CloneAllRepos.UnitTests.CommandLineVerbs
{
    public class BitBucketCommandLineVerbTests
    {
        public static IEnumerable<object[]> ParseTestData => new List<object[]>
        {
            new object[]
            {
                new []
                {
                    "bitbucket",
                },
                new BitBucketCommandLineVerb
                {
                    ApiKey = "testapikey",
                    RootDir = "c:\\git\\bitbucket",
                    WhatIf = false,
                },
                0,
            },

            new object[]
            {
                new[]
                {
                    "bitbucket",
                    "--whatif",
                },
                new BitBucketCommandLineVerb
                {
                    WhatIf = true,
                },
                1,
            },
        };

        [Theory]
        [MemberData(nameof(ParseTestData))]
        public void Parse(
            string[] args,
            BitBucketCommandLineVerb expectedResult,
            int expectedMapResult)
        {
            var parserResult = Parser.Default.ParseArguments<BitBucketCommandLineVerb>(args);
            var mapResult = parserResult.MapResult(
                actual => CheckParsedArgs(expectedResult, actual),
                _ => 1);
            Assert.Equal(expectedMapResult, mapResult);
        }

        private int CheckParsedArgs(BitBucketCommandLineVerb expected, BitBucketCommandLineVerb actual)
        {
            Assert.Equal(expected.ApiKey, actual.ApiKey);
            Assert.Equal(expected.RootDir, actual.RootDir);
            Assert.Equal(expected.WhatIf, actual.WhatIf);

            return 0;
        }
    }
}

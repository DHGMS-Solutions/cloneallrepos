#if TBC
using System.Collections.Generic;
using System.CommandLine.Parsing;
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
                    "--apikey testapikey",
                    "--rootdir c:\\git\\bitbucket",
                },
                new BitBucketCommandLineVerb("testapikey", "c:\\git\\bitbucket", false),
                0,
            },

            new object[]
            {
                new[]
                {
                    "bitbucket",
                    "--whatif",
                },
                new BitBucketCommandLineVerb(null, null, true),
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
            var parserResult = Parser..Default.ParseArguments<BitBucketCommandLineVerb>(args);
            var mapResult = parserResult.MapResult(
                actual => CheckParsedArgs(expectedResult, actual),
                _ => 1);
            Assert.Equal(expectedMapResult, mapResult);
        }

        private int CheckParsedArgs(BitBucketCommandLineVerb expected, BitBucketCommandLineVerb actual)
        {
            Assert.Equal(expected.ApiKey, actual.ApiKey);
            Assert.Equal(expected.RootDirectory, actual.RootDirectory);
            Assert.Equal(expected.WhatIf, actual.WhatIf);

            return 0;
        }
    }
}
#endif
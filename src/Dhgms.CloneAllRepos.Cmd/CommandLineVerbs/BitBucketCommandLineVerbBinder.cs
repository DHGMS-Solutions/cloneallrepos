using System;
using System.CommandLine;
using System.CommandLine.Binding;

namespace Dhgms.CloneAllRepos.Cmd.CommandLineVerbs
{
    public sealed class BitBucketCommandLineVerbBinder : BinderBase<BitBucketCommandLineVerb>
    {
        private readonly Option<string> _baseUrlOption;
        private readonly Option<string> _base64AuthTokenOption;
        private readonly Option<string> _rootDirectoryOption;
        private readonly Option<bool> _whatIfOption;

        public BitBucketCommandLineVerbBinder(
            Option<string> baseUrlOption,
            Option<string> base64AuthTokenOption,
            Option<string> rootDirectoryOption,
            Option<bool> whatIfOption)
        {
            ArgumentNullException.ThrowIfNull(baseUrlOption);
            ArgumentNullException.ThrowIfNull(rootDirectoryOption);
            ArgumentNullException.ThrowIfNull(whatIfOption);

            _baseUrlOption = baseUrlOption;
            _base64AuthTokenOption = base64AuthTokenOption;
            _rootDirectoryOption = rootDirectoryOption;
            _whatIfOption = whatIfOption;
        }

        protected override BitBucketCommandLineVerb GetBoundValue(BindingContext bindingContext)
        {
            var baseUrl = bindingContext.ParseResult.GetValueForOption(_baseUrlOption);
            var base64AuthToken = bindingContext.ParseResult.GetValueForOption(_base64AuthTokenOption);
            var rootDirectory = bindingContext.ParseResult.GetValueForOption(_rootDirectoryOption);
            var whatIf = bindingContext.ParseResult.GetValueForOption(_whatIfOption);

            return new BitBucketCommandLineVerb(
                baseUrl,
                base64AuthToken,
                rootDirectory,
                whatIf);
        }
    }
}

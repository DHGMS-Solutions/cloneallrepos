using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dhgms.CloneAllRepos.Cmd.Requests;
using MediatR;

namespace Dhgms.CloneAllRepos.Cmd.Errands
{
    public class GetJobSettingsRequestHandler : IRequestHandler<StringArrayRequest<IJobSettings>, IJobSettings>
    {
        public async Task<IJobSettings> Handle(StringArrayRequest<IJobSettings> message, CancellationToken cancellationToken)
        {
            return await Task.Run(() => GetJobSettings(message), cancellationToken);
        }

        private static IJobSettings GetJobSettings(StringArrayRequest<IJobSettings> message)
        {
            var commandLineArgs = new CommandLineArguments();

            if (!CommandLine.Parser.Default.ParseArgumentsStrict(message.Data, commandLineArgs))
            {
                throw new ArgumentException("unable to parse arguments", nameof(message));
            }

            return commandLineArgs;
        }
    }
}

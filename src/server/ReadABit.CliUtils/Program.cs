using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using Microsoft.Extensions.Hosting;
using System.CommandLine.Parsing;
using System.Threading.Tasks;
using ReadABit.CliUtils.Commands;

namespace ReadABit.CliUtils
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            return await BuildCommandLine()
                .UseHost(
                    _ => Host.CreateDefaultBuilder(),
                    host =>
                    {
                        host.ConfigureServices(services =>
                        {
                            // services.Add...
                        });
                    }
                )
                .UseDefaults()
                .Build()
                .InvokeAsync(args);
        }

        private static CommandLineBuilder BuildCommandLine()
        {
            var certCommand = new Command("cert");

            var root = new RootCommand
            {
                certCommand,
            };

            certCommand.Handler = CommandHandler.Create<IHost>(CertCommandHandler.Handle);
            return new CommandLineBuilder(root);
        }
    }
}

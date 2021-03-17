using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using Microsoft.Extensions.Hosting;
using System.CommandLine.Parsing;
using System.Threading.Tasks;
using ReadABit.CliUtils.Commands;
using ReadABit.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Reflection;

namespace ReadABit.CliUtils
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            return await BuildCommandLine()
                .UseHost(
                    _ => Host.CreateDefaultBuilder()
                        .ConfigureWebHostDefaults(webBuilder =>
                        {
                            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                            var config = new ConfigurationBuilder()
                                     .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location))
                                     .AddJsonFile("appsettings.json")
                                     .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                                     .AddUserSecrets<Startup>()
                                     .Build();
                            webBuilder.UseConfiguration(config);
                            webBuilder.UseStartup<Startup>();
                        }),
                    host => { }
                )
                .UseDefaults()
                .Build()
                .InvokeAsync(args);
        }

        private static CommandLineBuilder BuildCommandLine()
        {
            var certCommand = new Command("cert");
            var seedCommand = new Command("seed") {
                new Option(new string[] { "-f", "--force" }, "Override all existing entires. May cause data loss.")
            };

            var root = new RootCommand
            {
                certCommand,
                seedCommand,
            };

            certCommand.Handler = CommandHandler.Create(CertCommandHandler.Handle);
            seedCommand.Handler = CommandHandler.Create<bool, IHost>(SeedCommandHandler.Handle);
            return new CommandLineBuilder(root);
        }
    }
}

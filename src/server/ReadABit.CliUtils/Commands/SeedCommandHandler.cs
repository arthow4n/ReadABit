using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ReadABit.CliUtils.Commands
{
    public static class SeedCommandHandler
    {
        public static void Handle(IHost host)
        {
            // TODO: Move the content of OpenIddictWorker to here.

            var configuration = host.Services.GetRequiredService<IConfiguration>();
        }
    }
}

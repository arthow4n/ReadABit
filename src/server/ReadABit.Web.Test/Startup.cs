using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReadABit.Infrastructure;

namespace ReadABit.Web.Test
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<Startup>()
                .Build();

            services.AddDbContext<CoreDbContext>(
                    options => options.UseNpgsql(configuration.GetConnectionString("CoreDbContext"),
                    x => x.MigrationsAssembly("ReadABit.Infrastructure")
                ));

            services.AddMediatR(typeof(Startup));
        }
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReadABit.Core.Services.Utils;
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

            // TODO: Think about how to better deal with test DB
            services
                .BuildServiceProvider()
                .GetRequiredService<CoreDbContext>()
                .Database
                .Migrate();

            services.AddMediatR(typeof(ServiceBase));
        }
    }
}

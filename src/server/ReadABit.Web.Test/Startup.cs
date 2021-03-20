using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Npgsql;
using ReadABit.Core.Commands;
using ReadABit.Core.Utils;
using ReadABit.Infrastructure;
using ReadABit.Infrastructure.Models;
using ReadABit.Web.Test.Helpers;

namespace ReadABit.Web.Test
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<Startup>()
                .Build();

            // TODO: Consider replacing this with a fake clock
            services.AddSingleton<IClock>(SystemClock.Instance);

            NpgsqlConnection.GlobalTypeMapper.UseNodaTime();
            services.AddDbContext<UnsafeCoreDbContext>(
                options => options.UseNpgsql(
                    configuration.GetConnectionString("CoreDbContext"),
                    x => x.UseNodaTime()
                          .MigrationsAssembly("ReadABit.Infrastructure")
                )
            );
            services
                .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                })
                .AddEntityFrameworkStores<UnsafeCoreDbContext>()
                .AddDefaultTokenProviders();

            var db = services
                .BuildServiceProvider()
                .GetRequiredService<UnsafeCoreDbContext>()
                .Database;
            db.Migrate();

            services.AddScoped<IRequestContext, RequestContextMock>();
            services.AddScoped<DB, DB>();

            services.AddMediatR(typeof(SaveChanges));
            services.AddAutoMapper(typeof(SaveChanges));
        }
    }
}

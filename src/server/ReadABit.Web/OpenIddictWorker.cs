using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ReadABit.Web
{
    // Mostly resembles https://raw.githubusercontent.com/openiddict/openiddict-samples/dev/samples/Velusia/Velusia.Server/Worker.cs
    public class OpenIddictWorker : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public OpenIddictWorker(IServiceProvider serviceProvider, IConfiguration configuration, IWebHostEnvironment env)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _env = env;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            // FIXME: Find a better way to seed this in production
            if (_env.IsDevelopment())
            {
                var existing = await manager.FindByClientIdAsync("ReadABit");
                if (existing is not null)
                {
                    await manager.DeleteAsync(existing);
                }
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "ReadABit",
                    Type = ClientTypes.Public,
                    ConsentType = ConsentTypes.Explicit,
                    DisplayName = "ReadABit public client",
                    PostLogoutRedirectUris =
                    {
                        _configuration.GetValue<Uri>("OpenIddictWorker:ReadABit:PostLogoutRedirectUri"),
                    },
                    RedirectUris =
                    {
                        _configuration.GetValue<Uri>("OpenIddictWorker:ReadABit:RedirectUri"),
                    },
                    Permissions =
                    {
                        Permissions.Endpoints.Authorization,
                        Permissions.Endpoints.Logout,
                        Permissions.Endpoints.Token,
                        Permissions.GrantTypes.AuthorizationCode,
                        Permissions.GrantTypes.RefreshToken,
                        Permissions.ResponseTypes.Code,
                        Permissions.Scopes.Email,
                        Permissions.Scopes.Profile,
                        Permissions.Scopes.Roles,
                    },
                    Requirements =
                    {
                        Requirements.Features.ProofKeyForCodeExchange,
                    },
                });
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}

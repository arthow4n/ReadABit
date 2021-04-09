using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ReadABit.CliUtils.Commands
{
    public static class SeedCommandHandler
    {

        public static async Task Handle(bool force, IHost host)
        {
            using var scope = host.Services.CreateScope();
            var configuration = host.Services.GetRequiredService<IConfiguration>();

            var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
            var existing = await manager.FindByClientIdAsync("ReadABit");

            if (existing is not null)
            {
                if (force is not true)
                {
                    throw new InvalidOperationException(
@"Existing OpenIddict client entry found.
Run the command with --force if you really want to recreate it.
Recreating the client entry will immediately invalidate all issued access/refresh tokens.");
                }

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
                        new Uri("com.readabit.client.auth://"),
                        new Uri("com.readabit.client://"),
                        new Uri("readabit://"),
                    },
                RedirectUris =
                    {
                        new Uri("com.readabit.client.auth://"),
                        new Uri("com.readabit.client://"),
                        new Uri("readabit://"),
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
}

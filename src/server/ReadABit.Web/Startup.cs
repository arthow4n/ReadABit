using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ReadABit.Infrastructure;
using Microsoft.EntityFrameworkCore;
using MediatR;
using ReadABit.Core.Services.Utils;
using ReadABit.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using System;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ReadABit.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _env;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CoreDbContext>(
                options =>
                {
                    options.UseNpgsql(
                        Configuration.GetConnectionString("CoreDbContext"),
                        x => x.MigrationsAssembly("ReadABit.Infrastructure")
                    );
                    options.UseOpenIddict<Guid>();
                }
            );

            services
                .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    // TODO: Enable maybe in production, it depends.
                    options.SignIn.RequireConfirmedAccount = false;
                    // Use passphases please
                    options.Password.RequireDigit = false;
                    options.Password.RequiredUniqueChars = 0;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 0;

                    // Ref: https://github.com/openiddict/openiddict-samples/blob/dev/samples/Velusia/Velusia.Server/Startup.cs
                    options.ClaimsIdentity.UserNameClaimType = Claims.Name;
                    options.ClaimsIdentity.UserIdClaimType = Claims.Subject;
                    options.ClaimsIdentity.RoleClaimType = Claims.Role;
                })
                .AddEntityFrameworkStores<CoreDbContext>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();
            services.AddRazorPages();

            services
                .AddOpenIddict()
                .AddCore(options =>
                {
                    options
                        .UseEntityFrameworkCore()
                        .UseDbContext<CoreDbContext>()
                        .ReplaceDefaultEntities<Guid>();
                })
                .AddServer(options =>
                {
                    options
                        .SetAuthorizationEndpointUris("/connect/authorize")
                        .SetLogoutEndpointUris("/connect/logout")
                        .SetTokenEndpointUris("/connect/token")
                        .SetUserinfoEndpointUris("/connect/userinfo");
                    options.RegisterScopes(
                        Scopes.OpenId,
                        Scopes.Email,
                        Scopes.Profile,
                        Scopes.OfflineAccess,
                        Scopes.Roles
                    );
                    options
                        .AllowAuthorizationCodeFlow()
                        // Required by scope: offline_access
                        // https://github.com/openiddict/openiddict-core/issues/835
                        .AllowRefreshTokenFlow();

                    // FIXME: Fix this so it works in production
                    options
                        .AddDevelopmentEncryptionCertificate()
                        .AddDevelopmentSigningCertificate();

                    var aspOptions =
                        options
                            .UseAspNetCore()
                            .EnableAuthorizationEndpointPassthrough()
                            .EnableLogoutEndpointPassthrough()
                            .EnableTokenEndpointPassthrough()
                            .EnableUserinfoEndpointPassthrough()
                            .EnableStatusCodePagesIntegration();

                    if (_env.IsDevelopment())
                    {
                        aspOptions.DisableTransportSecurityRequirement();
                    }

                    // Enfore OIDC Authorization Code Flow with PKCE since the clients are going to be native app or SPA
                    options.RequireProofKeyForCodeExchange();

                    // TODO: Fix logout so it's possible to revoke tokens.
                    options.UseReferenceAccessTokens();
                    options.UseReferenceRefreshTokens();
                })
                .AddValidation(options =>
                {
                    options.UseLocalServer();
                    options.UseAspNetCore();
                    options.EnableAuthorizationEntryValidation();
                    options.EnableTokenEntryValidation();
                });
            services.AddHostedService<OpenIddictWorker>();

            services
                .AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                });
            services.AddHttpContextAccessor();

            services.AddMediatR(typeof(ServiceBase));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ReadABit.Web", Version = "v1" });
            });
            services.AddSwaggerDocument();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseOpenApi();
                app.UseSwaggerUi3();
            }

            if (!env.IsDevelopment())
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action}/{id?}"
                );
                endpoints.MapRazorPages();
            });
        }
    }
}

﻿using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using Npgsql;
using ReadABit.Core.Commands;
using ReadABit.Core.Contracts.Utils;
using ReadABit.Core.Integrations.SparvPipelineProxy;
using ReadABit.Core.Utils;
using ReadABit.Infrastructure;
using ReadABit.Infrastructure.Models;
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
            services.AddSingleton<IClock>(SystemClock.Instance);

            NpgsqlConnection.GlobalTypeMapper.UseNodaTime();
            services.AddDbContext<UnsafeCoreDbContext>(
                options =>
                {
                    options.UseNpgsql(
                        Configuration.GetConnectionString("CoreDbContext"),
                        x => x.UseNodaTime()
                              .MigrationsAssembly("ReadABit.Infrastructure")
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
                .AddEntityFrameworkStores<UnsafeCoreDbContext>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();
            services.AddRazorPages();

            services
                .AddOpenIddict()
                .AddCore(options =>
                {
                    options
                        .UseEntityFrameworkCore()
                        .UseDbContext<UnsafeCoreDbContext>()
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

                    options
                        .AddEncryptionCertificate(new X509Certificate2(Convert.FromBase64String(Configuration["Certificates:OpenIddictEncryption"]), string.Empty))
                        .AddSigningCertificate(new X509Certificate2(Convert.FromBase64String(Configuration["Certificates:OpenIddictSigning"]), string.Empty));

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

            services
                .AddControllers()
                .AddFluentValidation(x =>
                {
                    x.RegisterValidatorsFromAssemblyContaining<SaveChanges>();
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                    options.SerializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
                });
            services.AddHttpContextAccessor();
            services.AddScoped<IRequestContext, RequestContext>();
            services.AddScoped<DB, DB>();
            services.AddSingleton((serviceProvider) =>
                new SparvPipelineProxyClient(
                    Configuration["Integrations:SparvPipelineProxy:BaseUrl"],
                    new HttpClient
                    {
                        // Never timeout because sparv-pipeline can run quite slow
                        // when given large input especially on a machine that's not that performant.
                        Timeout = Timeout.InfiniteTimeSpan
                    }
                )
            );

            services.AddMediatR(typeof(SaveChanges));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ReadABit.Web", Version = "v1" });
            });
            services.AddSwaggerDocument();

            services.AddAutoMapper(typeof(ViewModelMapperProfile));
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

            var supportedCultures = new[] { "en", "zh-TW" };

            app.UseRequestLocalization(
                new RequestLocalizationOptions
                {
                    ApplyCurrentCultureToResponseHeaders = true,
                }
                    .SetDefaultCulture(supportedCultures[0])
                    .AddSupportedCultures(supportedCultures)
                    .AddSupportedUICultures(supportedCultures)
            );

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

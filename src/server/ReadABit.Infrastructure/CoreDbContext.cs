using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Infrastructure
{
    public class CoreDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public CoreDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        public DbSet<Article> Articles => Set<Article>();

    }
}

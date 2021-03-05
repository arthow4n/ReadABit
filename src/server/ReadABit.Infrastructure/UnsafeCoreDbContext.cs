using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Infrastructure
{
    /// <summary>
    /// The root DbContext. Avoid using this when possible since it's more easy to forget filtering the query by user when directly using this.
    /// </summary>
    public class UnsafeCoreDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public UnsafeCoreDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Article> Articles => Set<Article>();
        public DbSet<ArticleCollection> ArticleCollections => Set<ArticleCollection>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}

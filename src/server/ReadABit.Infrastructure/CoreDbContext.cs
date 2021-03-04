using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Infrastructure
{
    public class CoreDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public CoreDbContext(DbContextOptions options) : base(options)
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

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
        public DbSet<UserPreference> UserPreferences => Set<UserPreference>();
        public DbSet<Word> Words => Set<Word>();
        public DbSet<WordDefinition> WordDefinitions => Set<WordDefinition>();
        public DbSet<WordFamiliarity> WordFamiliarities => Set<WordFamiliarity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<ArticleCollection>()
                .HasIndex(ac => new
                {
                    ac.LanguageCode,
                    ac.Name,
                });

            modelBuilder
                .Entity<Word>()
                .HasIndex(w => new
                {
                    w.LanguageCode,
                    w.Expression,
                })
                .IsUnique();

            modelBuilder
                .Entity<WordDefinition>()
                .HasIndex(wd => wd.LanguageCode);

            modelBuilder
                .Entity<WordDefinition>()
                .HasIndex(wd => new
                {
                    wd.UserId,
                    wd.WordId,
                });

            modelBuilder
                .Entity<WordFamiliarity>()
                .HasIndex(wf => new
                {
                    wf.UserId,
                    wf.WordId,
                })
                .IsUnique();

            modelBuilder
                .Entity<UserPreference>()
                .HasIndex(up => up.UserId)
                .IsUnique();
        }
    }
}

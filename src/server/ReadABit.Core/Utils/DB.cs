using System;
using System.Linq;
using ReadABit.Infrastructure;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Utils
{
    public class DB
    {
        private readonly UnsafeCoreDbContext _coreDbContext;

        public DB(UnsafeCoreDbContext coreDbContext)
        {
            _coreDbContext = coreDbContext;
        }

        public UnsafeCoreDbContext Unsafe => _coreDbContext;

        public IQueryable<Article> ArticlesOfUser(Guid userId)
        {
            return _coreDbContext.Articles.Where(a => a.ArticleCollection.UserId == userId);
        }
        public IQueryable<Article> ArticlesOfUserOrPublic(Guid userId)
        {
            return _coreDbContext.Articles.Where(a => a.ArticleCollection.Public || a.ArticleCollection.UserId == userId);
        }
        public IQueryable<ArticleCollection> ArticleCollectionsOfUser(Guid userId)
        {
            return _coreDbContext.ArticleCollections.Where(ac => ac.UserId == userId);
        }
        public IQueryable<ArticleCollection> ArticleCollectionsOfUserOrPublic(Guid userId)
        {
            return _coreDbContext.ArticleCollections.Where(ac => ac.Public || ac.UserId == userId);
        }
        public IQueryable<WordDefinition> WordDefinitionsOfUser(Guid userId)
        {
            return _coreDbContext.WordDefinitions.Where(wd => wd.UserId == userId);
        }
        public IQueryable<WordDefinition> WordDefinitionsOfUserOrPublic(Guid userId)
        {
            return _coreDbContext.WordDefinitions.Where(wd => wd.Public || wd.UserId == userId);
        }
        public IQueryable<WordFamiliarity> WordFamiliaritiesOfUser(Guid userId)
        {
            return _coreDbContext.WordFamiliarities.Where(wf => wf.UserId == userId);
        }
        public IQueryable<Word> Words => _coreDbContext.Words;
        public IQueryable<UserPreference> UserPreferencesOfUser(Guid userId)
        {
            return _coreDbContext.UserPreferences.Where(up => up.UserId == userId);
        }
        public IQueryable<ArticleReadingProgress> ArticleReadingProgressOfUser(Guid userId)
        {
            return _coreDbContext.ArticleReadingProgress.Where(arp => arp.UserId == userId);
        }
    }
}

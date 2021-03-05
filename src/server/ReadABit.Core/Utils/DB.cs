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

        public IQueryable<ArticleCollection> ArticleCollectionOfUser(Guid userId)
        {
            return _coreDbContext.ArticleCollections.Where(a => a.UserId == userId);
        }
    }
}

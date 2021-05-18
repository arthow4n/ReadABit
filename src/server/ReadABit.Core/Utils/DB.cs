using System;
using System.Linq;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using ReadABit.Infrastructure;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Utils
{
    public class DB
    {
        public static TransactionScope TransactionScope()
        {
            return new TransactionScope(asyncFlowOption: TransactionScopeAsyncFlowOption.Enabled);
        }

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
        public IQueryable<UserAchievement> UserAchievementsOfUser(Guid userId)
        {
            return _coreDbContext.UserAchievements.Where(up => up.UserId == userId);
        }
        public IQueryable<UserAchievementStreak> UserAchievementStreaksOfUser(Guid userId, UserAchievementType type)
        {
            return _coreDbContext.UserAchievementStreaks
                .FromSqlInterpolated(
                $@"        
                    with
                    streaks as (
                        select
                            distinct ua.""CreatedAt""::date,
                            ""CreatedAt""::date - make_interval(days:= (dense_rank() over(order by ""CreatedAt""::date))::int) as ""StreakGroup""
                        from ""UserAchievements"" ua
                        where ua.""UserId"" = {userId}
                        and ua.""Type"" = {type}
                    )
                    select max(""CreatedAt"") as ""LastUtcDateInStreak"", COUNT(*) as ""StreakDays""
                    from streaks
                    group by ""StreakGroup""
                ");
        }
        public IQueryable<ArticleReadingProgress> ArticleReadingProgressOfUser(Guid userId)
        {
            return _coreDbContext.ArticleReadingProgress.Where(arp => arp.UserId == userId);
        }
    }
}

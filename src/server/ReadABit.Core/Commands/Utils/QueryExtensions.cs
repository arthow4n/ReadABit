using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using ReadABit.Infrastructure.Interfaces;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public static class QueryExtensions
    {
        public static IQueryable<T> OfWord<T>(this IQueryable<T> query, WordSelector selector) where T : IOfWord
        {
            return query
                .Where(wd =>
                    wd.Word.LanguageCode == selector.LanguageCode &&
                    wd.Word.Expression == selector.Expression
                );
        }
        public static IQueryable<Word> OfWord(this IQueryable<Word> query, WordSelector selector)
        {
            return query
                .Where(w =>
                    w.LanguageCode == selector.LanguageCode &&
                    w.Expression == selector.Expression
                );
        }
        public static IQueryable<Word> OfWords(this IQueryable<Word> query, ICollection<WordSelector> selectors)
        {
            var predicate = PredicateBuilder.New<Word>();

            foreach (var rw in selectors)
            {
                var languageCode = rw.LanguageCode;
                var expression = rw.Expression;
                predicate = predicate.Or(w =>
                    w.LanguageCode == languageCode &&
                    w.Expression == expression
                );
            }

            return query
                .AsExpandableEFCore()
                .Where(predicate);
        }
        public static IQueryable<T> OfWords<T>(this IQueryable<T> query, ICollection<WordSelector> selectors) where T : IOfWord
        {
            var predicate = PredicateBuilder.New<T>();

            foreach (var rw in selectors)
            {
                var languageCode = rw.LanguageCode;
                var expression = rw.Expression;
                predicate = predicate.Or(w =>
                    w.Word.LanguageCode == languageCode &&
                    w.Word.Expression == expression
                );
            }

            return query
                .AsExpandableEFCore()
                .Where(predicate);
        }
        public static Task<Guid> IdOfWord(this IQueryable<Word> query, WordSelector selector, CancellationToken cancellationToken)
        {
            return query.OfWord(selector)
                        .Select(w => w.Id)
                        .SingleOrDefaultAsync(cancellationToken: cancellationToken);
        }
        public static IQueryable<T> Page<T>(this IQueryable<T> query, PageFilterFilled filter)
        {
            return query.Skip((filter.Index - 1) * filter.Size).Take(filter.Size);
        }

        public static Paginated<T> ToPaginated<T>(this IQueryable<T> query, PageFilter filter, int defaultPageSize)
        {
            var totalCount = query.Count();
            var currentFilter = filter.Fill(defaultPageSize);
            var items = query.Page(currentFilter).ToList();
            return CreatePaginatedResult(items, currentFilter, totalCount);
        }

        public static async Task<Paginated<T>> ToPaginatedAsync<T>(this IQueryable<T> query, PageFilter filter, int defaultPageSize, CancellationToken cancellationToken = default)
        {
            var totalCount = await query.CountAsync(cancellationToken);
            var currentFilter = filter.Fill(defaultPageSize);
            var items = await query.Page(currentFilter).ToListAsync(cancellationToken);
            return CreatePaginatedResult(items, currentFilter, totalCount);
        }

        public static Paginated<T> CreatePaginatedResult<T>(List<T> items, PageFilterFilled currentFilter, int totalCount)
        {
            var totalPages = decimal.ToInt32(Math.Ceiling((decimal)totalCount / currentFilter.Size));

            return new Paginated<T>
            {
                Items = items,
                Page = new PageInfo
                {
                    Current = currentFilter,
                    Next = currentFilter.Index >= totalPages ? null : currentFilter with { Index = currentFilter.Index + 1 },
                    Previous = currentFilter.Index < 2 ? null : currentFilter with { Index = currentFilter.Index - 1 },
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                },
            };
        }

        public static IQueryable<T> SortBy<T>(this IQueryable<T> query, SortBy sortBy) where T : ITimestampedEntity
        {
            return sortBy switch
            {
                Commands.SortBy.LastUpdated => query.OrderByDescending(x => x.UpdatedAt),
                Commands.SortBy.LastCreated => query.OrderByDescending(x => x.CreatedAt),
                Commands.SortBy.CreatedAt => query.OrderByDescending(x => x.CreatedAt),
                _ => throw new ArgumentOutOfRangeException(nameof(sortBy), $"Sorting by {sortBy} is not implemented for {typeof(T).FullName}"),
            };
        }

        public static IQueryable<Article> SortBy(this IQueryable<Article> query, SortBy sortBy, Guid userId)
        {
            return sortBy switch
            {
                Commands.SortBy.LastAccessed =>
                    query.OrderByDescending(
                        x => x.ArticleReadingProgress
                            .Where(arp => arp.UserId == userId)
                            .Select(arp => arp.UpdatedAt)
                            .FirstOrDefault()
                    ),
                _ => SortBy(query, sortBy)
            };
        }
    }
}

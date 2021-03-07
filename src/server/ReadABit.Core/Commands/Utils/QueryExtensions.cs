using System.Linq;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public static class QueryExtensions
    {
        public static IQueryable<WordDefinition> OfWord(this IQueryable<WordDefinition> query, WordSelector selector)
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
    }
}

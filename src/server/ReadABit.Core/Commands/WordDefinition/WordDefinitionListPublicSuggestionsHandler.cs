using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReadABit.Infrastructure.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ReadABit.Core.Utils;
using System.Collections.Generic;
using FluentValidation;

namespace ReadABit.Core.Commands
{
    public class WordDefinitionListPublicSuggestionsHandler : IRequestHandler<WordDefinitionListPublicSuggestions, Paginated<WordDefinitionListPublicSuggestionViewModel>>
    {
        private readonly DB _db;

        public WordDefinitionListPublicSuggestionsHandler(DB db)
        {
            _db = db;
        }

        public async Task<Paginated<WordDefinitionListPublicSuggestionViewModel>> Handle(WordDefinitionListPublicSuggestions request, CancellationToken cancellationToken)
        {
            new WordDefinitionListPublicSuggestionsValidator().ValidateAndThrow(request);

            var tobePaginated = (await _db.WordDefinitionsOfUserOrPublic(request.UserId)
                                          .OfWord(request.Filter.Word)
                                          .Select(wd => new
                                          {
                                              wd.LanguageCode,
                                              wd.Meaning,
                                          })
                                          // FIXME: When EF Core fixes this https://github.com/dotnet/efcore/issues/12088
                                          .ToListAsync(cancellationToken: cancellationToken))
                                          .GroupBy(wd => new
                                          {
                                              wd.LanguageCode,
                                              wd.Meaning,
                                          })
                                          .Select(wdg => new WordDefinitionListPublicSuggestionViewModel
                                          {
                                              LanguageCode = wdg.First().LanguageCode,
                                              Meaning = wdg.First().Meaning,
                                              Count = wdg.Count(),
                                          });

            var userPreferredLanguageCode =
                await _db.UserPreferencesOfUser(request.UserId)
                         .Where(up => up.Type == UserPreferenceType.LanguageCode)
                         .Select(up => up.Value)
                         .SingleOrDefaultAsync(cancellationToken: cancellationToken);

            return tobePaginated.OrderByDescending(vm => vm.LanguageCode == userPreferredLanguageCode ? 1 : 0)
                                .ThenByDescending(vm => vm.Count)
                                .AsQueryable()
                                .ToPaginated(request.Page, 50);
        }
    }
}

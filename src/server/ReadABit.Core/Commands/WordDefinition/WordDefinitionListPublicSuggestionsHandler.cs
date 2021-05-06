using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReadABit.Core.Commands.Utils;
using ReadABit.Core.Utils;

namespace ReadABit.Core.Commands
{
    public class WordDefinitionListPublicSuggestionsHandler : CommandHandlerBase, IRequestHandler<WordDefinitionListPublicSuggestions, Paginated<WordDefinitionListPublicSuggestionViewModel>>
    {
        public WordDefinitionListPublicSuggestionsHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<Paginated<WordDefinitionListPublicSuggestionViewModel>> Handle(WordDefinitionListPublicSuggestions request, CancellationToken cancellationToken)
        {
            new WordDefinitionListPublicSuggestionsValidator().ValidateAndThrow(request);

            var tobePaginated = (await DB.WordDefinitionsOfUserOrPublic(request.UserId)
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

            return tobePaginated.OrderByDescending(vm => vm.LanguageCode == request.Filter.PreferredLanguageCode ? 1 : 0)
                                .ThenByDescending(vm => vm.Count)
                                .AsQueryable()
                                .ToPaginated(request.Page, 50);
        }
    }
}

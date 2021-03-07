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
    public class WordDefinitionListHandler : IRequestHandler<WordDefinitionList, List<WordDefinition>>
    {
        private readonly DB _db;

        public WordDefinitionListHandler(DB db)
        {
            _db = db;
        }

        public async Task<List<WordDefinition>> Handle(WordDefinitionList request, CancellationToken cancellationToken)
        {
            new WordDefinitionListValidator().ValidateAndThrow(request);

            return (await _db.WordDefinitionsOfUserOrPublic(request.UserId)
                            .AsNoTracking()
                            .OfWord(request.Filter.Word)
                            // FIXME: Try to avoid loading all entities into memory.
                            .ToListAsync(cancellationToken: cancellationToken))
                            .GroupBy(
                                wd => new
                                {
                                    // TODO: Sort the result by LanguageCode to match user preference.
                                    wd.LanguageCode,
                                    wd.Meaning,
                                }
                            )
                            .OrderByDescending(wdg => wdg.Count())
                            .Take(5)
                            .SelectMany(wdg => wdg)
                            .ToList();
        }
    }
}

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReadABit.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using ReadABit.Core.Utils;
using FluentValidation;

namespace ReadABit.Core.Commands
{
    public class WordDefinitionListHandler : IRequestHandler<WordDefinitionList, Paginated<WordDefinition>>
    {
        private readonly DB _db;

        public WordDefinitionListHandler(DB db)
        {
            _db = db;
        }

        public async Task<Paginated<WordDefinition>> Handle(WordDefinitionList request, CancellationToken cancellationToken)
        {
            new WordDefinitionListValidator().ValidateAndThrow(request);

            return await _db.WordDefinitionsOfUser(request.UserId)
                            .AsNoTracking()
                            .OfWord(request.Filter.Word)
                            .ToPaginatedAsync(request.Page, 50, cancellationToken);
        }
    }
}

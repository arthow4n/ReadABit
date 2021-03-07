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

            return await _db.WordDefinitionsOfUser(request.UserId)
                            .AsNoTracking()
                            .OfWord(request.Filter.Word)
                            .ToListAsync(cancellationToken: cancellationToken);
        }
    }
}

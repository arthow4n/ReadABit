using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReadABit.Core.Commands.Utils;
using ReadABit.Core.Utils;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public class WordDefinitionListHandler : CommandHandlerBase, IRequestHandler<WordDefinitionList, Paginated<WordDefinition>>
    {
        public WordDefinitionListHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<Paginated<WordDefinition>> Handle(WordDefinitionList request, CancellationToken cancellationToken)
        {
            new WordDefinitionListValidator().ValidateAndThrow(request);

            return await DB.WordDefinitionsOfUser(request.UserId)
                            .AsNoTracking()
                            .OfWord(request.Filter.Word)
                            .ToPaginatedAsync(request.Page, 50, cancellationToken);
        }
    }
}

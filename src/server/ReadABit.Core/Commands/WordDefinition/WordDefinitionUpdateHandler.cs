using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using ReadABit.Core.Commands.Utils;
using ReadABit.Core.Utils;

namespace ReadABit.Core.Commands
{
    public class WordDefinitionUpdateHandler : CommandHandlerBase, IRequestHandler<WordDefinitionUpdate, bool>
    {
        public WordDefinitionUpdateHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<bool> Handle(WordDefinitionUpdate request, CancellationToken cancellationToken)
        {
            new WordDefinitionUpdateValidator().ValidateAndThrow(request);

            var wordDefinition = await DB.WordDefinitionsOfUser(request.UserId)
                                   .Where(a => a.Id == request.Id)
                                   .SingleOrDefaultAsync(cancellationToken: cancellationToken);

            if (wordDefinition is null)
            {
                return false;
            }

            wordDefinition.Public = request.Public ?? wordDefinition.Public;
            wordDefinition.LanguageCode = request.LanguageCode ?? wordDefinition.LanguageCode;
            wordDefinition.Meaning = request.Meaning is not null ? request.Meaning.Trim() : wordDefinition.Meaning;
            wordDefinition.UpdatedAt = Clock.GetCurrentInstant();

            return true;
        }
    }
}

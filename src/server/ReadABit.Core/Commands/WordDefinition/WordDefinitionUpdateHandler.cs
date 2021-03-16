using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReadABit.Core.Utils;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using NodaTime;

namespace ReadABit.Core.Commands
{
    public class WordDefinitionUpdateHandler : IRequestHandler<WordDefinitionUpdate, bool>
    {
        private readonly DB _db;
        private readonly IClock _clock;

        public WordDefinitionUpdateHandler(DB db, IClock clock)
        {
            _db = db;
            _clock = clock;
        }

        public async Task<bool> Handle(WordDefinitionUpdate request, CancellationToken cancellationToken)
        {
            new WordDefinitionUpdateValidator().ValidateAndThrow(request);

            var wordDefinition = await _db.WordDefinitionsOfUser(request.UserId)
                                   .Where(a => a.Id == request.Id)
                                   .SingleOrDefaultAsync(cancellationToken: cancellationToken);

            if (wordDefinition is null)
            {
                return false;
            }

            wordDefinition.Public = request.Public ?? wordDefinition.Public;
            wordDefinition.LanguageCode = request.LanguageCode ?? wordDefinition.LanguageCode;
            wordDefinition.Meaning = request.Meaning is not null ? request.Meaning.Trim() : wordDefinition.Meaning;
            wordDefinition.UpdatedAt = _clock.GetCurrentInstant();

            return true;
        }
    }
}

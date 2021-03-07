using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReadABit.Core.Utils;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace ReadABit.Core.Commands
{
    public class WordDefinitionUpdateHandler : IRequestHandler<WordDefinitionUpdate, bool>
    {
        private readonly DB _db;

        public WordDefinitionUpdateHandler(DB db)
        {
            _db = db;
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

            return true;
        }
    }
}

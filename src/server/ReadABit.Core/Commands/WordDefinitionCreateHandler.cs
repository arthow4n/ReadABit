using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReadABit.Infrastructure.Models;
using ReadABit.Core.Utils;
using FluentValidation;
using NodaTime;

namespace ReadABit.Core.Commands
{
    public class WordDefinitionCreateHandler : IRequestHandler<WordDefinitionCreate, WordDefinition>
    {
        private readonly DB _db;
        private readonly IClock _clock;

        public WordDefinitionCreateHandler(DB db, IClock clock)
        {
            _db = db;
            _clock = clock;
        }

        public async Task<WordDefinition> Handle(WordDefinitionCreate request, CancellationToken cancellationToken)
        {
            new WordDefinitionCreateValidator().ValidateAndThrow(request);

            var wordDefinition = new WordDefinition
            {
                Id = Guid.NewGuid(),
                WordId = request.WordId,
                UserId = request.UserId,
                Public = request.Public,
                LanguageCode = request.LanguageCode,
                Meaning = request.Meaning.Trim(),
                CreatedAt = _clock.GetCurrentInstant(),
            };

            await _db.Unsafe.AddAsync(wordDefinition, cancellationToken);

            return wordDefinition;
        }
    }
}

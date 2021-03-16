using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReadABit.Infrastructure.Models;
using ReadABit.Core.Utils;
using FluentValidation;
using NodaTime;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Npgsql;

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

            Task<Guid> GetWordId()
            {
                return _db.Words
                          .OfWord(request.Word)
                          .Select(w => w.Id)
                          .SingleOrDefaultAsync(cancellationToken: cancellationToken);
            }

            var wordId = await GetWordId();

            if (wordId == default)
            {
                try
                {
                    var newWord = new Word
                    {
                        Id = Guid.NewGuid(),
                        LanguageCode = request.Word.LanguageCode,
                        Expression = request.Word.Expression,
                    };
                    await _db.Unsafe.Words.AddAsync(newWord, cancellationToken);
                    await _db.Unsafe.SaveChangesAsync(cancellationToken);
                    wordId = newWord.Id;
                }
                catch (PostgresException ex)
                {
                    if (ex.SqlState == PostgresErrorCodes.UniqueViolation)
                    {
                        wordId = await GetWordId();
                    }
                }
            }

            var wordDefinition = new WordDefinition
            {
                Id = Guid.NewGuid(),
                WordId = wordId,
                UserId = request.UserId,
                Public = request.Public,
                LanguageCode = request.LanguageCode,
                Meaning = request.Meaning.Trim(),
                CreatedAt = _clock.GetCurrentInstant(),
                UpdatedAt = _clock.GetCurrentInstant(),
            };

            await _db.Unsafe.AddAsync(wordDefinition, cancellationToken);

            return wordDefinition;
        }
    }
}

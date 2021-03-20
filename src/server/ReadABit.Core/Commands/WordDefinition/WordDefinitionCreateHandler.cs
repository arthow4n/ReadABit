using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using ReadABit.Core.Commands.Utils;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public class WordDefinitionCreateHandler : CommandHandlerBase, IRequestHandler<WordDefinitionCreate, WordDefinition>
    {
        public WordDefinitionCreateHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<WordDefinition> Handle(WordDefinitionCreate request, CancellationToken cancellationToken)
        {
            new WordDefinitionCreateValidator().ValidateAndThrow(request);

            Task<Guid> GetWordId()
            {
                return DB.Words
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
                    await DB.Unsafe.Words.AddAsync(newWord, cancellationToken);
                    await DB.Unsafe.SaveChangesAsync(cancellationToken);
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
                CreatedAt = Clock.GetCurrentInstant(),
                UpdatedAt = Clock.GetCurrentInstant(),
            };

            await DB.Unsafe.AddAsync(wordDefinition, cancellationToken);

            return wordDefinition;
        }
    }
}

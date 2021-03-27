using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Npgsql;
using ReadABit.Core.Utils;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public record WordSelector
    {
        [BindRequired]
        public string LanguageCode { get; init; } = "";
        [BindRequired]
        public string Expression { get; init; } = "";

        public Task<Guid> GetId(DB db, CancellationToken cancellationToken)
        {
            return db.Words.IdOfWord(this, cancellationToken);
        }

        public async Task<Guid> GetIdAndEnsureCreated(DB db, CancellationToken cancellationToken)
        {
            var wordId = await GetId(db, cancellationToken);

            if (wordId == default)
            {
                try
                {
                    var newWord = new Word
                    {
                        Id = Guid.NewGuid(),
                        LanguageCode = LanguageCode,
                        Expression = Expression,
                    };
                    await db.Unsafe.Words.AddAsync(newWord, cancellationToken);
                    await db.Unsafe.SaveChangesAsync(cancellationToken);
                    wordId = newWord.Id;
                }
                catch (PostgresException ex)
                {
                    if (ex.SqlState == PostgresErrorCodes.UniqueViolation)
                    {
                        wordId = await GetId(db, cancellationToken);
                    }
                }
            }

            return wordId;
        }
    }

    public class WordSelectorValidator : AbstractValidator<WordSelector>
    {
        public WordSelectorValidator()
        {
            RuleFor(x => x.LanguageCode).MustBeValidLanguageCode();
            RuleFor(x => x.Expression).NotEmpty();
        }
    }
}

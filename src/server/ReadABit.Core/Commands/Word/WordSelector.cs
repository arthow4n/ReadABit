using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
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

            return await EnsureCreated(db, wordId, cancellationToken);
        }

        public async Task<Guid> EnsureCreated(DB db, Guid wordId, CancellationToken cancellationToken)
        {
            if (wordId == default)
            {
                var newWordId = Guid.NewGuid();

                await db.Unsafe.Words
                    .Upsert(new Word
                    {
                        Id = newWordId,
                        LanguageCode = LanguageCode,
                        Expression = Expression,
                    })
                    .On(w => new
                    {
                        w.LanguageCode,
                        w.Expression,
                    })
                    .RunAsync(cancellationToken);

                return newWordId;
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

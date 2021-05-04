using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
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

        public async Task<Guid> GetIdAndEnsureCreated(DB db, IMapper mapper, CancellationToken cancellationToken)
        {
            var wordId = await GetId(db, cancellationToken);

            return await EnsureCreated(db, mapper, wordId, cancellationToken);
        }

        public async Task<Guid> EnsureCreated(DB db, IMapper mapper, Guid wordId, CancellationToken cancellationToken)
        {
            if (wordId != default)
            {
                return wordId;
            }

            var word = mapper.Map<WordSelector, Word>(this);
            word.Id = Guid.NewGuid();

            await db.Unsafe.Words
                .Upsert(word)
                .On(w => new
                {
                    w.LanguageCode,
                    w.Expression,
                })
                .RunAsync(cancellationToken);

            return word.Id;
        }

        public static async Task EnsureWordsCreated(DB db, IMapper mapper, List<WordSelector> words, CancellationToken cancellationToken)
        {
            await db.Unsafe.Words
                .UpsertRange(
                    mapper
                        .Map<List<WordSelector>, List<Word>>(words)
                        .ConvertAll(w =>
                        {
                            w.Id = Guid.NewGuid();
                            return w;
                        })
                )
                .On(w => new
                {
                    w.LanguageCode,
                    w.Expression,
                })
                .WhenMatched((existing, updated) => new Word
                {
                    Id = existing.Id,
                    LanguageCode = existing.LanguageCode,
                    Expression = existing.Expression,
                })
                .RunAsync(cancellationToken);
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

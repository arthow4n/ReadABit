using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReadABit.Core.Commands.Utils;
using ReadABit.Core.Utils;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public class WordFamiliarityUpsertBatchHandler : CommandHandlerBase, IRequestHandler<WordFamiliarityUpsertBatch, bool>
    {
        public WordFamiliarityUpsertBatchHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<bool> Handle(WordFamiliarityUpsertBatch request, CancellationToken cancellationToken)
        {
            new WordFamiliarityUpsertBatchValidator().ValidateAndThrow(request);

            await DB.Unsafe.Words
                .UpsertRange(
                    Mapper
                        .Map<List<WordSelector>, List<Word>>(request.Words)
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
                .RunAsync(cancellationToken);

            await DB.Unsafe.SaveChangesAsync(cancellationToken);

            var predicate = PredicateBuilder.New<Word>();

            foreach (var rw in request.Words)
            {
                var languageCode = rw.LanguageCode;
                var expression = rw.Expression;
                predicate = predicate.Or(w =>
                    w.LanguageCode == languageCode &&
                    w.Expression == expression
                );
            }

            var wordFamiliarities = await DB.Unsafe.Words
                .AsExpandableEFCore()
                .Where(predicate)
                .Select(w => new WordFamiliarity
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    Level = request.Level,
                    WordId = w.Id,
                })
                .ToListAsync(cancellationToken);

            await DB.Unsafe.WordFamiliarities
                .UpsertRange(wordFamiliarities)
                .On(wf => new
                {
                    wf.UserId,
                    wf.WordId,
                })
                .WhenMatched((existing, updated) => new WordFamiliarity
                {
                    Level = updated.Level,
                })
                .RunAsync(cancellationToken);

            return true;
        }
    }
}

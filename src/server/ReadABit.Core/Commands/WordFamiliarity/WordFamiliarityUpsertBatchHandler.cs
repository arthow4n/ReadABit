using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using NodaTime;
using ReadABit.Core.Commands.Utils;
using ReadABit.Core.Utils;
using ReadABit.Infrastructure.Models;
using Z.EntityFramework.Plus;

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

            await WordSelector.EnsureWordsCreated(DB, Mapper, request.Words, cancellationToken);

            await DB.Unsafe.SaveChangesAsync(cancellationToken);

            if (request.Level == 0)
            {
                // Not sure why but this doesn't run.
                //    System.ArgumentException : Field '_queryCompiler' defined on type 'Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryProvider' is not a field on the target object which is of type 'LinqKit.ExpandableQueryProvider`1[ReadABit.Infrastructure.Models.WordFamiliarity]'.
                // await DB.WordFamiliaritiesOfUser(request.UserId)
                //     .OfWords(request.Words)
                //     .DeleteAsync(cancellationToken);

                var wordFamiliarityIds = await DB.WordFamiliaritiesOfUser(request.UserId)
                    .OfWords(request.Words)
                    .Select(wf => wf.Id)
                    .ToListAsync(cancellationToken);

                await DB.WordFamiliaritiesOfUser(request.UserId)
                    .Where(wf => wordFamiliarityIds.Contains(wf.Id))
                    .DeleteAsync(cancellationToken);

                return true;
            }

            await UpsertWordFamiliarity(request, cancellationToken);

            return true;
        }

        private async Task UpsertWordFamiliarity(WordFamiliarityUpsertBatch request, CancellationToken cancellationToken)
        {
            var now = Clock.GetCurrentInstant();

            var wordFamiliarities = (await DB.Unsafe.Words
                .OfWords(request.Words)
                .Select(w => new WordFamiliarity
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    Level = request.Level,
                    WordId = w.Id,
                    CreatedAt = now,
                    UpdatedAt = now,
                })
                .ToListAsync(cancellationToken))
                .DistinctBy(w => w.Id);

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
                    CreatedAt = existing.CreatedAt,
                    UpdatedAt = updated.UpdatedAt,
                })
                .RunAsync(cancellationToken);

            // TODO: Update today's user achievement state
        }
    }
}

﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReadABit.Core.Commands.Utils;
using ReadABit.Core.Utils;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public class ArticleReadingProgressUpsertHandler : CommandHandlerBase, IRequestHandler<ArticleReadingProgressUpsert, bool>
    {
        public ArticleReadingProgressUpsertHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<bool> Handle(ArticleReadingProgressUpsert request, CancellationToken cancellationToken)
        {
            new ArticleReadingProgressUpdateValidator().ValidateAndThrow(request);

            var now = Clock.GetCurrentInstant();

            await DB.Unsafe.ArticleReadingProgress
                .Upsert(new()
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    ArticleId = request.ArticleId,
                    ConlluTokenPointer = request.ConlluTokenPointer,
                    ReadRatio = request.ReadRatio,
                    CreatedAt = now,
                    UpdatedAt = now,
                })
                .On(x => new
                {
                    x.UserId,
                    x.ArticleId,
                })
                .WhenMatched((existing, updated) => new()
                {
                    Id = existing.Id,
                    UserId = existing.UserId,
                    ArticleId = existing.ArticleId,
                    ConlluTokenPointer = updated.ConlluTokenPointer,
                    ReadRatio = updated.ReadRatio,
                    CreatedAt = existing.CreatedAt,
                    UpdatedAt = updated.UpdatedAt,
                })
                .RunAsync(cancellationToken);

            return true;
        }
    }
}

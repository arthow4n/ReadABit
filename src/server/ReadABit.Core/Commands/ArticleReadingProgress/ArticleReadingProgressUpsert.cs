using System;
using FluentValidation;
using MediatR;
using Newtonsoft.Json;
using NSwag.Annotations;
using ReadABit.Core.Integrations.Contracts.Conllu;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public record ArticleReadingProgressUpsert : IRequest<bool>
    {
        [OpenApiIgnore, JsonIgnore]
        public Guid UserId { get; init; }
        [OpenApiIgnore, JsonIgnore]
        public Guid ArticleId { get; init; }
        public Conllu.TokenPointer ConlluTokenPointer { get; init; } = new();
        public decimal ReadRatio { get; init; }
    }

    public class ArticleReadingProgressUpdateValidator : AbstractValidator<ArticleReadingProgressUpsert>
    {
        public ArticleReadingProgressUpdateValidator()
        {
            RuleFor(x => x.ReadRatio).InclusiveBetween(0, 1);
            // Not validating collu token pointer mainly because it could become out of sync because of e.g. article update,
            // and the consumer of that object should handler the case where a search with pointer misses.
        }
    }
}

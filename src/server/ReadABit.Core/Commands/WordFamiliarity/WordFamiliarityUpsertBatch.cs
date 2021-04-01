using System;
using System.Collections.Generic;
using FluentValidation;
using MediatR;
using Newtonsoft.Json;
using NSwag.Annotations;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public record WordFamiliarityUpsertBatch : IRequest<bool>
    {
        [OpenApiIgnore, JsonIgnore]
        public Guid UserId { get; init; }
        public int Level { get; set; }
        public List<WordSelector> Words { get; init; } = new();
    }

    public class WordFamiliarityUpsertBatchValidator : AbstractValidator<WordFamiliarityUpsertBatch>
    {
        public WordFamiliarityUpsertBatchValidator()
        {
            RuleForEach(x => x.Words).MustBeValidWordSelector();
            RuleFor(x => x.Level).InclusiveBetween(WordFamiliarity.LevelIgnored, WordFamiliarity.LevelMax);
        }
    }
}

using System;
using FluentValidation;
using MediatR;
using Newtonsoft.Json;
using NSwag.Annotations;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public record WordFamiliarityUpsert : IRequest<bool>
    {
        [OpenApiIgnore, JsonIgnore]
        public Guid UserId { get; init; }
        public WordSelector Word { get; init; } = new();
        public int Level { get; set; }
    }

    public class WordFamiliarityUpdateValidator : AbstractValidator<WordFamiliarityUpsert>
    {
        public WordFamiliarityUpdateValidator()
        {
            RuleFor(x => x.Word).MustBeValidWordSelector();
            RuleFor(x => x.Level).InclusiveBetween(WordFamiliarity.LevelIgnored, WordFamiliarity.LevelMax);
        }
    }
}

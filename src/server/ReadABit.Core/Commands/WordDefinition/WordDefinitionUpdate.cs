using System;
using FluentValidation;
using MediatR;
using Newtonsoft.Json;
using NSwag.Annotations;

namespace ReadABit.Core.Commands
{
    public record WordDefinitionUpdate : IRequest<bool>
    {
        [OpenApiIgnore, JsonIgnore]
        public Guid UserId { get; init; }
        [OpenApiIgnore, JsonIgnore]
        public Guid Id { get; init; }
        public bool? Public { get; init; }
        public string? LanguageCode { get; init; }
        public string? Meaning { get; init; }
    }

    public class WordDefinitionUpdateValidator : AbstractValidator<WordDefinitionUpdate>
    {
        public WordDefinitionUpdateValidator()
        {
            RuleFor(x => x.LanguageCode!).MustBeValidLanguageCode().When(x => x.LanguageCode is not null);
            RuleFor(x => x.Meaning).NotEmpty().When(x => x.Meaning is not null);
        }
    }
}

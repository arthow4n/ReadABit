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
        public Guid UserId { get; set; }
        public Guid Id { get; set; }
        public bool? Public { get; set; }
        public string? LanguageCode { get; set; }
        public string? Meaning { get; set; }
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

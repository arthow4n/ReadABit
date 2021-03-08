using System;
using FluentValidation;
using MediatR;
using Newtonsoft.Json;
using NSwag.Annotations;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public record WordDefinitionCreate : IRequest<WordDefinition>
    {
        [OpenApiIgnore, JsonIgnore]
        public Guid UserId { get; init; }
        public WordSelector Word { get; init; } = new WordSelector { };
        public bool Public { get; init; }
        public string LanguageCode { get; init; } = "";
        public string Meaning { get; init; } = "";
    }

    public class WordDefinitionCreateValidator : AbstractValidator<WordDefinitionCreate>
    {
        public WordDefinitionCreateValidator()
        {
            RuleFor(x => x.LanguageCode).MustBeValidLanguageCode();
            RuleFor(x => x.Meaning).NotEmpty();
            RuleFor(x => x.Word).MustBeValidWordSelector();
        }
    }
}

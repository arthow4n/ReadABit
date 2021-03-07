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
        public Guid UserId { get; set; }
        public WordSelector Word { get; set; } = new WordSelector { };
        public bool Public { get; set; }
        public string LanguageCode { get; set; } = "";
        public string Meaning { get; set; } = "";
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

using System;
using FluentValidation;
using MediatR;
using Newtonsoft.Json;
using NSwag.Annotations;

namespace ReadABit.Core.Commands
{
    public record WordDefinitionListPublicSuggestions : IPaginatedRequest, IRequest<Paginated<WordDefinitionListPublicSuggestionViewModel>>
    {
        [OpenApiIgnore, JsonIgnore]
        public Guid UserId { get; set; }
        public WordDefinitionListPublicSuggestionsFilter Filter { get; set; } = new();
        public PageFilter Page { get; set; } = new();
    }

    public class WordDefinitionListPublicSuggestionsValidator : AbstractValidator<WordDefinitionListPublicSuggestions>
    {
        public WordDefinitionListPublicSuggestionsValidator()
        {
            RuleFor(x => x.Filter.Word).MustBeValidWordSelector();
        }
    }

    public record WordDefinitionListPublicSuggestionViewModel
    {
        public string LanguageCode { get; set; } = "";
        public string Meaning { get; set; } = "";
        /// <summary>
        /// How many users use this word definition for the word.
        /// </summary>
        public int Count { get; set; }
    }

    public record WordDefinitionListPublicSuggestionsFilter
    {
        public WordSelector Word { get; set; } = new();
    }
}

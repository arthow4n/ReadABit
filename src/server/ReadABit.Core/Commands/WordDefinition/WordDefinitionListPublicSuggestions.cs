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
        public Guid UserId { get; init; }
        public WordDefinitionListPublicSuggestionsFilter Filter { get; init; } = new();
        public PageFilter Page { get; init; } = new();
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
        public string LanguageCode { get; init; } = "";
        public string Meaning { get; init; } = "";
        /// <summary>
        /// How many users use this word definition for the word.
        /// </summary>
        public int Count { get; init; }
    }

    public record WordDefinitionListPublicSuggestionsFilter
    {
        public WordSelector Word { get; init; } = new();
    }
}

using System;
using FluentValidation;
using MediatR;
using Newtonsoft.Json;
using NSwag.Annotations;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    // This doesn't need its own view model because it's not as useful.
    public record WordDefinitionList : IPaginatedRequest, IRequest<Paginated<WordDefinition>>
    {
        [OpenApiIgnore, JsonIgnore]
        public Guid UserId { get; init; }
        public WordDefinitionListFilter Filter { get; init; } = new();
        public PageFilter Page { get; init; } = new();
    }

    public class WordDefinitionListValidator : AbstractValidator<WordDefinitionList>
    {
        public WordDefinitionListValidator()
        {
            RuleFor(x => x.Filter.Word).MustBeValidWordSelector();
            RuleFor(x => x.Page).MustBeValidPageFilter();
        }
    }

    public record WordDefinitionListFilter
    {
        public WordSelector Word { get; init; } = new();
    }
}

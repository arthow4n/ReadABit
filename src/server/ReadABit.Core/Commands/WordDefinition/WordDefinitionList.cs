using System;
using System.Collections.Generic;
using FluentValidation;
using MediatR;
using Newtonsoft.Json;
using NSwag.Annotations;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public record WordDefinitionList : IRequest<List<WordDefinition>>
    {
        [OpenApiIgnore, JsonIgnore]
        public Guid UserId { get; set; }
        public WordDefinitionListFilter Filter { get; set; } = new WordDefinitionListFilter { };
    }

    public class WordDefinitionListValidator : AbstractValidator<WordDefinitionList>
    {
        public WordDefinitionListValidator()
        {
            RuleFor(x => x.Filter.Word).MustBeValidWordSelector();
        }
    }

    public record WordDefinitionListFilter
    {
        /// <summary>
        /// When true, search the top few popular public word definitions.
        /// When false, get a list of word definitions owned by requesting user.
        /// </summary>
        public bool Public { get; set; }

        public WordSelector Word { get; set; } = new WordSelector { };
    }
}

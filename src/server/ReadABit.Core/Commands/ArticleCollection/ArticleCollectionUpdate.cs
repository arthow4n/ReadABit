using System;
using MediatR;
using FluentValidation;
using NSwag.Annotations;
using Newtonsoft.Json;

namespace ReadABit.Core.Commands
{
    public record ArticleCollectionUpdate : IRequest<bool>
    {
        [OpenApiIgnore, JsonIgnore]
        public Guid Id { get; init; }
        [OpenApiIgnore, JsonIgnore]
        public Guid UserId { get; init; }
        public string? Name { get; init; }
        public string? LanguageCode { get; init; }
        public bool? Public { get; init; }
    }

    public class ArticleCollectionUpdateValidator : AbstractValidator<ArticleCollectionUpdate>
    {
        public ArticleCollectionUpdateValidator()
        {
            RuleFor(x => x.Name).NotEmpty().When(x => x.Name is not null);
            RuleFor(x => x.LanguageCode!).MustBeValidLanguageCode().When(x => x.LanguageCode is not null);
        }
    }
}

using System;
using FluentValidation;
using MediatR;
using Newtonsoft.Json;
using NSwag.Annotations;
using ReadABit.Core.Contracts;

namespace ReadABit.Core.Commands
{
    public record ArticleCollectionCreate : IRequest<ArticleCollectionViewModel>
    {
        [OpenApiIgnore, JsonIgnore]
        public Guid UserId { get; init; }
        public string Name { get; init; } = "";
        public string LanguageCode { get; init; } = "";
        public bool Public { get; init; }
    }

    public class ArticleCollectionCreateValidator : AbstractValidator<ArticleCollectionCreate>
    {
        public ArticleCollectionCreateValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.LanguageCode).MustBeValidLanguageCode();
        }
    }
}

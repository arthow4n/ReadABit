using System;
using FluentValidation;
using ReadABit.Infrastructure.Models;
using MediatR;
using NSwag.Annotations;
using Newtonsoft.Json;

namespace ReadABit.Core.Commands
{
    public record ArticleCollectionCreate : IRequest<ArticleCollection>
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

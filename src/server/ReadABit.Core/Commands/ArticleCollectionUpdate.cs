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
        public Guid Id { get; set; }
        [OpenApiIgnore, JsonIgnore]
        public Guid UserId { get; set; }
        public string Name { get; set; } = "";
        public string LanguageCode { get; set; } = "";
        public bool Public { get; set; }
    }

    public class ArticleCollectionUpdateValidator : AbstractValidator<ArticleCollectionUpdate>
    {
        public ArticleCollectionUpdateValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.LanguageCode).MustBeValidLanguageCode();
        }
    }
}

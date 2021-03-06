using System;
using FluentValidation;
using Newtonsoft.Json;
using ReadABit.Infrastructure.Models;
using MediatR;

namespace ReadABit.Core.Commands
{
    public record ArticleCollectionCreate : IRequest<ArticleCollection>
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
        public string Name { get; set; } = "";
        public string LanguageCode { get; set; } = "";
        public bool Public { get; set; }
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

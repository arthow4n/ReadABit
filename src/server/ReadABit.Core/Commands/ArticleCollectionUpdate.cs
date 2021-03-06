using System.ComponentModel.DataAnnotations;
using System;
using MediatR;
using Newtonsoft.Json;
using FluentValidation;

namespace ReadABit.Core.Commands
{
    public record ArticleCollectionUpdate : IRequest<bool>
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        [JsonIgnore]
        public Guid UserId { get; set; }
        public string Name { get; set; } = "";
        public string LanguageCode { get; set; } = "";
    }

    public class ArticleCollectionUpdateValidator : AbstractValidator<ArticleCollectionUpdate>
    {
        public ArticleCollectionUpdateValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.LanguageCode).NotEmpty();
        }
    }
}

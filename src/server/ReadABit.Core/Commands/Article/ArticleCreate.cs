using System;
using FluentValidation;
using MediatR;
using Newtonsoft.Json;
using NSwag.Annotations;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public record ArticleCreate : IRequest<Article>
    {
        [OpenApiIgnore, JsonIgnore]
        public Guid UserId { get; set; }
        [OpenApiIgnore, JsonIgnore]
        public Guid ArticleCollectionId { get; set; }
        public string Name { get; set; } = "";
        public string Text { get; set; } = "";
    }

    public class ArticleCreateValidator : AbstractValidator<ArticleCreate>
    {
        public ArticleCreateValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Text).NotEmpty();
        }
    }
}

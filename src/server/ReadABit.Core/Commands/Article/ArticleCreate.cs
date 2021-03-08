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
        public Guid UserId { get; init; }
        [OpenApiIgnore, JsonIgnore]
        public Guid ArticleCollectionId { get; init; }
        public string Name { get; init; } = "";
        public string Text { get; init; } = "";
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

using System;
using FluentValidation;
using MediatR;
using Newtonsoft.Json;
using NSwag.Annotations;

namespace ReadABit.Core.Commands
{
    public record ArticleList : IPaginatedRequest, IRequest<Paginated<ArticleListItemViewModel>>
    {
        [OpenApiIgnore, JsonIgnore]
        public Guid UserId { get; init; }
        public Guid ArticleCollectionId { get; init; }
        public PageFilter Page { get; init; } = new();
    }

    public class ArticleListValidator : AbstractValidator<ArticleList>
    {
        public ArticleListValidator()
        {
            RuleFor(x => x.Page).MustBeValidPageFilter();
        }
    }

    public record ArticleListItemViewModel
    {
        public Guid Id { get; init; }
        public Guid ArticleCollectionId { get; init; }
        public string Name { get; init; } = "";
    }
}

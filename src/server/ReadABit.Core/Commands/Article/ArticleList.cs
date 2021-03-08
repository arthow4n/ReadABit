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
        public Guid UserId { get; set; }
        public Guid ArticleCollectionId { get; set; }
        public PageFilter Page { get; set; } = new();
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
        public Guid Id { get; set; }
        public Guid ArticleCollectionId { get; set; }
        public string Name { get; set; } = "";
    }
}

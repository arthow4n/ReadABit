using System;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using NSwag.Annotations;
using ReadABit.Core.Contracts;

namespace ReadABit.Core.Commands
{
    public record ArticleCollectionList : IPaginatedRequest, IRequest<Paginated<ArticleCollectionViewModel>>
    {
        [OpenApiIgnore, JsonIgnore]
        public Guid UserId { get; init; }
        public ArticleCollectionListFilter Filter { get; init; } = new();
        public PageFilter Page { get; init; } = new();
        public SortBy SortBy { get; init; }
    }

    public class ArticleCollectionListValidator : AbstractValidator<ArticleCollectionList>
    {
        public ArticleCollectionListValidator()
        {
            RuleFor(x => x.Page).MustBeValidPageFilter();
        }
    }

    public record ArticleCollectionListFilter
    {
        public Guid? OwnedByUserId { get; init; }
        public string? Name { get; init; }
        [BindRequired]
        public string LanguageCode { get; init; } = "";
    }
}

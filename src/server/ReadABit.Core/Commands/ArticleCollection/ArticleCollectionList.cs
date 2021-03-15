using System;
using FluentValidation;
using MediatR;
using Newtonsoft.Json;
using NSwag.Annotations;
using ReadABit.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ReadABit.Core.Commands
{
    public record ArticleCollectionList : IPaginatedRequest, IRequest<Paginated<ArticleCollection>>
    {
        [OpenApiIgnore, JsonIgnore]
        public Guid UserId { get; init; }
        public ArticleCollectionListFilter Filter { get; init; } = new();
        public PageFilter Page { get; init; } = new();
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

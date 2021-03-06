using System;
using System.Collections.Generic;
using MediatR;
using Newtonsoft.Json;
using NSwag.Annotations;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public record ArticleCollectionList : IRequest<List<ArticleCollection>>
    {
        [OpenApiIgnore, JsonIgnore]
        public Guid UserId { get; set; }

        public ArticleCollectionListFilter Filter { get; set; } = new ArticleCollectionListFilter { };
    }

    public class ArticleCollectionListFilter
    {
        public Guid? OwnedByUserId { get; set; }

        public string? Name { get; set; }

        public string LanguageCode { get; set; } = "";
    }
}

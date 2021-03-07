using System;
using System.Collections.Generic;
using MediatR;
using Newtonsoft.Json;
using NSwag.Annotations;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public record ArticleList : IRequest<List<Article>>
    {
        [OpenApiIgnore, JsonIgnore]
        public Guid UserId { get; set; }
        public Guid ArticleCollectionId { get; set; }
    }
}

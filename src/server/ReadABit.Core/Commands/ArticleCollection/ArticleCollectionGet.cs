using System;
using MediatR;
using Newtonsoft.Json;
using NSwag.Annotations;
using ReadABit.Core.Contracts;

namespace ReadABit.Core.Commands
{
    public record ArticleCollectionGet : IRequest<ArticleCollectionViewModel?>
    {
        [OpenApiIgnore, JsonIgnore]
        public Guid Id { get; init; }
        [OpenApiIgnore, JsonIgnore]
        public Guid UserId { get; init; }
    }
}

using System;
using MediatR;
using Newtonsoft.Json;
using NSwag.Annotations;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public record ArticleCollectionGet : IRequest<ArticleCollection?>
    {
        [OpenApiIgnore, JsonIgnore]
        public Guid Id { get; init; }
        [OpenApiIgnore, JsonIgnore]
        public Guid UserId { get; init; }
    }
}

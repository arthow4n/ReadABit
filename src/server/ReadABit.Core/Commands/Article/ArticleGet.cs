using System;
using MediatR;
using Newtonsoft.Json;
using NSwag.Annotations;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public record ArticleGet : IRequest<Article?>
    {
        [OpenApiIgnore, JsonIgnore]
        public Guid Id { get; set; }
        [OpenApiIgnore, JsonIgnore]
        public Guid UserId { get; set; }
    }
}

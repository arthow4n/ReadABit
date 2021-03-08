using System;
using MediatR;
using Newtonsoft.Json;
using NSwag.Annotations;

namespace ReadABit.Core.Commands
{
    public record ArticleDelete : IRequest<bool>
    {
        [OpenApiIgnore, JsonIgnore]
        public Guid Id { get; init; }
        [OpenApiIgnore, JsonIgnore]
        public Guid UserId { get; init; }
    }
}

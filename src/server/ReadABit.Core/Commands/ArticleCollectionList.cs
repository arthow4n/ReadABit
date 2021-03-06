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
    }
}

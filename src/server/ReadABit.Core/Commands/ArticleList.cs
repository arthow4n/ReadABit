using System;
using System.Collections.Generic;
using MediatR;
using Newtonsoft.Json;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public record ArticleList : IRequest<List<Article>>
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
        [JsonIgnore]
        public Guid ArticleCollectionId { get; set; }
    }
}

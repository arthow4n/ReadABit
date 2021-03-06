using System;
using System.Collections.Generic;
using MediatR;
using Newtonsoft.Json;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public record ArticleCollectionList : IRequest<List<ArticleCollection>>
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using MediatR;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public class ArticleList : IRequest<List<Article>>
    {
        public Guid UserId { get; set; }
        public Guid ArticleCollectionId { get; set; }
    }
}

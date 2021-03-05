using System;
using System.Collections.Generic;
using MediatR;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public class ArticleCollectionList : IRequest<List<ArticleCollection>>
    {
        public Guid UserId { get; set; }
    }
}

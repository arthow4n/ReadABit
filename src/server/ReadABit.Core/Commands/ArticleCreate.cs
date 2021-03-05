using System;
using MediatR;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public class ArticleCreate : IRequest<Article>
    {
        public Guid UserId { get; set; }
        public Guid ArticleCollectionId { get; set; }
        public string Name { get; set; } = "";
        public string Text { get; set; } = "";
    }
}

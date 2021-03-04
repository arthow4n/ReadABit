using System;
using MediatR;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public class ArticleCollectionCreate : IRequest<ArticleCollection>
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = "";
    }
}

using System;
using MediatR;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public class ArticleCollectionGet : IRequest<ArticleCollection?>
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
    }
}

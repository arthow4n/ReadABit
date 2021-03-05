using System;
using MediatR;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public class ArticleGet : IRequest<Article?>
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
    }
}

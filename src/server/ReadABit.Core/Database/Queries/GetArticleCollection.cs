using System;
using MediatR;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Database.Commands
{
    public class GetArticleCollection : IRequest<ArticleCollection?>
    {
        public Guid Id { get; set; } = Guid.Empty;
    }
}

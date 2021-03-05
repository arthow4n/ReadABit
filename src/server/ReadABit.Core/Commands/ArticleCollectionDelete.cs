using System;
using MediatR;

namespace ReadABit.Core.Commands
{
    public class ArticleCollectionDelete : IRequest<bool>
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
    }
}

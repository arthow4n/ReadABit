using System;
using MediatR;

namespace ReadABit.Core.Commands
{
    public class ArticleDelete : IRequest<bool>
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
    }
}

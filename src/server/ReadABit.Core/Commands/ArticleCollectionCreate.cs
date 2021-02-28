using System;
using MediatR;

namespace ReadABit.Core.Commands
{
    public class ArticleCollectionCreate : IRequest<Guid>
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = "";
    }
}

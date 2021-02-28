using System;
using MediatR;

namespace ReadABit.Core.Database.Commands
{
    public class CreateArticleCollection : IRequest<Guid>
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = "";
    }
}

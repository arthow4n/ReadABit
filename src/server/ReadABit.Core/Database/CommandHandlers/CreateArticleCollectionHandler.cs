using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReadABit.Core.Database.Commands;
using EnsureThat;
using ReadABit.Infrastructure;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Database.CommandHandlers
{
    public class CreateArticleCollectionHandler : IRequestHandler<CreateArticleCollection, Guid>
    {
        private readonly CoreDbContext db;

        public CreateArticleCollectionHandler(CoreDbContext db)
        {
            this.db = db;
        }

        public async Task<Guid> Handle(CreateArticleCollection request, CancellationToken cancellationToken)
        {
            Ensure.That(request.Name, nameof(request.Name)).IsNotNullOrWhiteSpace();

            var articleCollection = new ArticleCollection
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
            };

            await db.AddAsync(articleCollection);

            return articleCollection.Id; 
        }
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using EnsureThat;
using ReadABit.Infrastructure;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public class ArticleCollectionCreateHandler : IRequestHandler<ArticleCollectionCreate, Guid>
    {
        private readonly CoreDbContext db;

        public ArticleCollectionCreateHandler(CoreDbContext db)
        {
            this.db = db;
        }

        public async Task<Guid> Handle(ArticleCollectionCreate request, CancellationToken cancellationToken)
        {
            Ensure.That(request.Name, nameof(request.Name)).IsNotNullOrWhiteSpace();

            var articleCollection = new ArticleCollection
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Name = request.Name,
            };

            await db.AddAsync(articleCollection);

            return articleCollection.Id;
        }
    }
}

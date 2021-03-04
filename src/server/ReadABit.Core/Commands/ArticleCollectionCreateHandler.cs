using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using EnsureThat;
using ReadABit.Infrastructure;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public class ArticleCollectionCreateHandler : IRequestHandler<ArticleCollectionCreate, ArticleCollection>
    {
        private readonly CoreDbContext _db;

        public ArticleCollectionCreateHandler(CoreDbContext db)
        {
            _db = db;
        }

        public async Task<ArticleCollection> Handle(ArticleCollectionCreate request, CancellationToken cancellationToken)
        {
            Ensure.That(request.Name, nameof(request.Name)).IsNotNullOrWhiteSpace();

            var articleCollection = new ArticleCollection
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Name = request.Name,
            };

            await _db.AddAsync(articleCollection);

            return articleCollection;
        }
    }
}

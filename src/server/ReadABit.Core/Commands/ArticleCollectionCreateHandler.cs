using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using EnsureThat;
using ReadABit.Infrastructure.Models;
using ReadABit.Core.Utils;

namespace ReadABit.Core.Commands
{
    public class ArticleCollectionCreateHandler : IRequestHandler<ArticleCollectionCreate, ArticleCollection>
    {
        private readonly DB _db;

        public ArticleCollectionCreateHandler(DB db)
        {
            _db = db;
        }

        public async Task<ArticleCollection> Handle(ArticleCollectionCreate request, CancellationToken cancellationToken)
        {
            Ensure.That(request.Name, nameof(request.Name)).IsNotNullOrWhiteSpace();
            Ensure.That(LanguageCode.IsValid(request.LanguageCode), $"{nameof(request.LanguageCode)} is valid").IsTrue();

            var articleCollection = new ArticleCollection
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Name = request.Name,
                LanguageCode = request.LanguageCode,
            };

            await _db.Unsafe.AddAsync(articleCollection, cancellationToken);

            return articleCollection;
        }
    }
}

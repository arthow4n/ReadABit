using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReadABit.Infrastructure.Models;
using ReadABit.Core.Utils;
using FluentValidation;

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
            new ArticleCollectionCreateValidator().ValidateAndThrow(request);

            var articleCollection = new ArticleCollection
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Name = request.Name,
                LanguageCode = request.LanguageCode,
                Public = request.Public,
            };

            await _db.Unsafe.AddAsync(articleCollection, cancellationToken);

            return articleCollection;
        }
    }
}

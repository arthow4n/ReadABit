using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReadABit.Infrastructure.Models;
using ReadABit.Core.Utils;
using FluentValidation;
using NodaTime;

namespace ReadABit.Core.Commands
{
    public class ArticleCollectionCreateHandler : IRequestHandler<ArticleCollectionCreate, ArticleCollection>
    {
        private readonly DB _db;
        private readonly IClock _clock;

        public ArticleCollectionCreateHandler(DB db, IClock clock)
        {
            _db = db;
            _clock = clock;
        }

        public async Task<ArticleCollection> Handle(ArticleCollectionCreate request, CancellationToken cancellationToken)
        {
            new ArticleCollectionCreateValidator().ValidateAndThrow(request);

            var articleCollection = new ArticleCollection
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Name = request.Name.Trim(),
                LanguageCode = request.LanguageCode,
                Public = request.Public,
                CreatedAt = _clock.GetCurrentInstant(),
                UpdatedAt = _clock.GetCurrentInstant(),
            };

            await _db.Unsafe.AddAsync(articleCollection, cancellationToken);

            return articleCollection;
        }
    }
}

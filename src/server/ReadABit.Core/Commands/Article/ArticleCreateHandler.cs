using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReadABit.Infrastructure.Models;
using ReadABit.Core.Utils;
using ReadABit.Core.Integrations.Services;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using NodaTime;

namespace ReadABit.Core.Commands
{
    public class ArticleCreateHandler : IRequestHandler<ArticleCreate, Article>
    {
        private readonly DB _db;
        private readonly IClock _clock;

        public ArticleCreateHandler(DB db, IClock clock)
        {
            _db = db;
            _clock = clock;
        }

        public async Task<Article> Handle(ArticleCreate request, CancellationToken cancellationToken)
        {
            new ArticleCreateValidator().ValidateAndThrow(request);

            var articleCollection =
                await _db.ArticleCollectionsOfUser(request.UserId)
                         .Where(ac => ac.Id == request.ArticleCollectionId)
                         .SingleOrDefaultAsync(cancellationToken: cancellationToken);

            articleCollection.UpdatedAt = _clock.GetCurrentInstant();

            var article = new Article
            {
                Id = Guid.NewGuid(),
                ArticleCollectionId = articleCollection.Id,
                Name = request.Name.Trim(),
                Text = request.Text,
                ConlluDocument = UDPipeV1Service.ToConlluDocument(articleCollection.LanguageCode, request.Text),
                CreatedAt = _clock.GetCurrentInstant(),
                UpdatedAt = _clock.GetCurrentInstant(),
            };

            await _db.Unsafe.AddAsync(article, cancellationToken);

            return article;
        }
    }
}

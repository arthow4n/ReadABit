using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using EnsureThat;
using ReadABit.Infrastructure.Models;
using ReadABit.Core.Utils;
using ReadABit.Core.Integrations.Services;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace ReadABit.Core.Commands
{
    public class ArticleCreateHandler : IRequestHandler<ArticleCreate, Article>
    {
        private readonly DB _db;

        public ArticleCreateHandler(DB db)
        {
            _db = db;
        }

        public async Task<Article> Handle(ArticleCreate request, CancellationToken cancellationToken)
        {
            new ArticleCreateValidator().ValidateAndThrow(request);

            var articleCollection =
                await _db.ArticleCollectionsOfUser(request.UserId)
                         .Where(ac => ac.Id == request.ArticleCollectionId)
                         .Select(ac => new
                         {
                             ac.Id,
                             ac.LanguageCode,
                         })
                         .SingleOrDefaultAsync(cancellationToken: cancellationToken);

            var article = new Article
            {
                Id = Guid.NewGuid(),
                ArticleCollectionId = articleCollection.Id,
                Name = request.Name,
                Text = request.Text,
                Conllu = UDPipeV1Service.ToConllu(articleCollection.LanguageCode, request.Text),
            };

            await _db.Unsafe.AddAsync(article, cancellationToken);

            return article;
        }
    }
}

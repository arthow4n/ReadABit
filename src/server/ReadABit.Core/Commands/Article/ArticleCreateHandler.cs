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

            // TODO: Take language code from request.
            var sparvXml = SparvPipelineService.ToSparvXml("sv", request.Text);
            var article = new Article
            {
                Id = Guid.NewGuid(),
                ArticleCollectionId = articleCollection.Id,
                Name = request.Name.Trim(),
                Text = request.Text,
                SparvXmlJson = sparvXml.XmlJson,
                SparvXmlVersion = sparvXml.Version,
            };

            await _db.Unsafe.AddAsync(article, cancellationToken);

            return article;
        }
    }
}

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReadABit.Core.Commands.Utils;
using ReadABit.Core.Contracts;
using ReadABit.Core.Integrations.Services;
using ReadABit.Core.Utils;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public class ArticleCreateHandler : CommandHandlerBase, IRequestHandler<ArticleCreate, ArticleViewModel>
    {
        public ArticleCreateHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<ArticleViewModel> Handle(ArticleCreate request, CancellationToken cancellationToken)
        {
            new ArticleCreateValidator().ValidateAndThrow(request);

            var articleCollectionQuery =
                DB.ArticleCollectionsOfUser(request.UserId)
                    .Where(ac => ac.Id == request.ArticleCollectionId);

            var articleCollection =
                await articleCollectionQuery.SingleAsync(cancellationToken);

            var lastArticleOrderInCollection =
                await articleCollectionQuery
                    .SelectMany(ac => ac.Articles)
                    .OrderByDescending(a => a.Order)
                    .Select(a => a.Order)
                    .FirstOrDefaultAsync(cancellationToken);

            var now = Clock.GetCurrentInstant();
            articleCollection.UpdatedAt = now;

            var normalisedText = request.Text.Normalize();
            var article = new Article
            {
                Id = Guid.NewGuid(),
                ArticleCollectionId = articleCollection.Id,
                // It's fine to start from 1 as we don't really care the exact number.
                Order = lastArticleOrderInCollection + 1,
                Name = request.Name.Trim().Normalize(),
                Text = normalisedText,
                ConlluDocument = ConlluService.ToConlluDocument(articleCollection.LanguageCode, normalisedText),
                CreatedAt = now,
                UpdatedAt = now,
            };

            await DB.Unsafe.AddAsync(article, cancellationToken);

            var vm = Mapper.Map<ArticleViewModel>(article);
            // FIXME: Find a better way to not do this hack.
            vm.ReadingProgress = new();

            return vm;
        }
    }
}

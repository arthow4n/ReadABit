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

            var articleCollection =
                await DB.ArticleCollectionsOfUser(request.UserId)
                         .Where(ac => ac.Id == request.ArticleCollectionId)
                         .SingleOrDefaultAsync(cancellationToken: cancellationToken);

            articleCollection.UpdatedAt = Clock.GetCurrentInstant();

            var article = new Article
            {
                Id = Guid.NewGuid(),
                ArticleCollectionId = articleCollection.Id,
                Name = request.Name.Trim(),
                Text = request.Text,
                ConlluDocument = UDPipeV1Service.ToConlluDocument(articleCollection.LanguageCode, request.Text),
                CreatedAt = Clock.GetCurrentInstant(),
                UpdatedAt = Clock.GetCurrentInstant(),
            };

            await DB.Unsafe.AddAsync(article, cancellationToken);

            var vm = Mapper.Map<ArticleViewModel>(article);
            // FIXME: Find a better way to not do this hack.
            vm.ReadingProgress = new();

            return vm;
        }
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using ReadABit.Core.Commands.Utils;
using ReadABit.Core.Contracts;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public class ArticleCollectionCreateHandler : CommandHandlerBase, IRequestHandler<ArticleCollectionCreate, ArticleCollectionViewModel>
    {
        public ArticleCollectionCreateHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<ArticleCollectionViewModel> Handle(ArticleCollectionCreate request, CancellationToken cancellationToken)
        {
            new ArticleCollectionCreateValidator().ValidateAndThrow(request);

            var now = Clock.GetCurrentInstant();
            var articleCollection = new ArticleCollection
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Name = request.Name.Trim(),
                LanguageCode = request.LanguageCode,
                Public = request.Public,
                CreatedAt = now,
                UpdatedAt = now,
            };

            await DB.Unsafe.AddAsync(articleCollection, cancellationToken);

            return Mapper.Map<ArticleCollectionViewModel>(articleCollection);
        }
    }
}

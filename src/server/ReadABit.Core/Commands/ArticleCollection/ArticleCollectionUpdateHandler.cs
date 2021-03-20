using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReadABit.Core.Commands.Utils;
using ReadABit.Core.Utils;

namespace ReadABit.Core.Commands
{
    public class ArticleCollectionUpdateHandler : CommandHandlerBase, IRequestHandler<ArticleCollectionUpdate, bool>
    {
        public ArticleCollectionUpdateHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<bool> Handle(ArticleCollectionUpdate request, CancellationToken cancellationToken)
        {
            new ArticleCollectionUpdateValidator().ValidateAndThrow(request);

            var articleCollection = await DB
                .ArticleCollectionsOfUser(request.UserId)
                .Where(ac => ac.Id == request.Id)
                .SingleOrDefaultAsync(cancellationToken: cancellationToken);

            if (articleCollection is null)
            {
                return false;
            }

            articleCollection.Name = request.Name is not null ? request.Name.Trim() : articleCollection.Name;
            articleCollection.LanguageCode = request.LanguageCode ?? articleCollection.LanguageCode;
            articleCollection.Public = request.Public ?? articleCollection.Public;
            articleCollection.UpdatedAt = Clock.GetCurrentInstant();

            return true;
        }
    }
}

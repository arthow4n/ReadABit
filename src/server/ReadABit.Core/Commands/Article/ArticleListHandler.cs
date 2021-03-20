using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReadABit.Core.Commands.Utils;
using ReadABit.Core.Utils;

namespace ReadABit.Core.Commands
{
    public class ArticleListHandler : CommandHandlerBase, IRequestHandler<ArticleList, Paginated<ArticleListItemViewModel>>
    {
        public ArticleListHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<Paginated<ArticleListItemViewModel>> Handle(ArticleList request, CancellationToken cancellationToken)
        {
            return await DB.ArticleCollectionsOfUserOrPublic(request.UserId)
                            .AsNoTracking()
                            .Where(ac => ac.Id == request.ArticleCollectionId)
                            .SelectMany(ac => ac.Articles)
                            .Select(a => new ArticleListItemViewModel
                            {
                                Id = a.Id,
                                ArticleCollectionId = a.ArticleCollectionId,
                                Name = a.Name,
                            })
                            .ToPaginatedAsync(request.Page, 50, cancellationToken);
        }
    }
}

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReadABit.Infrastructure.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ReadABit.Core.Utils;
using System.Collections.Generic;

namespace ReadABit.Core.Commands
{
    public class ArticleListHandler : IRequestHandler<ArticleList, Paginated<ArticleListItemViewModel>>
    {
        private readonly DB _db;

        public ArticleListHandler(DB db)
        {
            _db = db;
        }

        public async Task<Paginated<ArticleListItemViewModel>> Handle(ArticleList request, CancellationToken cancellationToken)
        {
            return await _db.ArticleCollectionsOfUserOrPublic(request.UserId)
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

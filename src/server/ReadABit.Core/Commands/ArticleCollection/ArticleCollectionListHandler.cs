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
    public class ArticleCollectionListHandler : IRequestHandler<ArticleCollectionList, Paginated<ArticleCollection>>
    {
        private readonly DB _db;

        public ArticleCollectionListHandler(DB db)
        {
            _db = db;
        }

        public async Task<Paginated<ArticleCollection>> Handle(ArticleCollectionList request, CancellationToken cancellationToken)
        {
            return await _db
                .ArticleCollectionsOfUserOrPublic(request.UserId)
                .AsNoTracking()
                .Where(ac => ac.LanguageCode == request.Filter.LanguageCode)
                .Where(ac => request.Filter.OwnedByUserId == null || ac.UserId == request.Filter.OwnedByUserId)
                .Where(ac => string.IsNullOrWhiteSpace(request.Filter.Name) || ac.Name.StartsWith(request.Filter.Name))
                // TODO: Sorting
                .ToPaginatedAsync(request.Page, 50, cancellationToken);
        }
    }
}

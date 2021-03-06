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
    public class ArticleCollectionListHandler : IRequestHandler<ArticleCollectionList, List<ArticleCollection>>
    {
        private readonly DB _db;

        public ArticleCollectionListHandler(DB db)
        {
            _db = db;
        }

        public async Task<List<ArticleCollection>> Handle(ArticleCollectionList request, CancellationToken cancellationToken)
        {
            return await _db
                .ArticleCollectionsOfUserOrPublic(request.UserId)
                .ToListAsync(cancellationToken: cancellationToken);
        }
    }
}

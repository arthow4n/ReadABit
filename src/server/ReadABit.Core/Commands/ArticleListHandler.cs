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
    public class ArticleListHandler : IRequestHandler<ArticleList, List<Article>>
    {
        private readonly DB _db;

        public ArticleListHandler(DB db)
        {
            _db = db;
        }

        public async Task<List<Article>> Handle(ArticleList request, CancellationToken cancellationToken)
        {
            return await _db.ArticleCollectionsOfUserOrPublic(request.UserId)
                            .Where(ac => ac.Id == request.ArticleCollectionId)
                            .SelectMany(ac => ac.Articles)
                            .ToListAsync(cancellationToken: cancellationToken);
        }
    }
}

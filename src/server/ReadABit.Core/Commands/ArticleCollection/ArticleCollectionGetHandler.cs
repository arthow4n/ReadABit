using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReadABit.Infrastructure.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ReadABit.Core.Utils;

namespace ReadABit.Core.Commands
{
    public class ArticleCollectionGetHandler : IRequestHandler<ArticleCollectionGet, ArticleCollection?>
    {
        private readonly DB _db;

        public ArticleCollectionGetHandler(DB db)
        {
            _db = db;
        }

        public async Task<ArticleCollection?> Handle(ArticleCollectionGet request, CancellationToken cancellationToken)
        {
            return await _db
                .ArticleCollectionsOfUserOrPublic(request.UserId)
                .AsNoTracking()
                .Where(ac => ac.Id == request.Id)
                .SingleOrDefaultAsync(cancellationToken: cancellationToken);
        }
    }
}

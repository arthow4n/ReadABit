using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReadABit.Core.Commands;
using ReadABit.Infrastructure;
using ReadABit.Infrastructure.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ReadABit.Core.Commands
{
    public class ArticleCollectionGetHandler : IRequestHandler<ArticleCollectionGet, ArticleCollection?>
    {
        private readonly CoreDbContext _db;

        public ArticleCollectionGetHandler(CoreDbContext db)
        {
            _db = db;
        }

        public async Task<ArticleCollection?> Handle(ArticleCollectionGet request, CancellationToken cancellationToken)
        {
            return await _db.ArticleCollections
                .Where(ac => ac.Id == request.Id)
                .SingleOrDefaultAsync();
        }
    }
}

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReadABit.Core.Database.Commands;
using ReadABit.Infrastructure;
using ReadABit.Infrastructure.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ReadABit.Core.Database.CommandHandlers
{
    public class GetArticleCollectionHandler : IRequestHandler<GetArticleCollection, ArticleCollection?>
    {
        private readonly CoreDbContext db;

        public GetArticleCollectionHandler(CoreDbContext db)
        {
            this.db = db;
        }

        public async Task<ArticleCollection?> Handle(GetArticleCollection request, CancellationToken cancellationToken)
        {
            return await db.ArticleCollections
                .Where(ac => ac.Id == request.Id)
                .SingleOrDefaultAsync();
        }
    }
}

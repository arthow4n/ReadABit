using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReadABit.Infrastructure.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ReadABit.Core.Utils;

namespace ReadABit.Core.Commands
{
    public class ArticleGetHandler : IRequestHandler<ArticleGet, Article?>
    {
        private readonly DB _db;

        public ArticleGetHandler(DB db)
        {
            _db = db;
        }

        public async Task<Article?> Handle(ArticleGet request, CancellationToken cancellationToken)
        {
            return await _db.ArticlesOfUser(request.UserId)
                            .Where(ac => ac.Id == request.Id)
                            .SingleOrDefaultAsync(cancellationToken: cancellationToken);
        }
    }
}

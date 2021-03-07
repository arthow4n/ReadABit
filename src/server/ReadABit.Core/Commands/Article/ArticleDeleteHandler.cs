using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReadABit.Infrastructure.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ReadABit.Core.Utils;

namespace ReadABit.Core.Commands
{
    public class ArticleDeleteHandler : IRequestHandler<ArticleDelete, bool>
    {
        private readonly DB _db;

        public ArticleDeleteHandler(DB db)
        {
            _db = db;
        }

        public async Task<bool> Handle(ArticleDelete request, CancellationToken cancellationToken)
        {
            var target =
                await _db.ArticlesOfUser(request.UserId)
                         .Where(ac => ac.Id == request.Id)
                         .SingleOrDefaultAsync(cancellationToken: cancellationToken);

            if (target is null)
            {
                return false;
            }

            _db.Unsafe.Remove(target);
            return true;
        }
    }
}

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReadABit.Infrastructure.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ReadABit.Core.Utils;

namespace ReadABit.Core.Commands
{
    public class ArticleCollectionDeleteHandler : IRequestHandler<ArticleCollectionDelete, bool>
    {
        private readonly DB _db;

        public ArticleCollectionDeleteHandler(DB db)
        {
            _db = db;
        }

        public async Task<bool> Handle(ArticleCollectionDelete request, CancellationToken cancellationToken)
        {
            var target = await _db.ArticleCollectionOfUser(request.UserId).Where(ac => ac.Id == request.Id).SingleOrDefaultAsync();
            if (target is null)
            {
                return false;
            }

            _db.Unsafe.Remove(target);
            return true;
        }
    }
}

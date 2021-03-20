using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReadABit.Core.Commands.Utils;
using ReadABit.Core.Utils;

namespace ReadABit.Core.Commands
{
    public class ArticleCollectionDeleteHandler : CommandHandlerBase, IRequestHandler<ArticleCollectionDelete, bool>
    {
        public ArticleCollectionDeleteHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<bool> Handle(ArticleCollectionDelete request, CancellationToken cancellationToken)
        {
            var target = await DB.ArticleCollectionsOfUser(request.UserId)
                                  .Where(ac => ac.Id == request.Id)
                                  .SingleOrDefaultAsync(cancellationToken: cancellationToken);

            if (target is null)
            {
                return false;
            }

            DB.Unsafe.Remove(target);
            return true;
        }
    }
}

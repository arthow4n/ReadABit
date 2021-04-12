using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReadABit.Core.Commands.Utils;
using ReadABit.Core.Contracts;
using ReadABit.Core.Utils;

namespace ReadABit.Core.Commands
{
    public class ArticleGetHandler : CommandHandlerBase, IRequestHandler<ArticleGet, ArticleViewModel?>
    {
        public ArticleGetHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<ArticleViewModel?> Handle(ArticleGet request, CancellationToken cancellationToken)
        {
            var result = await DB.ArticleCollectionsOfUserOrPublic(request.UserId)
                                 .SelectMany(ac => ac.Articles)
                                 .Where(a => a.Id == request.Id)
                                 .ProjectTo<ArticleViewModel>(Mapper.ConfigurationProvider, new { userId = request.UserId })
                                 .SingleOrDefaultAsync(cancellationToken: cancellationToken);

            if (result is not null)
            {
                result.ReadingProgress ??= new();
            }

            return result;
        }
    }
}

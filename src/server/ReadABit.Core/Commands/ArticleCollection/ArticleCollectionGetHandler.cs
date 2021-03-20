using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReadABit.Core.Commands.Utils;
using ReadABit.Core.Contracts;
using ReadABit.Core.Utils;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public class ArticleCollectionGetHandler : CommandHandlerBase, IRequestHandler<ArticleCollectionGet, ArticleCollectionViewModel?>
    {
        public ArticleCollectionGetHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<ArticleCollectionViewModel?> Handle(ArticleCollectionGet request, CancellationToken cancellationToken)
        {
            return await DB
                .ArticleCollectionsOfUserOrPublic(request.UserId)
                .AsNoTracking()
                .Where(ac => ac.Id == request.Id)
                .ProjectTo<ArticleCollectionViewModel>(Mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(cancellationToken: cancellationToken);
        }
    }
}

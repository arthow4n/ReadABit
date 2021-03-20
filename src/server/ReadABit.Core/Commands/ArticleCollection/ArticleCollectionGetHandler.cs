using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReadABit.Core.Contracts;
using ReadABit.Core.Utils;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public class ArticleCollectionGetHandler : IRequestHandler<ArticleCollectionGet, ArticleCollectionViewModel?>
    {
        private readonly DB _db;
        private readonly IMapper _mapper;

        public ArticleCollectionGetHandler(DB db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<ArticleCollectionViewModel?> Handle(ArticleCollectionGet request, CancellationToken cancellationToken)
        {
            return await _db
                .ArticleCollectionsOfUserOrPublic(request.UserId)
                .AsNoTracking()
                .Where(ac => ac.Id == request.Id)
                .ProjectTo<ArticleCollectionViewModel>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(cancellationToken: cancellationToken);
        }
    }
}

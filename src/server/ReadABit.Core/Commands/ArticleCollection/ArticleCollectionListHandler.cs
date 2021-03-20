using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReadABit.Core.Contracts;
using ReadABit.Core.Utils;

namespace ReadABit.Core.Commands
{
    public class ArticleCollectionListHandler : IRequestHandler<ArticleCollectionList, Paginated<ArticleCollectionViewModel>>
    {
        private readonly DB _db;
        private readonly IMapper _mapper;

        public ArticleCollectionListHandler(DB db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Paginated<ArticleCollectionViewModel>> Handle(ArticleCollectionList request, CancellationToken cancellationToken)
        {
            return await _db
                .ArticleCollectionsOfUserOrPublic(request.UserId)
                .AsNoTracking()
                .Where(ac => ac.LanguageCode == request.Filter.LanguageCode)
                .Where(ac => request.Filter.OwnedByUserId == null || ac.UserId == request.Filter.OwnedByUserId)
                .Where(ac => string.IsNullOrWhiteSpace(request.Filter.Name) || ac.Name.StartsWith(request.Filter.Name))
                .SortBy(request.SortBy)
                .ProjectTo<ArticleCollectionViewModel>(_mapper.ConfigurationProvider)
                .ToPaginatedAsync(request.Page, 50, cancellationToken);
        }
    }
}

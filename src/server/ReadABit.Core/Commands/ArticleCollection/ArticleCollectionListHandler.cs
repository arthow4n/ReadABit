using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReadABit.Core.Commands.Utils;
using ReadABit.Core.Contracts;

namespace ReadABit.Core.Commands
{
    public class ArticleCollectionListHandler : CommandHandlerBase, IRequestHandler<ArticleCollectionList, Paginated<ArticleCollectionListItemViewModel>>
    {
        public ArticleCollectionListHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<Paginated<ArticleCollectionListItemViewModel>> Handle(ArticleCollectionList request, CancellationToken cancellationToken)
        {
            new ArticleCollectionListValidator().ValidateAndThrow(request);

            return await DB
                .ArticleCollectionsOfUserOrPublic(request.UserId)
                .AsNoTracking()
                .Where(ac => ac.LanguageCode == request.Filter.LanguageCode)
                .Where(ac => request.Filter.OwnedByUserId == null || ac.UserId == request.Filter.OwnedByUserId)
                .Where(ac => string.IsNullOrWhiteSpace(request.Filter.Name) || ac.Name.StartsWith(request.Filter.Name))
                .SortBy(request.SortBy)
                .ProjectTo<ArticleCollectionListItemViewModel>(Mapper.ConfigurationProvider)
                .ToPaginatedAsync(request.Page, 50, cancellationToken);
        }
    }
}

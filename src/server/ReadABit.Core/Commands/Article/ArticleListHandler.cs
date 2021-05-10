using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using ReadABit.Core.Commands.Utils;
using ReadABit.Core.Contracts;
using ReadABit.Core.Utils;

namespace ReadABit.Core.Commands
{
    public class ArticleListHandler : CommandHandlerBase, IRequestHandler<ArticleList, Paginated<ArticleListItemViewModel>>
    {
        public ArticleListHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<Paginated<ArticleListItemViewModel>> Handle(ArticleList request, CancellationToken cancellationToken)
        {
            new ArticleListValidator().ValidateAndThrow(request);

            return await DB.ArticleCollectionsOfUserOrPublic(request.UserId)
                           .Where(ac => !request.ArticleCollectionId.HasValue || ac.Id == request.ArticleCollectionId)
                           .SelectMany(ac => ac.Articles)
                           .SortBy(request.SortBy, request.UserId)
                           .ProjectTo<ArticleListItemViewModel>(Mapper.ConfigurationProvider, new { userId = request.UserId })
                           .ToPaginatedAsync(request.Page, 50, cancellationToken);
        }
    }
}

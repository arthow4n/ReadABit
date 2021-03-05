using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using ReadABit.Core.Commands;
using ReadABit.Core.Services.Utils;
using ReadABit.Core.Utils;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Services
{
    public class ArticleCollectionService : ServiceBase
    {
        public ArticleCollectionService(IMediator mediator, IRequestContext requestContext) : base(mediator, requestContext)
        {
        }

        public async Task<List<ArticleCollection>> List()
        {
            throw new NotImplementedException();
        }

        public async Task<ArticleCollection> Create(string name, Guid? userId = null)
        {
            return await Mediator.Send(new ArticleCollectionCreate
            {
                Name = name,
                UserId = userId ?? RequestContext.UserId!.Value,
            });
        }

        public async Task<ArticleCollection?> Get(Guid id, Guid? userId = null)
        {
            return await Mediator.Send(new ArticleCollectionGet
            {
                Id = id,
                UserId = userId ?? RequestContext.UserId!.Value,
            });
        }
    }
}

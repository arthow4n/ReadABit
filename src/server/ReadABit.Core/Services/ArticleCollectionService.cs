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

        public async Task<Guid> Create(string name)
        {
            return await mediator.Send(new ArticleCollectionCreate
            {
                UserId = requestContext.UserId!.Value,
                Name = name,
            });
        }

        public async Task<ArticleCollection?> Get(Guid id)
        {
            return await mediator.Send(new ArticleCollectionGet
            {
                Id = id,
            });
        }
    }
}
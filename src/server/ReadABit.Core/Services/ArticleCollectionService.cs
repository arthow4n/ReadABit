using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using ReadABit.Core.Database.Commands;
using ReadABit.Core.Services.Utils;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Services
{
    public class ArticleCollectionService : ServiceBase
    {
        public ArticleCollectionService(IMediator mediator) : base(mediator)
        {
        }

        public async Task<List<ArticleCollection>> List()
        {
            throw new NotImplementedException();
        }

        public async Task<Guid> Create(string name)
        {
            return await mediator.Send(new CreateArticleCollection
            {
                Name = name,
            });
        }

        public async Task<ArticleCollection?> Get(Guid id)
        {
            return await mediator.Send(new GetArticleCollection
            {
                Id = id,
            });
        }
    }
}
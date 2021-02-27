using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReadABit.Core.Database.Commands;
using ReadABit.Core.Services;
using ReadABit.Infrastructure.Models;
using ReadABit.Web.Controller.Utils;

namespace ReadABit.Web.Controllers
{
    public class ArticleCollectionsController : ApiControllerBase
    {
        private readonly ArticleCollectionService service;

        public ArticleCollectionsController(IMediator mediator) : base(mediator)
        {
            this.service = new ArticleCollectionService(mediator);
        }

        [HttpGet]
        public async Task<List<ArticleCollection>> List()
        {
            return await service.List();
        }

        [HttpGet("{id}")]
        public async Task<ArticleCollection> GetArticleCollection(Guid id)
        {
            // TODO: Refactor so it return 404 when not found.
            return await service.Get(id) ?? throw new ArgumentOutOfRangeException();
        }

        [HttpPost]
        public async Task<Guid> CreateArticleCollection(string name)
        {
            var id = await service.Create(name);
            await SaveChangesAsync();
            return id;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReadABit.Core.Services;
using ReadABit.Core.Utils;
using ReadABit.Infrastructure.Models;
using ReadABit.Web.Controller.Utils;

namespace ReadABit.Web.Controllers
{
    public class ArticleCollectionsController : ApiControllerBase
    {
        private readonly ArticleCollectionService _service;

        public ArticleCollectionsController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _service = DI.New<ArticleCollectionService>(serviceProvider);
        }

        [HttpGet]
        public async Task<List<ArticleCollection>> List()
        {
            return await _service.List();
        }

        [HttpGet("{id}")]
        public async Task<ArticleCollection> GetArticleCollection(Guid id)
        {
            // TODO: Refactor so it return 404 when not found.
            return await _service.Get(id) ?? throw new ArgumentOutOfRangeException();
        }

        [HttpPost]
        public async Task<Guid> CreateArticleCollection(string name)
        {
            var id = await _service.Create(name);
            await SaveChangesAsync();
            return id;
        }
    }
}

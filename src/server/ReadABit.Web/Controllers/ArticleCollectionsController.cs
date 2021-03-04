using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ArticleCollection>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> List()
        {
            var list = await _service.List();
            return Ok(list);
        }

        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ArticleCollection))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetArticleCollection(Guid id)
        {
            var articleCollection = await _service.Get(id);
            if (articleCollection is null)
            {
                return NotFound();
            }

            return Ok(articleCollection);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ArticleCollection))]
        public async Task<IActionResult> CreateArticleCollection(string name)
        {
            var created = await _service.Create(name);
            await SaveChangesAsync();
            return CreatedAtAction(nameof(GetArticleCollection), new { id = created.Id }, created);
        }
    }
}

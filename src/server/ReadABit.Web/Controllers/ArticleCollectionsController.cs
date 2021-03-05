using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReadABit.Core.Commands;
using ReadABit.Infrastructure.Models;
using ReadABit.Web.Controller.Utils;

namespace ReadABit.Web.Controllers
{
    public class ArticleCollectionsController : ApiControllerBase
    {

        public ArticleCollectionsController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ArticleCollection>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ListArticleCollections()
        {
            var list = await Mediator.Send(new ArticleCollectionList
            {
                UserId = RequestUserId,
            });
            return Ok(list);
        }

        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ArticleCollection))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetArticleCollection(Guid id)
        {
            var articleCollection = await Mediator.Send(new ArticleCollectionGet
            {
                Id = id,
                UserId = RequestUserId,
            });

            if (articleCollection is null)
            {
                return NotFound();
            }

            return Ok(articleCollection);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ArticleCollection))]
        public async Task<IActionResult> CreateArticleCollection(string languageCode, string name)
        {
            var created = await Mediator.Send(new ArticleCollectionCreate
            {
                UserId = RequestUserId,
                Name = name,
                LanguageCode = languageCode,
            });
            await SaveChangesAsync();
            return CreatedAtAction(nameof(GetArticleCollection), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateArticleCollection(Guid id, string languageCode, string name)
        {
            var found = await Mediator.Send(new ArticleCollectionUpdate
            {
                Id = id,
                UserId = RequestUserId,
                Name = name,
                LanguageCode = languageCode,
            });

            if (!found)
            {
                return NotFound();
            }

            await SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteArticleCollection(Guid id)
        {
            var found = await Mediator.Send(new ArticleCollectionDelete
            {
                Id = id,
                UserId = RequestUserId,
            });

            if (!found)
            {
                return NotFound();
            }

            await SaveChangesAsync();
            return NoContent();
        }
    }
}

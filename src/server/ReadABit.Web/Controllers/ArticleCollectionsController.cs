using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReadABit.Core.Commands;
using ReadABit.Core.Contracts;
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Paginated<ArticleCollection>))]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> List([FromQuery] ArticleCollectionList request)
        {
            Paginated<ArticleCollectionViewModel> list = await Mediator.Send(request with
            {
                UserId = RequestUserId,
            });
            return Ok(list);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ArticleCollectionViewModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Get(Guid id, [FromQuery] ArticleCollectionGet request)
        {
            var articleCollection = await Mediator.Send(request with
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
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ArticleCollectionViewModel))]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Create(ArticleCollectionCreate request)
        {
            var created = await Mediator.Send(request with
            {
                UserId = RequestUserId,
            });
            await SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Update(Guid id, ArticleCollectionUpdate request)
        {
            var found = await Mediator.Send(request with
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

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(Guid id, [FromQuery] ArticleCollectionDelete request)
        {
            var found = await Mediator.Send(request with
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

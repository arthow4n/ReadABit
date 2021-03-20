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
    public class ArticlesController : ApiControllerBase
    {

        public ArticlesController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Paginated<ArticleListItemViewModel>))]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> List([FromQuery] ArticleList request)
        {
            Paginated<ArticleListItemViewModel> list = await Mediator.Send(request with
            {
                UserId = RequestUserId,
            });
            return Ok(list);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ArticleViewModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Get(Guid id, [FromQuery] ArticleGet request)
        {
            ArticleViewModel article = await Mediator.Send(request with
            {
                Id = id,
                UserId = RequestUserId,
            });

            if (article is null)
            {
                return NotFound();
            }

            return Ok(article);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Article))]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Create(ArticleCreate request)
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
        public async Task<IActionResult> Update(Guid id, ArticleUpdate request)
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
        public async Task<IActionResult> Delete(Guid id, [FromQuery] ArticleDelete request)
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

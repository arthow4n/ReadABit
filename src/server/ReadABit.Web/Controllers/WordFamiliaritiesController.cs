using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReadABit.Core.Commands;
using ReadABit.Core.Contracts;
using ReadABit.Web.Controller.Utils;

namespace ReadABit.Web.Controllers
{
    public class WordFamiliaritiesController : ApiControllerBase
    {

        public WordFamiliaritiesController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WordFamiliarityListViewModel))]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> List([FromQuery] WordFamiliarityList request)
        {
            WordFamiliarityListViewModel list = await Mediator.Send(request with
            {
                UserId = RequestUserId,
            });
            return Ok(list);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Upsert(WordFamiliarityUpsert request)
        {
            await Mediator.Send(request with
            {
                UserId = RequestUserId,
            });

            await SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(Guid id, [FromQuery] WordFamiliarityDelete request)
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

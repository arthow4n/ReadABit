using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReadABit.Core.Commands;
using ReadABit.Infrastructure.Models;
using ReadABit.Web.Controller.Utils;

namespace ReadABit.Web.Controllers
{
    public class WordDefinitionsController : ApiControllerBase
    {

        public WordDefinitionsController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Paginated<WordDefinition>))]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> List([FromQuery] WordDefinitionList request)
        {
            Paginated<WordDefinition> list = await Mediator.Send(request with
            {
                UserId = RequestUserId,
            });
            return Ok(list);
        }

        [Route("/Suggestions")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Paginated<WordDefinitionListPublicSuggestionViewModel>))]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> ListPublicSuggestions([FromQuery] WordDefinitionListPublicSuggestions request)
        {
            Paginated<WordDefinitionListPublicSuggestionViewModel> list = await Mediator.Send(request with
            {
                UserId = RequestUserId,
            });
            return Ok(list);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WordDefinition))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Get(Guid id, [FromQuery] WordDefinitionGet request)
        {
            var wordDefinition = await Mediator.Send(request with
            {
                Id = id,
                UserId = RequestUserId,
            });

            if (wordDefinition is null)
            {
                return NotFound();
            }

            return Ok(wordDefinition);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(WordDefinition))]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Create(WordDefinitionCreate request)
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
        public async Task<IActionResult> Update(Guid id, WordDefinitionUpdate request)
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
        public async Task<IActionResult> Delete(Guid id, [FromQuery] WordDefinitionDelete request)
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

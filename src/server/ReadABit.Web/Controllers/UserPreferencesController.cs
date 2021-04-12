using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReadABit.Core.Commands;
using ReadABit.Infrastructure.Models;
using ReadABit.Web.Controller.Utils;

namespace ReadABit.Web.Controllers
{
    public class UserPreferencesController : ApiControllerBase
    {

        public UserPreferencesController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserPreferenceData))]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Get([FromQuery] UserPreferenceGet request)
        {
            UserPreferenceData vm = await Mediator.Send(request with
            {
                UserId = RequestUserId,
            });
            return Ok(vm);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Upsert(UserPreferenceUpsert request)
        {
            await Mediator.Send(request with
            {
                UserId = RequestUserId,
            });

            await SaveChangesAsync();
            return NoContent();
        }
    }
}

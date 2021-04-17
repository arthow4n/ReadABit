using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReadABit.Core.Commands;
using ReadABit.Core.Contracts;
using ReadABit.Core.Utils;
using ReadABit.Infrastructure.Models;
using ReadABit.Web.Controller.Utils;

namespace ReadABit.Web.Controllers
{
    public class CultureInfoController : ApiControllerBase
    {
        public CultureInfoController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TimeZoneInfoViewModel>))]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> ListAllSupportedTimeZones()
        {
            UserPreferenceData vm = await Mediator.Send(new UserPreferenceGet
            {
                UserId = RequestUserId,
            });

            return Ok(CultureInfoHelper.ListAllSupportedTimeZones(vm.UserInterfaceLanguageCode));
        }
    }
}

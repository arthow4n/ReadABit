using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReadABit.Core.Database.Commands;
using ReadABit.Infrastructure;

namespace ReadABit.Web.Controller.Utils
{
    // Manually versioning because NSwag doesn't have good support for Microsoft.AspNetCore.Mvc.Versioning so far.
    // https://github.com/RicoSuter/NSwag/issues/1355
    [Route("api/v1/[controller]")]
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected readonly IMediator mediator;

        protected ApiControllerBase(IMediator mediator)
        {
            this.mediator = mediator;
        }

        protected async Task SaveChangesAsync()
        {
            await mediator.Send(new SaveChanges { });
        }
    }
}

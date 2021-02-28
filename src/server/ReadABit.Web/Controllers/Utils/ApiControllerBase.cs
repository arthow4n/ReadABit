using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReadABit.Core.Commands;

namespace ReadABit.Web.Controller.Utils
{
    // Manually versioning because NSwag doesn't have good support for Microsoft.AspNetCore.Mvc.Versioning so far.
    // https://github.com/RicoSuter/NSwag/issues/1355
    [Route("api/v1/[controller]")]
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected readonly IMediator mediator;

        protected ApiControllerBase(IServiceProvider serviceProvider, IMediator mediator)
        {
            this.mediator = mediator;
        }

        protected async Task SaveChangesAsync()
        {
            await mediator.Send(new SaveChanges { });
        }
    }
}

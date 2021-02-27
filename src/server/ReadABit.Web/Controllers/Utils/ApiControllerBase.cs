using System;
using Microsoft.AspNetCore.Mvc;
using ReadABit.Infrastructure;

namespace ReadABit.Web.Controller.Utils
{
    // Manually versioning because NSwag doesn't have good support for Microsoft.AspNetCore.Mvc.Versioning so far.
    // https://github.com/RicoSuter/NSwag/issues/1355
    [Route("api/v1/[controller]")]
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected IServiceProvider serviceProvider => HttpContext.RequestServices;

        protected readonly CoreDbContext dbContext;

        public ApiControllerBase(CoreDbContext context)
        {
            this.dbContext = context;
        }
    }
}

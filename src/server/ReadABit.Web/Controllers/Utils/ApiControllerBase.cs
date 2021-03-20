using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Validation.AspNetCore;
using ReadABit.Core.Commands;
using ReadABit.Core.Utils;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Web.Controller.Utils
{
    // Manually versioning because NSwag doesn't have good support for Microsoft.AspNetCore.Mvc.Versioning so far.
    // https://github.com/RicoSuter/NSwag/issues/1355
    [Route("api/v1/[controller]")]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;

        protected ApiControllerBase(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected IMediator Mediator => _serviceProvider.GetRequiredService<IMediator>();
        protected IRequestContext RequestContext => _serviceProvider.GetRequiredService<IRequestContext>();

        /// <summary>
        /// This should only be used in actions with [Authorize] because it asserts the user ID is not null.
        /// </summary>
        protected Guid RequestUserId => RequestContext.UserId!.Value;

        protected UserManager<ApplicationUser> UserManager => _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        protected async Task SaveChangesAsync()
        {
            await Mediator.Send(new SaveChanges { });
        }
    }
}

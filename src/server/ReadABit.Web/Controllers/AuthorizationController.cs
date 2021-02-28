using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ReadABit.Web.Controllers
{
    public class AuthorizationController : ControllerBase
    {
        private readonly IOpenIddictApplicationManager _applicationManager;

        public AuthorizationController(IOpenIddictApplicationManager applicationManager)
            => _applicationManager = applicationManager;

        [HttpPost("~/connect/token"), Produces("application/json")]
        public async Task<IActionResult> Exchange()
        {
            var request = HttpContext.GetOpenIddictServerRequest();
            if (request is null)
            {
                return BadRequest();
            }
            if (request.ClientId is null)
            {
                return BadRequest("Missing ClientId.");
            }

            if (!request.IsClientCredentialsGrantType())
            {
                throw new NotImplementedException("The specified grant is not implemented.");
            }

            var application =
                await _applicationManager.FindByClientIdAsync(request.ClientId) ??
                throw new InvalidOperationException("The application cannot be found.");

            var identity = new ClaimsIdentity(
                TokenValidationParameters.DefaultAuthenticationType,
                Claims.Name, Claims.Role);

            var applicationClientId =
                await _applicationManager.GetClientIdAsync(application) ??
                throw new InvalidOperationException($"Failed fetching applicationClientId.");

            identity.AddClaim(Claims.Subject,
                applicationClientId,
                Destinations.AccessToken, Destinations.IdentityToken);

            var applicationDisplayName =
                await _applicationManager.GetDisplayNameAsync(application) ??
                throw new InvalidOperationException("Missing applicationDisplayName.");

            identity.AddClaim(Claims.Name,
                applicationDisplayName,
                Destinations.AccessToken, Destinations.IdentityToken);

            return SignIn(new ClaimsPrincipal(identity),
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
    }
}

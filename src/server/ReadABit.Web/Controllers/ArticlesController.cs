using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using ReadABit.Core.Integrations.Services;
using ReadABit.Infrastructure.Models;
using ReadABit.Web.Controller.Utils;

namespace ReadABit.Web.Controller
{
    public class ArticlesController : ApiControllerBase
    {
        public ArticlesController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Article>> GetArticle(Guid id)
        {
            // TODO: Remove dummy code that was used for POC.
            return new Article
            {
                Id = id,
                Title = "Hello world!",
            };
        }

        // TODO: Dummy controller, should be removed soon.
        [Route("Conllu")]
        [HttpGet]
        public async Task<ActionResult<string>> Conllu(string input = "")
        {
            var pipe = new UDPipeV1Service(UDPipeV1Service.ModelLanguage.Swedish);
            var conllu = pipe.ConvertToConllu(input);
            return conllu;
        }

        // TODO: Dummy controller, should be removed soon.
        [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<ActionResult<ApplicationUser?>> GetUserInfo()
        {
            var user = await UserManager.FindByIdAsync(RequestContext.UserId.ToString());
            return user;
        }
    }
}

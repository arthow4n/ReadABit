using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReadABit.Core.Integrations.Services;
using ReadABit.Infrastructure.Models;
using ReadABit.Web.Controller.Utils;

namespace ReadABit.Web.Controller
{
    public class ArticlesController : ApiControllerBase
    {
        public ArticlesController(IServiceProvider serviceProvider, IMediator mediator) : base(serviceProvider, mediator)
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
        public async Task<ContentResult> Conllu(string input = "")
        {
            var pipe = new UDPipeV1Service(UDPipeV1Service.ModelLanguage.Swedish);
            var conllu = pipe.ConvertToConllu(input);
            return Content(conllu);
        }
    }
}

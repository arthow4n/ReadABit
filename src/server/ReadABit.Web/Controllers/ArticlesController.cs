using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReadABit.Infrastructure;
using ReadABit.Infrastructure.Models;
using ReadABit.Web.Controller.Utils;
using ReadABit.Web.Integrations.Services;

namespace ReadABit.Web.Controller
{
    public class ArticlesController : ApiControllerBase
    {
        private readonly CoreDbContext context;

        public ArticlesController(CoreDbContext context)
        {
            this.context = context;
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

            var article = await context.Articles.FindAsync(id);

            if (article == null)
            {
                return NotFound();
            }

            return article;
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

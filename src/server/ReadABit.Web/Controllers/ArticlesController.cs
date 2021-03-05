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

        // [HttpGet("{id}")]
        // public async Task<ActionResult<Article>> GetArticle(Guid id)
        // {

        // }
    }
}

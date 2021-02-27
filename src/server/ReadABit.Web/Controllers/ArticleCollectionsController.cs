using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReadABit.Core.Services;
using ReadABit.Infrastructure;
using ReadABit.Web.Controller.Utils;

namespace ReadABit.Web.Controllers
{
    public class ArticleCollectionsController : ApiControllerBase
    {
        public ArticleCollectionsController(CoreDbContext context) : base(context)
        {
        }

        [HttpGet]
        public async Task<List<int>> List()
        {
            var service = new ArticleCollectionService(serviceProvider);
            service.List();
            return new List<int> { };
        }
    }
}
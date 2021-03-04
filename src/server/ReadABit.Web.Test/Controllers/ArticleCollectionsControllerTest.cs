using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReadABit.Core.Utils;
using ReadABit.Infrastructure.Models;
using ReadABit.Web.Controllers;
using ReadABit.Web.Test.Helpers;
using Shouldly;
using Xunit;

namespace ReadABit.Web.Test.Controllers
{
    public class ArticleCollectionsControllerTest : TestBase<ArticleCollectionsController>
    {
        public ArticleCollectionsControllerTest(IServiceProvider serviceProvider, IRequestContext requestContext) : base(serviceProvider, requestContext)
        {
        }

        [Fact]
        public async Task Create_Succeeds()
        {
            var name = "dummy";

            var result = (await T1.CreateArticleCollection(name)).ShouldBeOfType<CreatedAtActionResult>();
            var id = result.Value.ShouldBeOfType<ArticleCollection>().Id;
            var created =
                (await T1.GetArticleCollection(id)).ShouldBeOfType<OkObjectResult>()
                .Value.ShouldBeOfType<ArticleCollection>();

            created.Id.ShouldBe(id);
            created.Name.ShouldBe(name);
        }
    }
}

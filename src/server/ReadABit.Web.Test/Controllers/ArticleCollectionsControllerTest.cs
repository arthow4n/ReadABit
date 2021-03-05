using System;
using System.Collections.Generic;
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
        public async Task CRUD_Succeeds()
        {
            var name = "dummy";

            var creationResult = (await T1.CreateArticleCollection(name)).ShouldBeOfType<CreatedAtActionResult>();
            var createdId = creationResult.Value.ShouldBeOfType<ArticleCollection>().Id;

            (await List()).Count.ShouldBe(1);

            var created =
                (await T1.GetArticleCollection(createdId)).ShouldBeOfType<OkObjectResult>()
                .Value.ShouldBeOfType<ArticleCollection>();

            created.Id.ShouldBe(createdId);
            created.Name.ShouldBe(name);

            (await T1.DeleteArticleCollection(createdId)).ShouldBeOfType<NoContentResult>();
            (await List()).Count.ShouldBe(0);
            (await T1.GetArticleCollection(createdId)).ShouldBeOfType<NotFoundResult>();
        }

        private async Task<List<ArticleCollection>> List()
        {
            return (await T1.ListArticleCollections()).ShouldBeOfType<OkObjectResult>()
                .Value.ShouldBeOfType<List<ArticleCollection>>();
        }
    }
}

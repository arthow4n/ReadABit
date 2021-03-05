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
            var languageCode = "sv";
            var name = "dummy";

            var creationResult = (await T1.CreateArticleCollection(languageCode, name)).ShouldBeOfType<CreatedAtActionResult>();
            var createdId = creationResult.Value.ShouldBeOfType<ArticleCollection>().Id;

            (await List()).Count.ShouldBe(1);

            var created = await Get(createdId);

            created.Id.ShouldBe(createdId);
            created.Name.ShouldBe(name);
            created.LanguageCode.ShouldBe(languageCode);

            var updatedName = "updated";
            var updatedLanguadeCode = "en";
            await T1.UpdateArticleCollection(createdId, updatedLanguadeCode, updatedName);

            var updated = await Get(createdId);
            updated.Id.ShouldBe(createdId);
            updated.Name.ShouldBe(updatedName);
            updated.LanguageCode.ShouldBe(updatedLanguadeCode);

            (await T1.DeleteArticleCollection(createdId)).ShouldBeOfType<NoContentResult>();
            (await List()).Count.ShouldBe(0);
            (await T1.GetArticleCollection(createdId)).ShouldBeOfType<NotFoundResult>();
        }

        private async Task<List<ArticleCollection>> List()
        {
            return (await T1.ListArticleCollections()).ShouldBeOfType<OkObjectResult>()
                .Value.ShouldBeOfType<List<ArticleCollection>>();
        }

        private async Task<ArticleCollection> Get(Guid id)
        {
            return (await T1.GetArticleCollection(id)).ShouldBeOfType<OkObjectResult>()
                .Value.ShouldBeOfType<ArticleCollection>();
        }
    }
}

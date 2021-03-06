using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReadABit.Core.Commands;
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

            var creationResult =
                (await T1.CreateArticleCollection(new ArticleCollectionCreate
                {
                    Name = name,
                    LanguageCode = languageCode,
                    Public = true,
                }))
                .ShouldBeOfType<CreatedAtActionResult>();
            var createdId = creationResult.Value.ShouldBeOfType<ArticleCollection>().Id;

            (await List(languageCode)).Count.ShouldBe(1);

            var created = await Get(createdId);
            created.Id.ShouldBe(createdId);
            created.Name.ShouldBe(name);
            created.LanguageCode.ShouldBe(languageCode);
            created.Public.ShouldBe(true);

            using (AnotherUser)
            {
                (await T1.UpdateArticleCollection(createdId, new ArticleCollectionUpdate
                {
                    LanguageCode = "en",
                    Name = "another",
                    Public = false,
                })).ShouldBeOfType<NotFoundResult>();
            }

            var updatedName = "updated";
            var updatedLanguadeCode = "en";
            await T1.UpdateArticleCollection(createdId, new ArticleCollectionUpdate
            {
                LanguageCode = updatedLanguadeCode,
                Name = updatedName,
                Public = false,
            });

            var updated = await Get(createdId);
            updated.Id.ShouldBe(createdId);
            updated.Name.ShouldBe(updatedName);
            updated.LanguageCode.ShouldBe(updatedLanguadeCode);
            updated.Public.ShouldBe(false);

            (await List("en")).Count.ShouldBe(1);

            // Another user shouldn't be able to see the article collection because it's not public
            using (AnotherUser)
            {
                (await List("en")).Count.ShouldBe(0);
                (await T1.GetArticleCollection(createdId, new ArticleCollectionGet { })).ShouldBeOfType<NotFoundResult>();
            }

            (await T1.DeleteArticleCollection(createdId, new ArticleCollectionDelete { })).ShouldBeOfType<NoContentResult>();
            (await List("en")).Count.ShouldBe(0);
            (await T1.GetArticleCollection(createdId, new ArticleCollectionGet { })).ShouldBeOfType<NotFoundResult>();
        }

        private async Task<List<ArticleCollection>> List(string languageCode)
        {
            return
                (await T1.ListArticleCollections(
                    new ArticleCollectionList
                    {
                        Filter = new ArticleCollectionListFilter
                        {
                            LanguageCode = languageCode,
                        },
                    }))
                    .ShouldBeOfType<OkObjectResult>()
                    .Value
                    .ShouldBeOfType<List<ArticleCollection>>();
        }

        private async Task<ArticleCollection> Get(Guid id)
        {
            return (await T1.GetArticleCollection(id, new ArticleCollectionGet { })).ShouldBeOfType<OkObjectResult>()
                .Value.ShouldBeOfType<ArticleCollection>();
        }
    }
}

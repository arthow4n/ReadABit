using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReadABit.Core.Commands;
using ReadABit.Core.Contracts;
using ReadABit.Core.Utils;
using ReadABit.Web.Controllers;
using ReadABit.Web.Test.Helpers;
using Shouldly;
using Xunit;

namespace ReadABit.Web.Test.Controllers
{
    public class ArticleCollectionsControllerTest : TestBase
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
                (await ArticleCollectionsController.Create(new ArticleCollectionCreate
                {
                    Name = name,
                    LanguageCode = languageCode,
                    Public = true,
                }))
                .ShouldBeOfType<CreatedAtActionResult>();
            var createdId = creationResult.Value.ShouldBeOfType<ArticleCollectionViewModel>().Id;

            (await List(languageCode)).Items.Count.ShouldBe(1);

            var created = await Get(createdId);
            created.Name.ShouldBe(name);
            created.LanguageCode.ShouldBe(languageCode);
            created.Public.ShouldBe(true);

            using (User(2))
            {
                (await ArticleCollectionsController.Update(createdId, new ArticleCollectionUpdate
                {
                    LanguageCode = "en",
                    Name = "another",
                    Public = false,
                })).ShouldBeOfType<NotFoundResult>();
            }

            var updatedName = "updated";
            var updatedLanguadeCode = "en";
            await ArticleCollectionsController.Update(createdId, new ArticleCollectionUpdate
            {
                LanguageCode = updatedLanguadeCode,
                Name = updatedName,
                Public = false,
            });

            var updated = await Get(createdId);
            updated.Name.ShouldBe(updatedName);
            updated.LanguageCode.ShouldBe(updatedLanguadeCode);
            updated.Public.ShouldBe(false);

            (await List("en")).Items.Count.ShouldBe(1);

            // Another user shouldn't be able to see the article collection because it's not public
            using (User(2))
            {
                (await List("en")).Items.Count.ShouldBe(0);
                (await ArticleCollectionsController.Get(createdId, new ArticleCollectionGet { })).ShouldBeOfType<NotFoundResult>();
            }

            (await ArticleCollectionsController.Delete(createdId, new ArticleCollectionDelete { })).ShouldBeOfType<NoContentResult>();
            (await List("en")).Items.Count.ShouldBe(0);
            (await ArticleCollectionsController.Get(createdId, new ArticleCollectionGet { })).ShouldBeOfType<NotFoundResult>();
        }

        private async Task<Paginated<ArticleCollectionListItemViewModel>> List(string languageCode)
        {
            return
                (await ArticleCollectionsController.List(
                    new ArticleCollectionList
                    {
                        Filter = new ArticleCollectionListFilter
                        {
                            LanguageCode = languageCode,
                        },
                        Page = new PageFilter
                        {
                            Index = 1,
                        },
                    }))
                    .ShouldBeOfType<OkObjectResult>()
                    .Value
                    .ShouldBeOfType<Paginated<ArticleCollectionListItemViewModel>>();
        }

        private async Task<ArticleCollectionViewModel> Get(Guid id)
        {
            return (await ArticleCollectionsController.Get(id, new ArticleCollectionGet { })).ShouldBeOfType<OkObjectResult>()
                .Value.ShouldBeOfType<ArticleCollectionViewModel>();
        }
    }
}

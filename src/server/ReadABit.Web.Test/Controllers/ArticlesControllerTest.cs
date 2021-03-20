using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ReadABit.Core.Commands;
using ReadABit.Core.Contracts;
using ReadABit.Core.Utils;
using ReadABit.Infrastructure.Models;
using ReadABit.Web.Controllers;
using ReadABit.Web.Test.Helpers;
using Shouldly;
using Xunit;

namespace ReadABit.Web.Test.Controllers
{
    public class ArticlesControllerTest : TestBase
    {
        public ArticlesControllerTest(IServiceProvider serviceProvider, IRequestContext requestContext) : base(serviceProvider, requestContext)
        {
        }

        [Fact]
        public async Task CRUD_Succeeds()
        {
            var articleCollectionId =
                (await ArticleCollectionsController.Create(new ArticleCollectionCreate
                {
                    LanguageCode = "sv",
                    Name = "collection",
                    Public = false,
                }))
                    .ShouldBeOfType<CreatedAtActionResult>()
                    .Value.ShouldBeOfType<ArticleCollectionViewModel>().Id;

            var name = "dummy";
            var text = "Hallå!";
            var creationResult =
                (await ArticlesController.Create(new ArticleCreate
                {
                    ArticleCollectionId = articleCollectionId,
                    Name = name,
                    Text = text,
                }))
                .ShouldBeOfType<CreatedAtActionResult>();
            var createdId = creationResult.Value.ShouldBeOfType<ArticleViewModel>().Id;

            (await List(articleCollectionId)).Items.Count.ShouldBe(1);

            // Article in private article collection should not be accessible by another user.
            using (User(2))
            {
                (await ArticlesController.Get(createdId, new ArticleGet { })).ShouldBeOfType<NotFoundResult>();
            }

            await ArticleCollectionsController.Update(articleCollectionId, new ArticleCollectionUpdate { Public = true });

            // Article in public article collection should be accessible by another user.
            using (User(2))
            {
                var created = await Get(createdId);
                created.Name.ShouldBe(name);
                JsonConvert.SerializeObject(created.ConlluDocument, Formatting.Indented)
                    .ShouldMatchApproved(c => c.WithDiscriminator("CreatedConlluDocument"));

                (await ArticlesController.Update(createdId, new ArticleUpdate
                {
                    Name = "another",
                    Text = "another",
                })).ShouldBeOfType<NotFoundResult>();
            }

            var updatedName = "updated";
            var upadtedText =
@"Hallå värld!

Han är min pappa,
hon är min mamma,
de är mina föräldrar.



Varför vill du lära dig svenska?
Det beror på att det gör det lättare att förstå vad folk säger.
";
            await ArticlesController.Update(createdId, new ArticleUpdate
            {
                Name = updatedName,
                Text = upadtedText,
            });

            var updated = await Get(createdId);
            updated.Name.ShouldBe(updatedName);
            JsonConvert.SerializeObject(updated.ConlluDocument, Formatting.Indented)
                .ShouldMatchApproved(c => c.WithDiscriminator("UpdatedConlluDocument"));

            using (User(2))
            {
                (await ArticlesController.Delete(createdId, new ArticleDelete { })).ShouldBeOfType<NotFoundResult>();
            }
            (await List(articleCollectionId)).Items.Count.ShouldBe(1);

            (await ArticlesController.Delete(createdId, new ArticleDelete { })).ShouldBeOfType<NoContentResult>();
            (await List(articleCollectionId)).Items.Count.ShouldBe(0);
            (await ArticlesController.Get(createdId, new ArticleGet { })).ShouldBeOfType<NotFoundResult>();
        }

        private async Task<Paginated<ArticleListItemViewModel>> List(Guid articleCollectionId)
        {
            return (await ArticlesController.List(new ArticleList
            {
                ArticleCollectionId = articleCollectionId,
                Page = new PageFilter
                {
                    Index = 1,
                },
            })).ShouldBeOfType<OkObjectResult>()
                .Value.ShouldBeOfType<Paginated<ArticleListItemViewModel>>();
        }

        private async Task<ArticleViewModel> Get(Guid id)
        {
            return (await ArticlesController.Get(id, new ArticleGet { })).ShouldBeOfType<OkObjectResult>()
                .Value.ShouldBeOfType<ArticleViewModel>();
        }
    }
}

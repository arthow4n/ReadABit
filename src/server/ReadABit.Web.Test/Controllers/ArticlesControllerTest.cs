using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ReadABit.Core.Commands;
using ReadABit.Core.Contracts;
using ReadABit.Core.Utils;
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
            var created = await SetupArticle(
                articleCollectionLanguageCode: "sv",
                articleCollectionName: "collection",
                articleCollectionIsPublic: false,
                articleName: "dummy",
                articleText: "Hallå!"
            );

            var articleCollectionId = created.ArticleCollectionId;

            (await List(articleCollectionId)).Items.Count.ShouldBe(1);

            // Article in private article collection should not be accessible by another user.
            using (User(2))
            {
                (await ArticlesController.Get(created.Id, new ArticleGet { })).ShouldBeOfType<NotFoundResult>();
            }

            await ArticleCollectionsController.Update(articleCollectionId, new ArticleCollectionUpdate { Public = true });

            // Article in public article collection should be accessible by another user.
            using (User(2))
            {

                (await Get(created.Id)).ShouldSatisfyAllConditions(
                    x => x.Name.ShouldBe(created.Name),
                    x => JsonConvert
                            .SerializeObject(x.ConlluDocument, Formatting.Indented)
                            .ShouldMatchApproved(c => c.WithDiscriminator("CreatedConlluDocument"))
                );

                (await ArticlesController.Update(created.Id, new ArticleUpdate
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
            await ArticlesController.Update(created.Id, new ArticleUpdate
            {
                Name = updatedName,
                Text = upadtedText,
            });

            var updated = await Get(created.Id);
            updated.Name.ShouldBe(updatedName);
            JsonConvert.SerializeObject(updated.ConlluDocument, Formatting.Indented)
                .ShouldMatchApproved(c => c.WithDiscriminator("UpdatedConlluDocument"));

            using (User(2))
            {
                (await ArticlesController.Delete(created.Id, new ArticleDelete { })).ShouldBeOfType<NotFoundResult>();
            }
            (await List(articleCollectionId)).Items.Count.ShouldBe(1);

            // Listing without specifying article collection ID should return articles from any article collection
            (await List(null)).Items.Count.ShouldBe(1);

            (await ArticlesController.Delete(created.Id, new ArticleDelete { })).ShouldBeOfType<NoContentResult>();
            (await List(articleCollectionId)).Items.Count.ShouldBe(0);
            (await ArticlesController.Get(created.Id, new ArticleGet { })).ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task ArticleReadingProgress_ShouldBeAbleToReadAndWrite()
        {
            var created = await SetupArticle(
                articleCollectionLanguageCode: "sv",
                articleCollectionIsPublic: true,
                articleText: "Jag kommer att cykla till stan."
            );

            // Should default to 0 if never read.
            (await Get(created.Id)).ReadingProgress.ShouldSatisfyAllConditions(
                x => x.ReadRatio.ShouldBe(0),
                x => x.ConlluTokenPointer.ShouldBe(new())
            );

            var upsertRequest = new ArticleReadingProgressUpsert()
            {
                ConlluTokenPointer = new()
                {
                    DocumentIndex = 0,
                    ParagraphIndex = 1,
                    SentenceIndex = 2,
                    TokenIndex = 3,
                },
                ReadRatio = 0.5m,
            };
            await ArticlesController.UpsertReadingProgress(created.Id, upsertRequest);

            (await Get(created.Id)).ReadingProgress.ShouldSatisfyAllConditions(
                x => x.ReadRatio.ShouldBe(upsertRequest.ReadRatio),
                x => x.ConlluTokenPointer.ShouldBe(upsertRequest.ConlluTokenPointer)
            );

            // Should not mix read progress between users.
            using (User(2))
            {
                (await Get(created.Id)).ReadingProgress.ShouldSatisfyAllConditions(
                    x => x.ReadRatio.ShouldBe(0),
                    x => x.ConlluTokenPointer.ShouldBe(new())
                );
            }
        }

        private async Task<Paginated<ArticleListItemViewModel>> List(Guid? articleCollectionId)
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

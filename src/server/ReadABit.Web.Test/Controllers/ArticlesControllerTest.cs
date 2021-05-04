using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Testing;
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
            (await List(null)).Items.Single().ReadRadio.ShouldBe(0);
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

            (await List(null)).Items.Single().ReadRadio.ShouldBe(upsertRequest.ReadRatio);
            (await Get(created.Id)).ReadingProgress.ShouldSatisfyAllConditions(
                x => x.ReadRatio.ShouldBe(upsertRequest.ReadRatio),
                x => x.ConlluTokenPointer.ShouldBe(upsertRequest.ConlluTokenPointer)
            );

            // Should not mix read progress between users.
            using (User(2))
            {
                (await List(null)).Items.Single().ReadRadio.ShouldBe(0);
                (await Get(created.Id)).ReadingProgress.ShouldSatisfyAllConditions(
                    x => x.ReadRatio.ShouldBe(0),
                    x => x.ConlluTokenPointer.ShouldBe(new())
                );
            }
        }

        [Fact]
        public async Task SortBy_LastAccessed()
        {
            FakeClock.AutoAdvance = Duration.FromSeconds(1);

            var articles = new List<ArticleViewModel>
            {
            };

            articles.Add(await SetupArticle());
            articles.Add(await SetupArticle());
            articles.Add(await SetupArticle());

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

            await ArticlesController.UpsertReadingProgress(articles[1].Id, upsertRequest);
            await ArticlesController.UpsertReadingProgress(articles[2].Id, upsertRequest);
            await ArticlesController.UpsertReadingProgress(articles[0].Id, upsertRequest);

            (await List(null, SortBy.LastAccessed))
                .Items
                .Select(x => x.Id)
                .ShouldBe(new List<Guid>
                {
                    articles[0].Id,
                    articles[2].Id,
                    articles[1].Id,
                });
        }

        #region Unicode normalisation
        /// <summary>
        // UDPipe v1 seems to yield malformed output when given an input that's not normalised,
        // thus we need to normalise the input before processing it with UDPipe v1.
        //
        // These tests are placed here because we might want to switch the underlying CoNLL-U generator in the future.
        /// </summary>
        [Fact]
        public async Task Create_ShouldNormaliseUnicodeContentBeforeProcessingConlluConversion()
        {
            var article = await SetupArticle(
                articleName: Uri.UnescapeDataString("a%CC%88ven"),
                articleText: Uri.UnescapeDataString("a%CC%88ven")
            );

            // Example error if we don't do the normalisation:
            // Difference     |  |    |    |    |    |   
            //                | \|/  \|/  \|/  \|/  \|/  
            // Index          | 0    1    2    3    4    
            // Expected Value | ä    v    e    n         
            // Actual Value   | a    ?    v    e    n    
            // Expected Code  | 228  118  101  110       
            // Actual Code    | 97   63   118  101  110  

            (await Get(article.Id))
                .ShouldSatisfyAllConditions(
                    x => x.Name.ShouldBe(Uri.UnescapeDataString("%C3%A4ven")),
                    x => x.ConlluDocument
                        .Paragraphs.Single()
                        .Sentences.Single()
                        .Tokens.Single()
                        .Form.ShouldBe(Uri.UnescapeDataString("%C3%A4ven"))
                );
        }

        [Fact]
        public async Task Update_ShouldNormaliseUnicodeContentBeforeProcessingConlluConversion()
        {
            var article = await SetupArticle(
                articleName: Uri.UnescapeDataString("a%CC%88ven"),
                articleText: Uri.UnescapeDataString("a%CC%88ven")
            );

            await ArticlesController.Update(article.Id, new()
            {
                Name = Uri.UnescapeDataString("a%CC%8Atta"),
                Text = Uri.UnescapeDataString("a%CC%8Atta"),
            });

            (await Get(article.Id))
                .ShouldSatisfyAllConditions(
                    x => x.Name.ShouldBe(Uri.UnescapeDataString("%C3%A5tta")),
                    x => x.ConlluDocument
                        .Paragraphs.Single()
                        .Sentences.Single()
                        .Tokens.Single()
                        .Form.ShouldBe(Uri.UnescapeDataString("%C3%A5tta"))
                );
        }
        #endregion

        private async Task<Paginated<ArticleListItemViewModel>> List(Guid? articleCollectionId, SortBy sortBy = default)
        {
            return (await ArticlesController.List(new ArticleList
            {
                ArticleCollectionId = articleCollectionId,
                Page = new PageFilter
                {
                    Index = 1,
                },
                SortBy = sortBy,
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

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
    public class ArticlesControllerTest : TestBase<ArticlesController, ArticleCollectionsController>
    {
        public ArticlesControllerTest(IServiceProvider serviceProvider, IRequestContext requestContext) : base(serviceProvider, requestContext)
        {
        }

        [Fact]
        public async Task CRUD_Succeeds()
        {
            var articleCollectionId =
                (await T2.CreateArticleCollection(new ArticleCollectionCreate
                {
                    LanguageCode = "sv",
                    Name = "collection",
                    Public = false,
                }))
                    .ShouldBeOfType<CreatedAtActionResult>()
                    .Value.ShouldBeOfType<ArticleCollection>().Id;

            var name = "dummy";
            var text = "Hallå!";
            var creationResult =
                (await T1.CreateArticle(new ArticleCreate
                {
                    ArticleCollectionId = articleCollectionId,
                    Name = name,
                    Text = text,
                }))
                .ShouldBeOfType<CreatedAtActionResult>();
            var createdId = creationResult.Value.ShouldBeOfType<Article>().Id;

            (await List(articleCollectionId)).Count.ShouldBe(1);

            // Article in private article collection should not be accessible by another user.
            using (User(2))
            {
                (await T1.GetArticle(createdId, new ArticleGet { })).ShouldBeOfType<NotFoundResult>();
            }

            await T2.UpdateArticleCollection(articleCollectionId, new ArticleCollectionUpdate { Public = true });

            // Article in public article collection should be accessible by another user.
            using (User(2))
            {
                var created = await Get(createdId);
                created.Name.ShouldBe(name);
                created.Text.ShouldBe(text);
                created.Conllu.ShouldContain("# text = Hallå!");
                created.Conllu.ShouldMatchApproved(c => c.WithDiscriminator("CreatedConllu"));

                (await T1.UpdateArticle(createdId, new ArticleUpdate
                {
                    Name = "another",
                    Text = "another",
                })).ShouldBeOfType<NotFoundResult>();
            }

            var updatedName = "updated";
            var upadtedText = "Hallå värld!";
            await T1.UpdateArticle(createdId, new ArticleUpdate
            {
                Name = updatedName,
                Text = upadtedText,
            });

            var updated = await Get(createdId);
            updated.Name.ShouldBe(updatedName);
            updated.Text.ShouldBe(upadtedText);
            updated.Conllu.ShouldContain("# text = Hallå värld!");
            updated.Conllu.ShouldMatchApproved(c => c.WithDiscriminator("UpdatedConllu"));

            using (User(2))
            {
                (await T1.DeleteArticle(createdId, new ArticleDelete { })).ShouldBeOfType<NotFoundResult>();
            }
            (await List(articleCollectionId)).Count.ShouldBe(1);

            (await T1.DeleteArticle(createdId, new ArticleDelete { })).ShouldBeOfType<NoContentResult>();
            (await List(articleCollectionId)).Count.ShouldBe(0);
            (await T1.GetArticle(createdId, new ArticleGet { })).ShouldBeOfType<NotFoundResult>();
        }

        private async Task<List<Article>> List(Guid articleCollectionId)
        {
            return (await T1.ListArticles(new ArticleList { ArticleCollectionId = articleCollectionId })).ShouldBeOfType<OkObjectResult>()
                .Value.ShouldBeOfType<List<Article>>();
        }

        private async Task<Article> Get(Guid id)
        {
            return (await T1.GetArticle(id, new ArticleGet { })).ShouldBeOfType<OkObjectResult>()
                .Value.ShouldBeOfType<Article>();
        }
    }
}

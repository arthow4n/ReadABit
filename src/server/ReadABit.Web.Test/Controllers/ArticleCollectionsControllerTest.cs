using System;
using System.Threading.Tasks;
using ReadABit.Web.Controllers;
using ReadABit.Web.Test.Helpers;
using Shouldly;
using Xunit;

namespace ReadABit.Web.Test.Controllers
{
    public class ArticleCollectionsControllerTest : TestBase<ArticleCollectionsController>
    {
        private readonly ArticleCollectionsController controller;

        public ArticleCollectionsControllerTest(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            controller = t1;
        }

        [Fact]
        public async Task Create_Succeeds()
        {
            var name = "dummy";

            var id = await controller.CreateArticleCollection(name);
            var created = await controller.GetArticleCollection(id);

            created.Id.ShouldBe(id);
            created.Name.ShouldBe(name);
        }
    }
}
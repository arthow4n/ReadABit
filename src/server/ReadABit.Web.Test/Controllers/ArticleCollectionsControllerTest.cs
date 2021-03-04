using System;
using System.Threading.Tasks;
using ReadABit.Core.Utils;
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

            var id = await t1.CreateArticleCollection(name);
            var created = await t1.GetArticleCollection(id);

            created.Id.ShouldBe(id);
            created.Name.ShouldBe(name);
        }
    }
}

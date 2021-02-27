using Xunit;
using ReadABit.Web.Controller;
using ReadABit.Infrastructure;
using System.Threading.Tasks;
using Shouldly;

namespace ReadABit.Web.Test
{
    public class ArticlesControllerTest
    {
        private readonly ArticlesController controller;

        public ArticlesControllerTest(CoreDbContext context)
        {
            this.controller = new ArticlesController(context);
        }

        [Fact]
        public async Task Conllu_Runs()
        {
            var response = await controller.Conllu("Hallå värld!");
            response.Content.ShouldContain("# text = hallå värld");
        }
    }
}

using Xunit;
using ReadABit.Web.Controller;
using System.Threading.Tasks;
using Shouldly;
using MediatR;

namespace ReadABit.Web.Test
{
    public class ArticlesControllerTest
    {
        private readonly ArticlesController controller;

        public ArticlesControllerTest(IMediator mediator)
        {
            this.controller = new ArticlesController(mediator);
        }

        [Fact]
        public async Task Conllu_Runs()
        {
            var response = await controller.Conllu("Hallå värld!");
            response.Content.ShouldContain("# text = hallå värld");
        }
    }
}

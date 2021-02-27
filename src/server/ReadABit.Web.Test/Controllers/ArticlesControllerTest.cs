using System;
using Xunit;
using ReadABit.Web.Controller;
using System.Threading.Tasks;
using Shouldly;
using MediatR;
using ReadABit.Web.Test.Helpers;

namespace ReadABit.Web.Test
{
    public class ArticlesControllerTest : TestBase<ArticlesController>
    {
        private readonly ArticlesController controller;

        public ArticlesControllerTest(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            controller = t1;
        }

        [Fact]
        public async Task Conllu_Runs()
        {
            var response = await controller.Conllu("Hallå värld!");
            response.Content.ShouldContain("# text = hallå värld");
        }
    }
}

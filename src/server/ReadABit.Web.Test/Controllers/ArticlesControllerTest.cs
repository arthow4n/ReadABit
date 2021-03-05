using System;
using Xunit;
using ReadABit.Web.Controller;
using System.Threading.Tasks;
using Shouldly;
using ReadABit.Web.Test.Helpers;
using ReadABit.Core.Utils;

namespace ReadABit.Web.Test
{
    public class ArticlesControllerTest : TestBase<ArticlesController>
    {
        public ArticlesControllerTest(IServiceProvider serviceProvider, IRequestContext requestContext) : base(serviceProvider, requestContext)
        {
        }

        // [Fact]
        // public async Task Conllu_Runs()
        // {
        //     var response = await T1.Conllu("Hallå värld!");
        //     response.Value.ShouldContain("# text = hallå värld");
        // }
    }
}

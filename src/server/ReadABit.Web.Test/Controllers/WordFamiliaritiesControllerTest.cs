using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReadABit.Core.Commands;
using ReadABit.Core.Contracts;
using ReadABit.Core.Utils;
using ReadABit.Web.Controllers;
using ReadABit.Web.Test.Helpers;
using Shouldly;
using Xunit;

namespace ReadABit.Web.Test.Controllers
{
    public class WordFamiliaritiesControllerTest : TestBase
    {
        public WordFamiliaritiesControllerTest(IServiceProvider serviceProvider, IRequestContext requestContext) : base(serviceProvider, requestContext)
        {
        }

        [Fact]
        public async Task CRUD_Succeeds()
        {
            await WordFamiliaritiesController.Upsert(new WordFamiliarityUpsert
            {
                Level = 1,
                Word = new WordSelector
                {
                    LanguageCode = "sv",
                    Expression = "Hallå",
                },
            });
            (await List()).ShouldSatisfyAllConditions(
                x => x.Flatten()
                    .ShouldHaveSingleItem()
                    .ShouldSatisfyAllConditions(
                        vm => vm.Level.ShouldBe(1),
                        vm => vm.WordLanguageCode.ShouldBe("sv"),
                        vm => vm.WordExpression.ShouldBe("Hallå")
                    ),
                x => x.GroupedWordFamiliarities["sv"]["Hallå"].ShouldSatisfyAllConditions(
                        vm => vm.Level.ShouldBe(1),
                        vm => vm.WordLanguageCode.ShouldBe("sv"),
                        vm => vm.WordExpression.ShouldBe("Hallå")
                    )
            );

            using (User(2))
            {
                (await List()).Flatten().ShouldBeEmpty();
            }

            await WordFamiliaritiesController.Upsert(new WordFamiliarityUpsert
            {
                Level = 2,
                Word = new WordSelector
                {
                    LanguageCode = "sv",
                    Expression = "Hallå",
                },
            });
            (await List()).ShouldSatisfyAllConditions(
                x => x.Flatten()
                    .ShouldHaveSingleItem()
                    .ShouldSatisfyAllConditions(
                        vm => vm.Level.ShouldBe(2),
                        vm => vm.WordLanguageCode.ShouldBe("sv"),
                        vm => vm.WordExpression.ShouldBe("Hallå")
                    ),
                x => x.GroupedWordFamiliarities["sv"]["Hallå"].ShouldSatisfyAllConditions(
                        vm => vm.Level.ShouldBe(2),
                        vm => vm.WordLanguageCode.ShouldBe("sv"),
                        vm => vm.WordExpression.ShouldBe("Hallå")
                    )
            );

            var wordFamiliarityId = (await List()).Flatten().Single().Id;

            using (User(2))
            {
                await WordFamiliaritiesController.Delete(wordFamiliarityId, new WordFamiliarityDelete { });
            }
            (await List()).Flatten().ShouldNotBeEmpty();

            await WordFamiliaritiesController.Delete(wordFamiliarityId, new WordFamiliarityDelete { });
            (await List()).Flatten().ShouldBeEmpty();
        }

        private async Task<WordFamiliarityListViewModel> List()
        {
            return (await WordFamiliaritiesController.List(new WordFamiliarityList { }))
                .ShouldBeOfType<OkObjectResult>()
                .Value.ShouldBeOfType<WordFamiliarityListViewModel>();
        }
    }
}

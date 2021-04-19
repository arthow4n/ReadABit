using System;
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
            // TODO: Add test for daily goal

            await WordFamiliaritiesController.UpsertBatch(new()
            {
                Level = 1,
                Words = new()
                {
                    new()
                    {
                        LanguageCode = "sv",
                        Expression = "Hallå",
                    }
                },
            });
            (await List()).ShouldSatisfyAllConditions(
                x => x.Flatten()
                    .ShouldHaveSingleItem()
                    .ShouldSatisfyAllConditions(
                        vm => vm.Level.ShouldBe(1),
                        vm => vm.Word.LanguageCode.ShouldBe("sv"),
                        vm => vm.Word.Expression.ShouldBe("Hallå")
                    ),
                x => x.GroupedWordFamiliarities["sv"]["Hallå"].ShouldSatisfyAllConditions(
                        vm => vm.Level.ShouldBe(1),
                        vm => vm.Word.LanguageCode.ShouldBe("sv"),
                        vm => vm.Word.Expression.ShouldBe("Hallå")
                    )
            );

            using (User(2))
            {
                (await List()).Flatten().ShouldBeEmpty();
            }

            await WordFamiliaritiesController.UpsertBatch(new()
            {
                Level = 2,
                Words = new()
                {
                    new()
                    {
                        LanguageCode = "sv",
                        Expression = "Hallå",
                    },
                },
            });
            (await List()).ShouldSatisfyAllConditions(
                x => x.Flatten()
                    .ShouldHaveSingleItem()
                    .ShouldSatisfyAllConditions(
                        vm => vm.Level.ShouldBe(2),
                        vm => vm.Word.LanguageCode.ShouldBe("sv"),
                        vm => vm.Word.Expression.ShouldBe("Hallå")
                    ),
                x => x.GroupedWordFamiliarities["sv"]["Hallå"].ShouldSatisfyAllConditions(
                        vm => vm.Level.ShouldBe(2),
                        vm => vm.Word.LanguageCode.ShouldBe("sv"),
                        vm => vm.Word.Expression.ShouldBe("Hallå")
                    )
            );

            // Setting a word familiarity level to 0 should delete it instead,
            // because uncreated word familiarity is treated as 0.
            await WordFamiliaritiesController.UpsertBatch(new()
            {
                Level = 0,
                Words = new()
                {
                    new()
                    {
                        LanguageCode = "sv",
                        Expression = "Hallå",
                    },
                },
            });
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

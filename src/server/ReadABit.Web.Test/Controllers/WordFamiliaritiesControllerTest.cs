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
            await SetupWordFamiliarity(1, "sv", new() { "Hallå" });
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

            await SetupWordFamiliarity(2, "sv", new() { "Hallå" });
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
            await SetupWordFamiliarity(0, "sv", new() { "Hallå" });
            (await List()).Flatten().ShouldBeEmpty();
        }

        [Fact]
        public async Task UpsertBatch_DailyGoal_CountsCorrectly()
        {
            #region day 1
            FakeClock.SetToIso("2020-03-01T11:00:00+08:00");

            await UserPreferencesController.Upsert(new()
            {
                Data = new()
                {
                    // +08:00
                    DailyGoalResetTimeTimeZone = "Asia/Taipei",
                    DailyGoalResetTimePartial = "12:00:00",
                    DailyGoalNewlyCreatedWordFamiliarityCount = 4,
                },
            });

            (await SetupWordFamiliarity(1, "sv", new() { "a" }))
                .DailyGoalStatus
                .ShouldSatisfyAllConditions(
                    x => x.NewlyCreated.ShouldBe(1),
                    x => x.NewlyCreatedGoal.ShouldBe(4),
                    x => x.NewlyCreatedReached.ShouldBeFalse()
                );

            // Should yield the same result as what we got from Upsert's return.
            (await DailyGoalCheck())
                .ShouldSatisfyAllConditions(
                    x => x.NewlyCreated.ShouldBe(1),
                    x => x.NewlyCreatedGoal.ShouldBe(4),
                    x => x.NewlyCreatedReached.ShouldBeFalse()
                );

            // Already learned word shouldn't count.
            (await SetupWordFamiliarity(3, "sv", new() { "a" }))
                .DailyGoalStatus
                .ShouldSatisfyAllConditions(
                    x => x.NewlyCreated.ShouldBe(1),
                    x => x.NewlyCreatedGoal.ShouldBe(4),
                    x => x.NewlyCreatedReached.ShouldBeFalse()
                );

            (await SetupWordFamiliarity(2, "sv", new() { "b", "c" }))
                .DailyGoalStatus
                .ShouldSatisfyAllConditions(
                    x => x.NewlyCreated.ShouldBe(3),
                    x => x.NewlyCreatedGoal.ShouldBe(4),
                    x => x.NewlyCreatedReached.ShouldBeFalse()
                );

            (await SetupWordFamiliarity(3, "sv", new() { "d" }))
                .DailyGoalStatus
                .ShouldSatisfyAllConditions(
                    x => x.NewlyCreated.ShouldBe(4),
                    x => x.NewlyCreatedGoal.ShouldBe(4),
                    x => x.NewlyCreatedReached.ShouldBeTrue()
                );

            // Word "marked as new" (removed) shouldn't count.
            (await SetupWordFamiliarity(0, "sv", new() { "d" }))
                .DailyGoalStatus
                .ShouldSatisfyAllConditions(
                    x => x.NewlyCreated.ShouldBe(3),
                    x => x.NewlyCreatedGoal.ShouldBe(4),
                    x => x.NewlyCreatedReached.ShouldBeFalse()
                );

            (await SetupWordFamiliarity(1, "sv", new() { "d", "e" }))
                .DailyGoalStatus
                .ShouldSatisfyAllConditions(
                    x => x.NewlyCreated.ShouldBe(5),
                    x => x.NewlyCreatedGoal.ShouldBe(4),
                    x => x.NewlyCreatedReached.ShouldBeTrue()
                );
            #endregion


            #region day 2
            FakeClock.AdvanceDays(1);

            (await SetupWordFamiliarity(2, "sv", new() { "f" }))
                .DailyGoalStatus
                // Should reset state each day.
                .ShouldSatisfyAllConditions(
                    x => x.NewlyCreated.ShouldBe(1),
                    x => x.NewlyCreatedGoal.ShouldBe(4),
                    x => x.NewlyCreatedReached.ShouldBeFalse()
                );
            #endregion
        }

        private async Task<WordFamiliarityListViewModel> List()
        {
            return (await WordFamiliaritiesController.List(new WordFamiliarityList { }))
                .ShouldBeOfType<OkObjectResult>()
                .Value
                .ShouldBeOfType<WordFamiliarityListViewModel>();
        }

        private async Task<WordFamiliarityDailyGoalCheckViewModel> DailyGoalCheck()
        {
            return (await WordFamiliaritiesController.DailyGoalCheck(new()))
                .ShouldBeOfType<OkObjectResult>()
                .Value
                .ShouldBeOfType<WordFamiliarityDailyGoalCheckViewModel>();
        }
    }
}

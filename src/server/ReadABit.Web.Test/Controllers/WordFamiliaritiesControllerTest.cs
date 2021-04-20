using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReadABit.Core.Commands;
using ReadABit.Core.Contracts;
using ReadABit.Core.Utils;
using ReadABit.Web.Contracts;
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
            await UpsertBatch(new()
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

            await UpsertBatch(new()
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
            await UpsertBatch(new()
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

        [Fact]
        public async Task UpsertBatch_DailyGoal_CountsCorrectly()
        {
            #region day 1
            SetFakeClockTo("2020-03-01T11:00:00+08:00");

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

            (await UpsertBatch(new()
            {
                Level = 1,
                Words = new()
                {
                    new()
                    {
                        Expression = "a",
                        LanguageCode = "sv",
                    }
                }
            }))
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
            (await UpsertBatch(new()
            {
                Level = 3,
                Words = new()
                {
                    new()
                    {
                        Expression = "a",
                        LanguageCode = "sv",
                    }
                }
            }))
                .DailyGoalStatus
                .ShouldSatisfyAllConditions(
                    x => x.NewlyCreated.ShouldBe(1),
                    x => x.NewlyCreatedGoal.ShouldBe(4),
                    x => x.NewlyCreatedReached.ShouldBeFalse()
                );

            (await UpsertBatch(new()
            {
                Level = 2,
                Words = new()
                {
                    new()
                    {
                        Expression = "b",
                        LanguageCode = "sv",
                    },
                    new()
                    {
                        Expression = "c",
                        LanguageCode = "sv",
                    }
                }
            }))
                .DailyGoalStatus
                .ShouldSatisfyAllConditions(
                    x => x.NewlyCreated.ShouldBe(3),
                    x => x.NewlyCreatedGoal.ShouldBe(4),
                    x => x.NewlyCreatedReached.ShouldBeFalse()
                );

            (await UpsertBatch(new()
            {
                Level = 3,
                Words = new()
                {
                    new()
                    {
                        Expression = "d",
                        LanguageCode = "sv",
                    }
                }
            }))
                .DailyGoalStatus
                .ShouldSatisfyAllConditions(
                    x => x.NewlyCreated.ShouldBe(4),
                    x => x.NewlyCreatedGoal.ShouldBe(4),
                    x => x.NewlyCreatedReached.ShouldBeTrue()
                );

            // Word "marked as new" (removed) shouldn't count.
            (await UpsertBatch(new()
            {
                Level = 0,
                Words = new()
                {
                    new()
                    {
                        Expression = "d",
                        LanguageCode = "sv",
                    }
                }
            }))
                .DailyGoalStatus
                .ShouldSatisfyAllConditions(
                    x => x.NewlyCreated.ShouldBe(3),
                    x => x.NewlyCreatedGoal.ShouldBe(4),
                    x => x.NewlyCreatedReached.ShouldBeFalse()
                );

            (await UpsertBatch(new()
            {
                Level = 1,
                Words = new()
                {
                    new()
                    {
                        Expression = "d",
                        LanguageCode = "sv",
                    },
                    new()
                    {
                        Expression = "e",
                        LanguageCode = "sv",
                    }
                }
            }))
                .DailyGoalStatus
                .ShouldSatisfyAllConditions(
                    x => x.NewlyCreated.ShouldBe(5),
                    x => x.NewlyCreatedGoal.ShouldBe(4),
                    x => x.NewlyCreatedReached.ShouldBeTrue()
                );
            #endregion


            #region day 2
            FakeClock.AdvanceDays(1);

            (await UpsertBatch(new()
            {
                Level = 2,
                Words = new()
                {
                    new()
                    {
                        Expression = "f",
                        LanguageCode = "sv",
                    }
                }
            }))
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

        private async Task<WordFamiliarityUpsertBatchResultViewModal> UpsertBatch(WordFamiliarityUpsertBatch request)
        {
            return (await WordFamiliaritiesController.UpsertBatch(request))
                .ShouldBeOfType<OkObjectResult>()
                .Value
                .ShouldBeOfType<WordFamiliarityUpsertBatchResultViewModal>();
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

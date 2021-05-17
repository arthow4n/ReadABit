using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReadABit.Core.Contracts;
using ReadABit.Core.Utils;
using ReadABit.Web.Controllers;
using ReadABit.Web.Test.Helpers;
using Shouldly;
using Xunit;

namespace ReadABit.Web.Test.Controllers
{
    public class UserAchievementsControllerTest : TestBase
    {
        public UserAchievementsControllerTest(IServiceProvider serviceProvider, IRequestContext requestContext) : base(serviceProvider, requestContext)
        {
        }

        [Fact]
        public async Task DailyGoalStreak_CalculatesCorrectly()
        {
            #region Day 1
            FakeClock.SetToIso("2020-03-01T11:00:00+08:00");

            await UserPreferencesController.Upsert(new()
            {
                Data = new()
                {
                    // +08:00
                    DailyGoalResetTimeTimeZone = "Asia/Taipei",
                    DailyGoalResetTimePartial = "12:00:00",
                    DailyGoalNewlyCreatedWordFamiliarityCount = 1,
                },
            });


            (await DailyGoalStreak())
                .CurrentStreakDays
                .ShouldBe(0);

            await SetupWordFamiliarity(1, "sv", new() { "a" });

            (await DailyGoalStreak())
                .CurrentStreakDays
                .ShouldBe(1);

            await UserPreferencesController.Upsert(new()
            {
                Data = new()
                {
                    DailyGoalNewlyCreatedWordFamiliarityCount = 2,
                }
            });

            // Changing goal shouldn't affect that day's streak record.
            // This could be changed in the future but at the moment that's how it should work.
            (await DailyGoalStreak())
                .CurrentStreakDays
                .ShouldBe(1);
            #endregion


            #region Day 2
            FakeClock.AdvanceDays(1);
            await SetupWordFamiliarity(1, "sv", new() { "b", "c" });

            await UserPreferencesController.Upsert(new()
            {
                Data = new()
                {
                    DailyGoalNewlyCreatedWordFamiliarityCount = 1,
                }
            });

            (await DailyGoalStreak())
                .CurrentStreakDays
                .ShouldBe(2);
            #endregion

            #region Day 3 - before daily goal reset
            FakeClock.AdvanceDays(1);

            (await DailyGoalStreak())
                .CurrentStreakDays
                .ShouldBe(0);
            #endregion

            // FIXME: Wrong date calculation in command handler, need to take daily goal reset time into consideration.
            // #region Day 3 - after daily goal reset
            // FakeClock.AdvanceHours(2);

            // await SetupWordFamiliarity(1, "sv", new() { "d" });

            // (await DailyGoalStreak())
            //     .CurrentStreakDays
            //     .ShouldBe(1);
            // #endregion

            #region Day 4
            FakeClock.AdvanceDays(1);
            await SetupWordFamiliarity(1, "sv", new() { "e" });

            (await DailyGoalStreak())
                .CurrentStreakDays
                .ShouldBe(1);
            #endregion
        }

        private async Task<UserAchievementsDailyGoalStreakStateViewModel> DailyGoalStreak()
        {
            return (await UserAchievementsController.GetDailyGoalStreakState(new()))
                .ShouldBeOfType<OkObjectResult>()
                .Value.ShouldBeOfType<UserAchievementsDailyGoalStreakStateViewModel>();
        }
    }
}

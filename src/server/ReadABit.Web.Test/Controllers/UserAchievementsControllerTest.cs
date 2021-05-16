using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReadABit.Core.Commands;
using ReadABit.Core.Contracts;
using ReadABit.Core.Utils;
using ReadABit.Infrastructure.Models;
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
            await UserPreferencesController.Upsert(new UserPreferenceUpsert
            {
                Data = new()
                {
                    DailyGoalNewlyCreatedWordFamiliarityCount = 1,
                }
            });

            (await DailyGoalStreak())
                .StreakDays
                .ShouldBe(0);

            await SetupWordFamiliarity(1, "sv", new() { "a" });

            (await DailyGoalStreak())
                .StreakDays
                .ShouldBe(1);

            await UserPreferencesController.Upsert(new UserPreferenceUpsert
            {
                Data = new()
                {
                    DailyGoalNewlyCreatedWordFamiliarityCount = 2,
                }
            });

            // Changing goal shouldn't affect that day's streak record.
            // This could be changed in the future but at the moment that's how it should work.
            (await DailyGoalStreak())
                .StreakDays
                .ShouldBe(1);


            #region day 2
            FakeClock.AdvanceDays(1);
            await SetupWordFamiliarity(1, "sv", new() { "b", "c" });

            (await DailyGoalStreak())
                .StreakDays
                .ShouldBe(2);
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

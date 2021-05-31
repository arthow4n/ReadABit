using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
            #region Day 1
            FakeClock.SetToIso("2020-03-01T11:00:00+08:00");

            var preferenceDataBase = new UserPreferenceData()
            {
                // +08:00
                DailyGoalResetTimeTimeZone = "Asia/Taipei",
                DailyGoalResetTimePartial = "12:00:00",
                DailyGoalNewlyCreatedWordFamiliarityCount = 1,
            };

            await UserPreferencesController.Upsert(new()
            {
                Data = preferenceDataBase with
                {
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
            #endregion


            #region Day 2
            FakeClock.AdvanceDays(1);
            await SetupWordFamiliarity(1, "sv", new() { "b" });

            (await DailyGoalStreak())
                .CurrentStreakDays
                .ShouldBe(2);

            await UserPreferencesController.Upsert(new()
            {
                Data = preferenceDataBase with
                {
                    DailyGoalNewlyCreatedWordFamiliarityCount = 2,
                },
            });

            (await DailyGoalStreak())
                .CurrentStreakDays
                .ShouldBe(1);

            await SetupWordFamiliarity(1, "sv", new() { "c" });
            (await DailyGoalStreak())
                .CurrentStreakDays
                .ShouldBe(2);

            await UserPreferencesController.Upsert(new()
            {
                Data = preferenceDataBase with
                {
                    DailyGoalNewlyCreatedWordFamiliarityCount = 1,
                },
            });

            (await DailyGoalStreak())
                .CurrentStreakDays
                .ShouldBe(2);


            using (User(2))
            {
                await UserPreferencesController.Upsert(new()
                {
                    Data = preferenceDataBase with
                    {
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
            }
            #endregion

            #region Day 3 - before daily goal reset
            FakeClock.AdvanceDays(1);

            (await DailyGoalStreak())
                .CurrentStreakDays
                .ShouldBe(2);
            #endregion

            #region Day 3 - after daily goal reset
            FakeClock.AdvanceHours(2);

            (await DailyGoalStreak())
                .CurrentStreakDays
                .ShouldBe(0);

            await SetupWordFamiliarity(1, "sv", new() { "d" });

            (await DailyGoalStreak())
                .CurrentStreakDays
                .ShouldBe(1);
            #endregion

            #region Day 4 - before daily goal reset
            FakeClock.AdvanceHours(-2);
            FakeClock.AdvanceDays(1);
            await SetupWordFamiliarity(1, "sv", new() { "e" });

            (await DailyGoalStreak())
                .CurrentStreakDays
                .ShouldBe(1);
            #endregion

            #region Day 5
            FakeClock.AdvanceDays(1);
            #endregion

            #region Day 6
            FakeClock.AdvanceDays(1);
            await SetupWordFamiliarity(1, "sv", new() { "f" });

            (await DailyGoalStreak())
                .CurrentStreakDays
                .ShouldBe(1);
            #endregion
        }

        // TODO: Should rename this whole endpoint to something like "get home screen state"
        [Fact]
        public async Task DailyGoalStreak_CountsSeenWords()
        {
            await SetupWordFamiliarity(1, "sv", new() { "a" });

            (await DailyGoalStreak())
                .SeenWordCount
                .ShouldBe(1);

            await SetupWordFamiliarity(-1, "sv", new() { "a" });

            (await DailyGoalStreak())
                .SeenWordCount
                .ShouldBe(0);

            await SetupWordFamiliarity(3, "sv", new() { "a", "b", "c" });

            (await DailyGoalStreak())
                .SeenWordCount
                .ShouldBe(3);
        }

        private async Task<UserAchievementsDailyGoalStreakStateViewModel> DailyGoalStreak()
        {
            return (await UserAchievementsController.GetDailyGoalStreakState(new()))
                .ShouldBeOfType<OkObjectResult>()
                .Value.ShouldBeOfType<UserAchievementsDailyGoalStreakStateViewModel>();
        }
    }
}

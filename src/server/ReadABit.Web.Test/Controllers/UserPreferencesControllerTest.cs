using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReadABit.Core.Commands;
using ReadABit.Core.Utils;
using ReadABit.Infrastructure.Models;
using ReadABit.Web.Controllers;
using ReadABit.Web.Test.Helpers;
using Shouldly;
using Xunit;

namespace ReadABit.Web.Test.Controllers
{
    public class UserPreferencesControllerTest : TestBase
    {
        public UserPreferencesControllerTest(IServiceProvider serviceProvider, IRequestContext requestContext) : base(serviceProvider, requestContext)
        {
        }

        [Fact]
        public async Task Upsert_Succeeds()
        {
            await UserPreferencesController.Upsert(new UserPreferenceUpsert
            {
                Data = new()
                {
                    DailyGoalNewlyCreatedWordFamiliarityCount = 10,
                }
            });
            (await Get()).DailyGoalNewlyCreatedWordFamiliarityCount.ShouldBe(10);

            using (User(2))
            {
                await UserPreferencesController.Upsert(new UserPreferenceUpsert
                {
                    Data = new()
                    {
                        DailyGoalNewlyCreatedWordFamiliarityCount = 30,
                    }
                });
            }
            (await Get()).DailyGoalNewlyCreatedWordFamiliarityCount.ShouldBe(10);
        }

        [Fact]
        public async Task Get_ShouldReturnDefaultValueWhenUserDoesNotOwnAnUserPreference()
        {
            using (User(3))
            {
                (await Get()).DailyGoalNewlyCreatedWordFamiliarityCount.ShouldNotBe(default);
            }
        }

        private async Task<UserPreferenceData> Get()
        {
            return (await UserPreferencesController.Get(new UserPreferenceGet { }))
                .ShouldBeOfType<OkObjectResult>()
                .Value.ShouldBeOfType<UserPreferenceData>();
        }
    }
}

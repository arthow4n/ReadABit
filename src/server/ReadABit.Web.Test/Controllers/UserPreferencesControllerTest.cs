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
        public async Task CRUD_Succeeds()
        {
            await UserPreferencesController.Upsert(new UserPreferenceUpsert
            {
                Type = UserPreferenceType.LanguageCode,
                Value = "en",
            });
            (await List()).ShouldHaveSingleItem().ShouldSatisfyAllConditions(
                x => x.Type.ShouldBe(UserPreferenceType.LanguageCode),
                x => x.Value.ShouldBe("en")
            );

            using (User(2))
            {
                (await List()).ShouldBeEmpty();
            }

            await UserPreferencesController.Upsert(new UserPreferenceUpsert
            {
                Type = UserPreferenceType.LanguageCode,
                Value = "sv",
            });
            var preference = (await List()).ShouldHaveSingleItem();
            preference.ShouldSatisfyAllConditions(
                x => x.Type.ShouldBe(UserPreferenceType.LanguageCode),
                x => x.Value.ShouldBe("sv")
            );

            using (User(2))
            {
                await UserPreferencesController.Delete(preference.Id, new UserPreferenceDelete { });
            }
            (await List()).ShouldNotBeEmpty();

            await UserPreferencesController.Delete(preference.Id, new UserPreferenceDelete { });
            (await List()).ShouldBeEmpty();
        }

        private async Task<List<UserPreference>> List()
        {
            return (await UserPreferencesController.List(new UserPreferenceList { }))
                .ShouldBeOfType<OkObjectResult>()
                .Value.ShouldBeOfType<List<UserPreference>>();
        }
    }
}

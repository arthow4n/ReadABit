using ReadABit.Web.Controllers;
using Xunit;
using ReadABit.Web.Test.Helpers;
using System.Threading.Tasks;
using System;
using ReadABit.Core.Utils;
using System.Collections.Generic;
using ReadABit.Infrastructure.Models;
using ReadABit.Core.Commands;
using Shouldly;
using Microsoft.AspNetCore.Mvc;

namespace ReadABit.Web.Test.Controllers
{
    public class UserPreferencesControllerTest : TestBase<UserPreferencesController>
    {
        public UserPreferencesControllerTest(IServiceProvider serviceProvider, IRequestContext requestContext) : base(serviceProvider, requestContext)
        {
        }

        [Fact]
        public async Task CRUD_Succeeds()
        {
            await T1.UpsertUserPreference(new UserPreferenceUpsert
            {
                Type = UserPreferenceType.LanguageCode,
                Value = "en",
            });
            (await List()).ShouldHaveSingleItem().ShouldSatisfyAllConditions(
                x => x.Type.ShouldBe(UserPreferenceType.LanguageCode),
                x => x.Value.ShouldBe("en")
            );

            await T1.UpsertUserPreference(new UserPreferenceUpsert
            {
                Type = UserPreferenceType.LanguageCode,
                Value = "sv",
            });
            var preference = (await List()).ShouldHaveSingleItem();
            preference.ShouldSatisfyAllConditions(
                x => x.Type.ShouldBe(UserPreferenceType.LanguageCode),
                x => x.Value.ShouldBe("sv")
            );

            await T1.DeleteUserPreference(preference.Id, new UserPreferenceDelete { });
            (await List()).ShouldBeEmpty();
        }

        private async Task<List<UserPreference>> List()
        {
            return (await T1.ListUserPreferences(new UserPreferenceList { }))
                .ShouldBeOfType<OkObjectResult>()
                .Value.ShouldBeOfType<List<UserPreference>>();
        }
    }
}

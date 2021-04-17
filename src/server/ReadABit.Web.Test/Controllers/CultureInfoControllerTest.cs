using System;
using System.Collections.Generic;
using System.Linq;
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
    public class CultureInfoControllerTest : TestBase
    {
        public CultureInfoControllerTest(IServiceProvider serviceProvider, IRequestContext requestContext) : base(serviceProvider, requestContext)
        {
        }


        // TODO: Support BCP 47
        // [InlineData("zh-TW", "Europe/Stockholm", "斯德哥爾摩")]
        [InlineData("ja", "Europe/Stockholm", "ストックホルム")]
        [InlineData("sv", "Europe/Copenhagen", "Köpenhamn")]
        [Theory]
        public async Task ListAllSupportedTimeZones_ShouldReturnLocalisedDisplayNames(string userInterfaceLanguageCode, string ianaTimeZoneId, string expectDisplayName)
        {
            await UserPreferencesController.Upsert(new()
            {
                Data = new()
                {
                    UserInterfaceLanguageCode = userInterfaceLanguageCode,
                    WordDefinitionLanguageCode = "sv",
                },
            });

            (await ListAllSupportedTimeZones())
                .Where(vm => vm.Id == ianaTimeZoneId)
                .Single()
                .DisplayName
                .ShouldContain(expectDisplayName);
        }

        private async Task<List<TimeZoneInfoViewModel>> ListAllSupportedTimeZones()
        {
            return (await CultureInfoController.ListAllSupportedTimeZones())
                .ShouldBeOfType<OkObjectResult>()
                .Value.ShouldBeOfType<List<TimeZoneInfoViewModel>>();
        }
    }
}

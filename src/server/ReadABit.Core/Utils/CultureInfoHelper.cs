using System.Collections.Generic;
using System.Linq;
using NodaTime.Extensions;
using ReadABit.Core.Contracts;
using TimeZoneConverter;
using TimeZoneNames;

namespace ReadABit.Core.Utils
{
    public static class CultureInfoHelper
    {
        /// <param name="languageCode">BCP 47 language code for render the display name of time zones in.</param>
        public static List<TimeZoneInfoViewModel> ListAllSupportedTimeZones(string languageCode)
        {
            return NodaTime.DateTimeZoneProviders.Tzdb
                .GetAllZones()
                .Select(tz =>
                {
                    TZConvert.TryGetTimeZoneInfo(tz.Id, out var tzi);
                    return new
                    {
                        tz.Id,
                        tzi,
                    };
                })
                .Where(x => x.tzi != null)
                .Select(x => new
                {
                    x.Id,
                    x.tzi.BaseUtcOffset.TotalSeconds,
                })
                .OrderBy(x => x.TotalSeconds)
                .Select(x => new TimeZoneInfoViewModel
                {
                    Id = x.Id,
                    DisplayName = TZNames.GetDisplayNameForTimeZone(x.Id, languageCode),
                })
                .ToList();
        }
    }
}

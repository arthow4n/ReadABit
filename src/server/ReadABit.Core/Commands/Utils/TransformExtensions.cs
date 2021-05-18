using NodaTime;
using NodaTime.Text;

namespace ReadABit.Core.Commands.Utils
{
    public static class TransformExtensions
    {
        public static ParseResult<LocalTime> TryParseIsoHhmmssToLocalTime(this string x)
        {
            return LocalTimePattern
                .CreateWithInvariantCulture("HH':'mm':'ss")
                .Parse(x);
        }

        public static LocalTime ParseIsoHhmmssToLocalTimeOrThrow(this string x)
        {
            return LocalTimePattern
                .CreateWithInvariantCulture("HH':'mm':'ss")
                .Parse(x)
                .GetValueOrThrow();
        }

        public static DateTimeZone? TryParseToDateTimeZone(this string x)
        {
            return DateTimeZoneProviders.Tzdb.GetZoneOrNull(x);
        }

        public static DateTimeZone ParseToDateTimeZoneOrThrow(this string x)
        {
            return DateTimeZoneProviders.Tzdb[x];
        }

        public static ZonedDateTime WithLocalTime(this ZonedDateTime source, LocalTime newLocalTime)
        {
            source.Deconstruct(out var localDateTime, out var zone, out var offset);
            localDateTime.Deconstruct(out var date, out var _);

            return new ZonedDateTime(
                newLocalTime.On(date),
                zone,
                offset
            );
        }
    }
}

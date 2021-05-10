using NodaTime;
using NodaTime.Testing;
using NodaTime.Text;

namespace ReadABit.Web.Test.Helpers
{
    public static class FakeClockExtensions
    {
        public static FakeClock SetToIso(this FakeClock clock, string offsetDateTimePatternIso)
        {
            clock.Reset(
                OffsetDateTimePattern
                    .GeneralIso
                    .Parse(offsetDateTimePatternIso)
                    .Value
                    .ToInstant()
            );

            return clock;
        }

        public static FakeClock Run(this FakeClock clock)
        {
            clock.AutoAdvance = Duration.FromSeconds(1);

            return clock;
        }
    }
}

using Newtonsoft.Json;
using NodaTime;
using NSwag.Annotations;

namespace ReadABit.Core.Contracts
{
    public record WordFamiliarityDailyGoalCheckViewModel
    {
        public int NewlyCreated { get; init; }
        public int NewlyCreatedGoal { get; init; }
        public bool NewlyCreatedReached { get; init; }
        [OpenApiIgnore, JsonIgnore]
        public WordFamiliarityDailyGoalCheckViewModelMetaData Metadata { get; init; } = new();
    }

    public record WordFamiliarityDailyGoalCheckViewModelMetaData
    {
        public bool IsNowEarlierThanTodaysReset { get; init; }
        public ZonedDateTime CurrentDailyGoalPeriodStart { get; internal set; }
    }
}

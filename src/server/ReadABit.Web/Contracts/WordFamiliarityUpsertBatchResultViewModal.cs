using ReadABit.Core.Contracts;

namespace ReadABit.Web.Contracts
{
    public record WordFamiliarityUpsertBatchResultViewModal
    {
        public WordFamiliarityDailyGoalCheckViewModel DailyGoalStatus { get; set; } = new();
    }
}

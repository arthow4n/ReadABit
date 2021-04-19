namespace ReadABit.Core.Contracts
{
    public record WordFamiliarityDailyGoalCheckViewModel
    {
        public int NewlyCreated { get; init; }
        public int NewlyCreatedGoal { get; init; }
        public bool NewlyCreatedReached { get; init; }
    }
}

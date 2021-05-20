namespace ReadABit.Core.Contracts
{
    public record UserAchievementsDailyGoalStreakStateViewModel
    {
        public int CurrentStreakDays { get; init; }
        public WordFamiliarityDailyGoalCheckViewModel DailyGoalCheckResult { get; init; } = new();
    }
}

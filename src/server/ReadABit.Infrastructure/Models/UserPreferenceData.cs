namespace ReadABit.Infrastructure.Models
{
    /// <summary>
    /// JSON of user preference.
    /// 
    /// Don't forget to update <see cref="UserPreferenceUpdateValidator" /> if you added a prop here.
    /// </summary>
    public record UserPreferenceData
    {
        // TODO: Use thread culture instead
        public string UserInterfaceLanguageCode { get; init; } = "en";
        public string DailyGoalResetTimeTimeZone { get; init; } = "Europe/London";
        public string DailyGoalResetTimePartial { get; init; } = "00:00:00";
        public int DailyGoalNewlyCreatedWordFamiliarityCount { get; init; } = 25;
    }
}

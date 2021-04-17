namespace ReadABit.Infrastructure.Models
{
    /// <summary>
    /// JSON of user preference.
    /// 
    /// Don't forget to update <see cref="UserPreferenceUpdateValidator" /> if you added a prop here.
    /// </summary>
    public record UserPreferenceData
    {
        public string WordDefinitionLanguageCode { get; init; } = "en";
        public string UserInterfaceLanguageCode { get; init; } = "en";
    }
}

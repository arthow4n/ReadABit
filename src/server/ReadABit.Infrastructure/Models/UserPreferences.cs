using System;

namespace ReadABit.Infrastructure.Models
{
    public class UserPreference
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public UserPreferenceType Type { get; init; }
        /// <summary>
        /// Arbitrary string value that should be intepreted based on <see cref="Type" />.
        /// </summary>
        public string Value { get; set; }
    }

    public enum UserPreferenceType
    {
        Invalid = 0,
        /// <summary>
        /// User's preferred language code for e.g. word definition.
        /// 
        /// Value format: See <see cref="WordDefinition.LanguageCode" />.
        /// </summary>
        LanguageCode = 1,
    }
}

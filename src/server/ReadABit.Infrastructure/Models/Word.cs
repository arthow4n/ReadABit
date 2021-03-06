using System;
using System.ComponentModel.DataAnnotations;

namespace ReadABit.Infrastructure.Models
{
    /// <summary>
    /// This entity is mainly aggregating search results of <see cref="WordDefinition" />,
    /// therefore this should never be owned by a specific user.
    /// </summary>
    public class Word
    {
        public Guid Id { get; set; }
        /// <summary>
        /// <see cref="CultureInfo.TwoLetterISOLanguageName" />
        /// </summary>
        [Required]
        public string LanguageCode { get; set; }
        /// <summary>
        /// The expression for matching the word.
        /// The format of expression haven't been decided so far.
        /// Put the whole word into this field for now.
        /// </summary>
        [Required]
        public string Expression { get; set; }
    }
}

using System;

namespace ReadABit.Infrastructure.Models
{
    public class WordDefinition
    {
        public Guid WordId { get; set; }
        public Word Word { get; set; }
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
        public bool Public { get; set; }
        /// <summary>
        /// This can be different from the connected <see cref="Word.LanguageCode" />.
        /// For example, a native English speaker who is studying Swedish might
        /// create an English <see cref="WordDefinition" /> for a <see cref="Word" /> that's in Swedish.
        /// Format is <see cref="CultureInfo.TwoLetterISOLanguageName" />.
        /// </summary>
        public string LanguageCode { get; set; }
        /// <summary>
        /// The meaning of the connected word.
        /// </summary>
        public string Meaning { get; set; }
    }
}

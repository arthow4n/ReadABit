using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NodaTime;
using ReadABit.Infrastructure.Interfaces;

namespace ReadABit.Infrastructure.Models
{
    public class WordDefinition : IEntityWithCreateUpdateTimestamps, IOfWord
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        public Guid WordId { get; set; }
        public Word Word { get; set; }
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
        [Required]
        public bool Public { get; set; }
        /// <summary>
        /// This can be different from the connected <see cref="Word.LanguageCode" />.
        /// For example, a native English speaker who is studying Swedish might
        /// create an English <see cref="WordDefinition" /> for a <see cref="Word" /> that's in Swedish.
        /// Format is <see cref="CultureInfo.TwoLetterISOLanguageName" />.
        /// </summary>
        [Required]
        public string LanguageCode { get; set; }
        /// <summary>
        /// The meaning of the connected word.
        /// </summary>
        [Required]
        public string Meaning { get; set; }
        /// <summary>
        /// When the definition is first created.
        /// Can be used for tracking user goals.
        /// </summary>
        [Required]
        public Instant CreatedAt { get; set; }
        [Required]
        public Instant UpdatedAt { get; set; }
    }
}

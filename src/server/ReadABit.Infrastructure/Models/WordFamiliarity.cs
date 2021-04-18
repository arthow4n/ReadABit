using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NodaTime;
using ReadABit.Infrastructure.Interfaces;

namespace ReadABit.Infrastructure.Models
{
    public class WordFamiliarity : IOfWord, ITimestampedEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        public Guid WordId { get; set; }
        public Word Word { get; set; }
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
        [Required]
        public int Level { get; set; }
        public static readonly int LevelMax = 3;
        public static readonly int LevelIgnored = -1;
        [Required]
        public Instant CreatedAt { get; set; }
        [Required]
        public Instant UpdatedAt { get; set; }
    }
}

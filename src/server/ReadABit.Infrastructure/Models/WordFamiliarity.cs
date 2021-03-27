using System;

namespace ReadABit.Infrastructure.Models
{
    public class WordFamiliarity
    {
        public Guid Id { get; set; }
        public Guid WordId { get; set; }
        public Word Word { get; set; }
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int Level { get; set; }
        public static readonly int LevelMax = 5;
        public static readonly int LevelIgnored = -1;
    }
}

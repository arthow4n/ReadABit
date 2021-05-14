using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NodaTime;
using ReadABit.Core.Integrations.Contracts.Conllu;
using ReadABit.Infrastructure.Interfaces;

namespace ReadABit.Infrastructure.Models
{
    public class ArticleReadingProgress : IEntityWithCreateUpdateTimestamps
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public ApplicationUser User { get; init; }
        public Guid ArticleId { get; init; }
        public Article Article { get; init; }
        /// <summary>
        /// A pointer pointing to the last read token in the <see cref="Article" />.
        /// Be ware that this could get out of sync and point to a non-existing token if the article is updated.
        /// </summary>
        [Required]
        [Column(TypeName = "jsonb")]
        public Conllu.TokenPointer ConlluTokenPointer { get; init; }
        /// <summary>
        /// Percentage value of reading progress where 1 means 100%.
        /// This can be arbitary set by the client.
        /// </summary>
        [Required]
        public decimal ReadRatio { get; init; }
        [Required]
        public Instant CreatedAt { get; set; }
        [Required]
        public Instant UpdatedAt { get; set; }

        // TODO: Consider saving word familiarity percentage progress here as well.
    }
}

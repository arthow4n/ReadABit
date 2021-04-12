using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NodaTime;
using ReadABit.Core.Integrations.Contracts.Conllu;
using ReadABit.Infrastructure.Interfaces;

namespace ReadABit.Infrastructure.Models
{
    public class Article : ITimestampedEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        public Guid ArticleCollectionId { get; set; }
        public ArticleCollection ArticleCollection { get; set; }

        /// <summary>
        /// Article title
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Plain text of the article content
        /// </summary>
        [Required]
        public string Text { get; set; }
        [Required]
        [Column(TypeName = "jsonb")]
        public Conllu.Document ConlluDocument { get; set; }
        [Required]
        public Instant CreatedAt { get; set; }
        [Required]
        public Instant UpdatedAt { get; set; }

        public List<ArticleReadingProgress> ArticleReadingProgress { get; set; }
    }
}

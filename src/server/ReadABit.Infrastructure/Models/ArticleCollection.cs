using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NodaTime;
using ReadABit.Infrastructure.Interfaces;

namespace ReadABit.Infrastructure.Models
{
    public class ArticleCollection : IEntityWithCreateUpdateTimestamps
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// <see cref="CultureInfo.TwoLetterISOLanguageName" />
        /// </summary>
        [Required]
        public string LanguageCode { get; set; }
        public List<Article> Articles { get; set; }
        [Required]
        public bool Public { get; set; }
        [Required]
        public Instant CreatedAt { get; set; }
        [Required]
        public Instant UpdatedAt { get; set; }
    }
}

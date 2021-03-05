using System;
using System.ComponentModel.DataAnnotations;

namespace ReadABit.Infrastructure.Models
{
    public class Article
    {
        public Guid Id { get; set; }

        [Required]
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

        /// <summary>
        /// Conllu annotation of <see cref="Text" />
        /// </summary>
        [Required]
        public string Conllu { get; set; }
    }
}

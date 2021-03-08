using System;
using System.ComponentModel.DataAnnotations;

namespace ReadABit.Infrastructure.Models
{
    public class Article
    {
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
        /// <summary>
        /// XML annotation of <see cref="Article.Text" /> generated with sparv-pipeline and serial.
        /// https://spraakbanken.gu.se/verktyg/sparv
        /// </summary>
        [Required]
        public string SparvXmlJson { get; set; }
        /// <summary>
        /// Version of sparv-pipeline usered to generate <see cref="SparvXmlJson" />.
        /// This is saved just in case there will be unknown changes.
        /// </summary>
        [Required]
        public string SparvXmlVersion { get; set; }
    }
}

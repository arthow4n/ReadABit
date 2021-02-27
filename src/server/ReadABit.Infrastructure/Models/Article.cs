using System;
using System.ComponentModel.DataAnnotations;

namespace ReadABit.Infrastructure.Models
{
    public class Article
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }

        public Guid ArticleCollectionId { get; set; }
        public ArticleCollection ArticleCollection { get; set; }
    }
}

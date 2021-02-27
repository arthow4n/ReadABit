using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReadABit.Infrastructure.Models
{
    public class ArticleCollection
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public List<Article> Articles { get; set; }

        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}

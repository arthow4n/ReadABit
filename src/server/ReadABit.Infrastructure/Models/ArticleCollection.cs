using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReadABit.Infrastructure.Models
{
    public class ArticleCollection
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required]
        public string Name { get; set; }

        /// <summary>
        /// <see cref="CultureInfo.TwoLetterISOLanguageName" />
        /// </summary>
        /// <value></value>
        [Required]
        public string LanguageCode { get; set; }

        public List<Article> Articles { get; set; }
    }
}

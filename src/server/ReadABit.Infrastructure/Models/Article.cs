﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ReadABit.Core.Integrations.Contracts.Conllu;

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

        [Required]
        [Column(TypeName = "jsonb")]
        public Conllu.Document ConlluDocument { get; set; }
    }
}

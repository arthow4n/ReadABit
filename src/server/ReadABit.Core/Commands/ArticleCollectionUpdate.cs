using System.ComponentModel.DataAnnotations;
using System;
using MediatR;

namespace ReadABit.Core.Commands
{
    public class ArticleCollectionUpdate : IRequest<bool>
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        [Required]
        public string Name { get; set; } = "";
        [Required]
        public string LanguageCode { get; set; } = "";
    }
}

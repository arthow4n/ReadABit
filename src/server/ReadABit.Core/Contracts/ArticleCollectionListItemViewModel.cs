using System;
using NodaTime;

namespace ReadABit.Core.Contracts
{
    public record ArticleCollectionListItemViewModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = "";
        public string LanguageCode { get; set; } = "";
        public bool Public { get; set; }
        public Instant CreatedAt { get; set; }
        public Instant UpdatedAt { get; set; }
    }
}

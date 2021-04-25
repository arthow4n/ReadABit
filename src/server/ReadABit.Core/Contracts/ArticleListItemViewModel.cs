using System;

namespace ReadABit.Core.Contracts
{
    public record ArticleListItemViewModel
    {
        public Guid Id { get; init; }
        public Guid ArticleCollectionId { get; init; }
        public string Name { get; init; } = "";
        public decimal ReadRadio { get; init; }
    }
}

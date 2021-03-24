using System;
using NodaTime;
using ReadABit.Core.Integrations.Contracts.Conllu;

namespace ReadABit.Core.Contracts
{
    public record ArticleViewModel
    {
        public Guid Id { get; init; }
        public Guid ArticleCollectionId { get; init; }
        public string Name { get; init; } = "";
        public string LanguageCode { get; init; } = "";
        public Conllu.Document ConlluDocument { get; init; } = new();
        public Instant CreatedAt { get; init; }
        public Instant UpdatedAt { get; init; }
    }
}

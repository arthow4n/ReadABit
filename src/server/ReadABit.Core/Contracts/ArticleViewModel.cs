using System;
using Newtonsoft.Json;
using NodaTime;
using NSwag.Annotations;
using ReadABit.Core.Integrations.Contracts.Conllu;

namespace ReadABit.Core.Contracts
{
    public record ArticleViewModel
    {
        public Guid Id { get; init; }
        public Guid ArticleCollectionId { get; init; }
        public string Name { get; init; } = "";
        public string LanguageCode { get; init; } = "";
        /// <summary>
        /// This is only for easier fetching data from DB.
        /// </summary>
        [OpenApiIgnore, JsonIgnore]
        public Conllu.Document ColluDocumentInternal { get; init; } = new();
        public ConlluDocumentViewModel ConlluDocument { get; set; } = new();
        public Instant CreatedAt { get; init; }
        public Instant UpdatedAt { get; init; }
        public ArticleReadingProgressViewModel ReadingProgress { get; set; } = new();

        public record ArticleReadingProgressViewModel
        {
            public Conllu.TokenPointer ConlluTokenPointer { get; init; } = new();
            public decimal ReadRatio { get; init; } = 0;
            public Instant CreatedAt { get; init; }
            public Instant UpdatedAt { get; init; }
        }
    }
}

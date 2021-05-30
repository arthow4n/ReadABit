using System.Collections.Generic;
namespace ReadABit.Core.Contracts
{
    public record FolketsLexikonLookUpViewModelWordEntry
    {
        public string WordExpression { get; init; } = "";
        public List<FolketsLexikonLookUpViewModelTranslationEntry> Translations { get; init; } = new();
    }
    public record FolketsLexikonLookUpViewModelTranslationEntry
    {
        public string Text { get; init; } = "";
        public string? Comment { get; init; }
    }
}

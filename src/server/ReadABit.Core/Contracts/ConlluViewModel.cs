using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ReadABit.Core.Contracts
{
    public record ConlluDocumentViewModel
    {
        public string Id { get; init; } = "";
        public List<ConlluParagraphViewModel> Paragraphs { get; init; } = new();
    }

    public record ConlluParagraphViewModel
    {
        public string Id { get; init; } = "";
        public List<ConlluSentenceViewModel> Sentences { get; init; } = new();
    }

    public record ConlluSentenceViewModel
    {
        public string Id { get; init; } = "";
        public string Text { get; set; } = "";
        public List<ConlluTokenViewModel> Tokens { get; init; } = new();
    }

    public record ConlluTokenViewModel
    {
        public string Id { get; init; } = "_";
        public string Form { get; init; } = "_";
        public string Lemma { get; init; } = "_";
        public string Upos { get; init; } = "_";
        public string Xpos { get; init; } = "_";
        public string Feats { get; init; } = "_";
        public string Head { get; init; } = "0";
        public string Deprel { get; init; } = "_";
        public string Deps { get; init; } = "_";
        public string Misc { get; init; } = "_";
        public string LanguageCode { get; init; } = "en";
        /// <summary>
        /// Normalised form of the token mainly for matching word definition and familiarity.
        /// </summary>
        public ConlluNormalisedTokenViewModel NormalisedToken { get; init; } = new();
    }

    public record ConlluNormalisedTokenViewModel
    {
        public string Form { get; init; } = "_";
        public string Lemma { get; init; } = "_";
        public string LanguageCode { get; init; } = "en";

        public static string NormaliseString(string input, CultureInfo culture)
        {
            var result = input.ToLower(culture);

            return Regex.Replace(result, @"(^[\W]+|[\W]+$)", string.Empty);
        }
    }
}

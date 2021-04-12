// Reference: https://universaldependencies.org/format.html
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

#nullable enable

namespace ReadABit.Core.Integrations.Contracts.Conllu
{
    public static class Conllu
    {
        /// <summary>
        /// Parse input CoNLL-U into structured object.
        /// </summary>
        public static Document ToConlluDocument(this string conllu)
        {
            var doc = new Document { };
            var documentCounter = 0;
            var currentParagraph = new Paragraph { };
            var paragraphCounter = 0;
            var currentSentence = new Sentence { };
            var sentenceCounter = 0;

            using var reader = new StringReader(conllu);

            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                switch (line)
                {
                    case var s when s.StartsWith("# newdoc"):
                        var documentId = new Regex(@"\A# newdoc = (?<id>.+)\z").Match(s).Groups["id"].Value;
                        documentCounter += 1;
                        doc = new()
                        {
                            Id = string.IsNullOrWhiteSpace(documentId) ?
                                "1" :
                                documentId,
                        };
                        continue;
                    case var s when s.StartsWith("# newpar"):
                        var paragraphId = new Regex(@"\A# newpar = (?<id>.+)\z").Match(s).Groups["id"].Value;
                        paragraphCounter += 1;
                        currentParagraph = new()
                        {
                            Id = string.IsNullOrWhiteSpace(paragraphId) ?
                                $"{paragraphCounter}" :
                                paragraphId,
                        };
                        doc.Paragraphs.Add(currentParagraph);
                        continue;
                    case var s when s.StartsWith("# sent_id = "):
                        var sentenceId = new Regex(@"\A# sent_id = (?<id>.+)\z").Match(s).Groups["id"].Value;
                        sentenceCounter += 1;
                        currentSentence = new()
                        {
                            Id = string.IsNullOrWhiteSpace(sentenceId) ?
                                $"{sentenceCounter}" :
                                sentenceId,
                        };
                        currentParagraph.Sentences.Add(currentSentence);
                        continue;
                    case var s when s.StartsWith("# text = "):
                        currentSentence.Text = new Regex(@"\A# text = (?<text>.+)\z").Match(s).Groups["text"].Value;
                        continue;
                    case var s when Regex.IsMatch(s, @"\A[0-9]"):
                        var tokenMatchGroups = new Regex(@"\A(?<Id>.+?)\t(?<Form>.+?)\t(?<Lemma>.+?)\t(?<Upos>.+?)\t(?<Xpos>.+?)\t(?<Feats>.+?)\t(?<Head>.+?)\t(?<Deprel>.+?)\t(?<Deps>.+?)\t(?<Misc>.+?)\z").Match(s).Groups;
                        currentSentence.Tokens.Add(new()
                        {
                            Id = tokenMatchGroups["Id"].Value,
                            Form = tokenMatchGroups["Form"].Value,
                            Lemma = tokenMatchGroups["Lemma"].Value,
                            Upos = tokenMatchGroups["Upos"].Value,
                            Xpos = tokenMatchGroups["Xpos"].Value,
                            Feats = tokenMatchGroups["Feats"].Value,
                            Head = tokenMatchGroups["Head"].Value,
                            Deprel = tokenMatchGroups["Deprel"].Value,
                            Deps = tokenMatchGroups["Id"].Value,
                            Misc = tokenMatchGroups["Misc"].Value,
                        });
                        continue;
                    case var s when string.IsNullOrWhiteSpace(s):
                    default:
                        continue;
                }
            }

            return doc;
        }

        /// <summary>
        /// CoNLL-U document formatted in plain object.
        /// </summary>
        public record Document
        {
            public string Id { get; init; } = "";
            public List<Paragraph> Paragraphs { get; init; } = new();
        }

        public record Paragraph
        {
            public string Id { get; init; } = "";
            public List<Sentence> Sentences { get; init; } = new();
        }

        public record Sentence
        {
            /// <summary>
            /// Content of `# sent_id = ` comment.
            /// This is often integer from <see cref="Services.UDPipeV1Service" />'s output, but it can be something else.
            /// </summary>
            public string Id { get; init; } = "";
            /// <summary>
            /// Content of `# text = ` comment.
            /// </summary>
            public string Text { get; set; } = "";
            public List<Token> Tokens { get; init; } = new();
        }

        public record Token
        {
            /// <summary>
            /// 1st column in a token. Could be either integer or range ID (for example, "1-2").
            /// </summary>
            public string Id { get; init; } = "_";
            /// <summary>
            /// 2nd column in a token. The actually used word.
            /// </summary>
            public string Form { get; init; } = "_";
            /// <summary>
            /// 3rd column in a token. Lemma or stem of word form.
            /// </summary>
            public string Lemma { get; init; } = "_";
            /// <summary>
            /// 4th column in a token. Universal part-of-speech tag. https://universaldependencies.org/u/pos/index.html
            /// </summary>
            public string Upos { get; init; } = "_";
            /// <summary>
            /// 5th column in a token. Language-specific part-of-speech tag; underscore if not available.
            /// </summary>
            /// <value></value>
            public string Xpos { get; init; } = "_";
            /// <summary>
            /// 6th column in a token. List of morphological features from either of the below; underscore if not available.
            /// - https://universaldependencies.org/u/feat/index.html
            /// - https://universaldependencies.org/ext-feat-index.html
            /// </summary>
            public string Feats { get; init; } = "_";
            /// <summary>
            /// 7th column in a token. Head of the current word, which is either a value of ID or zero (0).
            /// </summary>
            public string Head { get; init; } = "0";
            /// <summary>
            /// 8th column in a token. Could be either of the below:
            /// - https://universaldependencies.org/u/dep/index.html
            /// - A defined language-specific subtype of one
            /// </summary>
            public string Deprel { get; init; } = "_";
            /// <summary>
            /// 9th column in a token. Enhanced dependency graph in the form of a list of head-deprel pairs.
            /// </summary>
            public string Deps { get; init; } = "_";
            /// <summary>
            /// 10th column in a token. Could have supportive info like `SpaceAfter=No`, `SpaceAfter=\n` for transforming the tokens back into original text.
            /// </summary>
            /// <value></value>
            public string Misc { get; init; } = "_";
        }

        /// <summary>
        /// A general pointer for pointing to a specific place in the <see cref="Document" />.
        /// </summary>
        public record TokenPointer
        {
            public string DocumentId { get; init; } = "";
            public string ParagraphId { get; init; } = "";
            public string SentenceId { get; init; } = "";
            public string TokenId { get; init; } = "";
        }
    }
}

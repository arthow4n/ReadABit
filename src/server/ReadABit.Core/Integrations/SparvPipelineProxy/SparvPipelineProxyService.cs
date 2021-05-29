using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using ReadABit.Core.Integrations.Contracts.Conllu;
using ReadABit.Core.Integrations.Services;

namespace ReadABit.Core.Integrations.SparvPipelineProxy
{
    public class SparvPipelineProxyService : ConlluServiceBase
    {
        public SparvPipelineProxyService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override async Task<Conllu.Document> ToConlluDocument(string languageCode, string input)
        {
            var sparvResult = await SparvPipelineProxyClient.SparvAsync(new()
            {
                LanguageCode =
                    languageCode.StartsWith("sv") ?
                        "swe" :
                        throw new NotImplementedException("SparvPipelineProxy integration doesn't support languages other than Swedish for now."),
                Input = input,
            });

            return TransformSparvXmlToConlluDocument(sparvResult.Xml);
        }

        private static Conllu.Document TransformSparvXmlToConlluDocument(string xmlInput)
        {
            var xml = XDocument.Parse(xmlInput);

            return new()
            {
                Id = "1",
                Paragraphs =
                    xml.Elements("paragraph").Any() ?
                        xml.Elements("paragraph")
                            .Select((xp, xpi) => new Conllu.Paragraph
                            {
                                Id = xpi.ToString(),
                                Sentences =
                                    ToSentences(xp.Descendants("sentence")),
                            })
                            .ToList() :
                        // Create a dummy paragraph for type 2 input.
                        new()
                        {
                            new()
                            {
                                Id = "1",
                                Sentences = ToSentences(xml.Descendants("sentence"))
                            }
                        },
            };
        }

        private static List<Conllu.Sentence> ToSentences(IEnumerable<XElement> sentenceElements)
        {
            return sentenceElements
                .Select((xs, xsi) => new Conllu.Sentence
                {
                    Id = xsi.ToString(),
                    Tokens =
                        // Look into descendants instead of direct child to support type 2 input.
                        xs.Descendants("token")
                            .Select((xt, xti) => new Conllu.Token
                            {
                                Id = xti.ToString(),
                                Form = xt.Value,
                                Lemma = StripAndFallbackToUnderscore(xt.Attribute("baseform")?.Value),
                                Upos = StripAndFallbackToUnderscore(xt.Attribute("upos")?.Value),
                                Xpos = StripAndFallbackToUnderscore(xt.Attribute("msd")?.Value.Replace(".", "|")),
                                Feats = StripAndFallbackToUnderscore(xt.Attribute("ufeats")?.Value),
                                // TODO: Support token dependency
                                Head = "_",
                                Deprel = "_",
                                Deps = "_",
                                Misc = "_",
                                SparvPipelineMisc = new()
                                {
                                    Tail = xt.Attribute("tail")?.Value ?? "",
                                    Compwf =
                                        StripRedundantPipes(xt.Attribute("compwf")?.Value)
                                            .Split("|")
                                            .Where(x => !string.IsNullOrWhiteSpace(x))
                                            .Select(x => x.Split("+").ToList())
                                            .ToList(),
                                },
                            })
                            .ToList(),
                })
                .ToList();
        }

        private static string StripAndFallbackToUnderscore(string? input)
        {
            var stripped = StripRedundantPipes(input ?? "_");
            return string.IsNullOrEmpty(stripped) ? "_" : stripped;
        }

        private static string StripRedundantPipes(string? input)
        {
            if (input is null)
            {
                return string.Empty;
            }

            return Regex.Replace(
                input,
                @"(^\||\|$)",
                string.Empty
            );
        }
    }
}

// Types of xml input tree structure:
// 1. text -> paragraph -> sentence -> token
// 2. text -> sentence -> paragraph -> token
// Some example of output format (ignore content please :P):

// Type 1:
//
// <?xml version='1.0' encoding='UTF-8'?>
// <text number_position="1">
//   <paragraph>
//     <sentence number_rel_text="1">
//       <token sentence="1" tail="\s" upos="VERB" baseform2="|" compwf="|hal+la|hall+la|" baseform="halla" msd="VB.IMP.AKT" ufeats="|Mood=Imp|VerbForm=Fin|Voice=Act|">Halla</token>
//       <token sentence="2" upos="NOUN" baseform2="|" compwf="|" baseform="varld" msd="NN.UTR.SIN.IND.NOM" ufeats="|Case=Nom|Definite=Ind|Gender=Com|Number=Sing|">varld</token>
//       <token sentence="3" tail="\s" upos="PUNCT" baseform2="|" compwf="|" baseform="!" msd="MAD" ufeats="|">!</token>
//     </sentence>
//     <sentence number_rel_text="2">
//       <token sentence="1" tail="\s" upos="VERB" baseform2="|" compwf="|hal+la|hall+la|" baseform="halla" msd="VB.IMP.AKT" ufeats="|Mood=Imp|VerbForm=Fin|Voice=Act|">Halla</token>
//       <token sentence="2" upos="NOUN" baseform2="|" compwf="|" baseform="allihopa" msd="NN.UTR.SIN.IND.NOM" ufeats="|Case=Nom|Definite=Ind|Gender=Com|Number=Sing|">allihopa</token>
//       <token sentence="3" tail="\n\n\n" upos="PUNCT" baseform2="|" compwf="|" baseform="." msd="MAD" ufeats="|">.</token>
//     </sentence>
//   </paragraph>
//   <paragraph>
//     <sentence number_rel_text="3">
//       <token sentence="1" tail="\s" upos="PRON" baseform2="|jag|" compwf="|" baseform="jag" msd="PN.UTR.SIN.DEF.SUB" ufeats="|Case=Nom|Definite=Def|Gender=Com|Number=Sing|">Jag</token>
//       <token sentence="2" tail="\s" upos="VERB" baseform2="|" compwf="|" baseform="ar" msd="VB.PRS.AKT" ufeats="|Mood=Ind|Tense=Pres|VerbForm=Fin|Voice=Act|">ar</token>
//       <token sentence="3" tail="\s" upos="DET" baseform2="|en|" compwf="|" baseform="en" msd="DT.UTR.SIN.IND" ufeats="|Definite=Ind|Gender=Com|Number=Sing|">en</token>
//       <token sentence="4" upos="NOUN" baseform2="|" compwf="|" baseform="larare" msd="NN.UTR.SIN.IND.NOM" ufeats="|Case=Nom|Definite=Ind|Gender=Com|Number=Sing|">larare</token>
//       <token sentence="5" upos="PUNCT" baseform2="|" compwf="|" baseform="." msd="MAD" ufeats="|">.</token>
//     </sentence>
//   </paragraph>
// </text>

// Type 2:
// 
// <?xml version='1.0' encoding='UTF-8'?>
// <text number_position="1">
//   <sentence number_rel_text="1">
//     <paragraph>
//       <token sentence="1" tail="\s" upos="VERB" baseform2="|" compwf="|hal+la|hall+la|" baseform="halla" msd="VB.IMP.AKT" ufeats="|Mood=Imp|VerbForm=Fin|Voice=Act|">Halla</token>
//       <token sentence="2" upos="NOUN" baseform2="|" compwf="|" baseform="varld" msd="NN.UTR.SIN.IND.NOM" ufeats="|Case=Nom|Definite=Ind|Gender=Com|Number=Sing|">varld</token>
//       <token sentence="3" tail="\n\n\n" upos="PUNCT" baseform2="|" compwf="|" baseform="!" msd="MAD" ufeats="|">!</token>
//     </paragraph>
//  </sentence>
//   <sentence number_rel_text="2">
//     <paragraph>
//       <token sentence="1" tail="\s" upos="PRON" baseform2="|jag|" compwf="|" baseform="jag" msd="PN.UTR.SIN.DEF.SUB" ufeats="|Case=Nom|Definite=Def|Gender=Com|Number=Sing|">Jag</token>
//       <token sentence="2" tail="\s" upos="VERB" baseform2="|" compwf="|" baseform="ar" msd="VB.PRS.AKT" ufeats="|Mood=Ind|Tense=Pres|VerbForm=Fin|Voice=Act|">ar</token>
//       <token sentence="3" tail="\s" upos="DET" baseform2="|en|" compwf="|" baseform="en" msd="DT.UTR.SIN.IND" ufeats="|Definite=Ind|Gender=Com|Number=Sing|">en</token>
//       <token sentence="4" upos="NOUN" baseform2="|" compwf="|" baseform="larare" msd="NN.UTR.SIN.IND.NOM" ufeats="|Case=Nom|Definite=Ind|Gender=Com|Number=Sing|">larare</token>
//       <token sentence="5" upos="PUNCT" baseform2="|" compwf="|" baseform="." msd="MAD" ufeats="|">.</token>
//     </paragraph>
//   </sentence>
// </text>

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using ReadABit.Core.Contracts;

namespace ReadABit.CliUtils.Commands
{
    // Folkets lexikon can by downloaded from:
    // http://folkets-lexikon.csc.kth.se/folkets/om.html
    // This command handler only handles their own XML format instead of XDXF because their own XML contains much more info.
    internal static class FolketsLexikonToJsonLookupCommandHandler
    {
        internal static async Task Handle(string inputXdxfPath, string outputJsonPath)
        {

            var xmlRaw = await File.ReadAllTextAsync(inputXdxfPath);
            var xml = XDocument.Parse(xmlRaw);

            var result = new Dictionary<string, List<FolketsLexikonLookUpViewModelWordEntry>> { };

            // Prefer using not null assertion instead of fallback value for easier debugging.

            var wordEntries = xml
                .Element("dictionary")!
                .Elements("word")
                .SelectMany(xw =>
                {
                    var translations = xw
                        .Elements("translation")
                        .Select(x => new FolketsLexikonLookUpViewModelTranslationEntry
                        {
                            Text = x.Attribute("value")!.Value,
                            Comment = x.Attribute("comment")?.Value,
                        })
                        .ToList();

                    var expressions = new List<string> {
                        xw.Attribute("value")!.Value
                    };

                    var xparadigm = xw.Element("paradigm");
                    if (xparadigm is not null)
                    {
                        expressions.AddRange(
                            xparadigm
                                .Elements("inflection")
                                .Select(x => x.Attribute("value")!.Value)
                        );
                    }

                    return expressions.ConvertAll(expression => new FolketsLexikonLookUpViewModelWordEntry
                    {
                        WordExpression = expression,
                        Translations = translations,
                    });
                });


            await File.WriteAllTextAsync(outputJsonPath, JsonConvert.SerializeObject(result));
        }
    }
}

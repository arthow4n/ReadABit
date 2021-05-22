using System;
using System.Threading.Tasks;
using System.Xml;
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
            return (await ToConllu(languageCode, input)).ToConlluDocument();
        }

        private async Task<string> ToConllu(string languageCode, string input)
        {
            var sparvResult = await SparvPipelineProxyClient.SparvAsync(new()
            {
                LanguageCode =
                    languageCode.StartsWith("sv") ?
                        "swe" :
                        throw new NotImplementedException("SparvPipelineProxy integration doesn't support languages other than Swedish for now."),
                Input = input,
            });

            return TransformSparvXmlToConllu(sparvResult.Xml);
        }

        private static string TransformSparvXmlToConllu(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            throw new NotImplementedException();
        }
    }
}

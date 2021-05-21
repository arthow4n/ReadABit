using MoreLinq;
using ReadABit.Core.Integrations.Contracts.Conllu;

namespace ReadABit.Core.Integrations.Services
{
    public static class ConlluService
    {
        /// <param name="languageCode">BCP 47 language code</param>
        /// <param name="input">Text content of the input.</param>
        public static Conllu.Document ToConlluDocument(string languageCode, string input)
        {
            return languageCode switch
            {
                var s when s.StartsWith("sv") =>
                    // TODO: Switch to SparvPipelineProxy
                    UDPipeV1Service.ToConlluDocument(languageCode.Substring(0, 2), input),
                _ => UDPipeV1Service.ToConlluDocument(languageCode.Substring(0, 2), input),
            };
        }
    }
}

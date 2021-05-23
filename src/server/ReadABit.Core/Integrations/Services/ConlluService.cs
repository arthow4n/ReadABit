using System;
using System.Threading.Tasks;
using ReadABit.Core.Integrations.Contracts.Conllu;
using ReadABit.Core.Integrations.Ufal;

namespace ReadABit.Core.Integrations.Services
{
    public class ConlluService : ConlluServiceBase
    {
        public ConlluService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <param name="languageCode">BCP 47 language code</param>
        /// <param name="input">Text content of the input.</param>
        public override async Task<Conllu.Document> ToConlluDocument(string languageCode, string input)
        {
            Conllu.Document? result = null;

            try
            {
                result = languageCode switch
                {
                    var s when s.StartsWith("sv") =>
                        // TODO: Switch to SparvPipelineProxy
                        await SparvPipelineProxyService.ToConlluDocument(languageCode.Substring(0, 2), input),
                    _ => null,
                };
            }
            catch (Exception ex)
            {
                // TODO: Log error in a better way e.g. Serilog
                Console.WriteLine($"ConlluService.ToConlluDocument({languageCode}, ...) soft error: {ex.Message}");
            }

            if (result is not null)
            {
                return result;
            }

            return await UDPipeV1Service.ToConlluDocument(languageCode.Substring(0, 2), input);
        }
    }
}

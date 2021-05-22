using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ReadABit.Core.Integrations.Contracts.Conllu;
using ReadABit.Core.Integrations.SparvPipelineProxy;
using ReadABit.Core.Integrations.Ufal;

namespace ReadABit.Core.Integrations.Services
{
    public abstract class ConlluServiceBase
    {
        private readonly IServiceProvider _serviceProvider;
        protected UDPipeV1Service UDPipeV1Service => new(_serviceProvider);
        protected SparvPipelineProxyService SparvPipelineProxyService => new(_serviceProvider);
        protected SparvPipelineProxyClient SparvPipelineProxyClient => _serviceProvider.GetRequiredService<SparvPipelineProxyClient>();

        public ConlluServiceBase(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <param name="twoLetterISOLanguageName"><see cref="CultureInfo.TwoLetterISOLanguageName" /></param>
        /// <param name="input">Text content of the input.</param>
        public abstract Task<Conllu.Document> ToConlluDocument(string languageCode, string input);
    }
}

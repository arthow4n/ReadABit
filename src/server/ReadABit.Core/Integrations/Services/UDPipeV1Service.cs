using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Ufal.UDPipe;

namespace ReadABit.Core.Integrations.Services
{
    public class UDPipeV1Service
    {
        private readonly Pipeline _pipeline;

        public UDPipeV1Service(ModelLanguage lang)
        {
            var modelPath = Path.Join(
                Path.Join(
                    Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location),
                    "Integrations",
                    "Ufal",
                    "UDPipe",
                    "models"
                ),
                ModelLanguageToFileNameMapping[lang]
            );
            var model = Model.load(modelPath);
            var pipeline = new Pipeline(
                model,
                "tokenize",
                Pipeline.DEFAULT,
                Pipeline.DEFAULT,
                "conllu"
            );

            _pipeline = pipeline;
        }

        /// <returns>CoNLL-U annotaion of the input. See https://universaldependencies.org/format.html for full spec.</returns>
        public string ConvertToConllu(string input)
        {
            var error = new ProcessingError();

            var processed = _pipeline.process(input, error);

            if (error.occurred())
            {
                throw new Exception(error.message);
            }

            return processed;
        }

        public enum ModelLanguage
        {
            Swedish,
        }

        private static Dictionary<ModelLanguage, string> ModelLanguageToFileNameMapping => new()
        {
            { ModelLanguage.Swedish, "swedish-ud-1.2-160523.udpipe" },
        };
    }
}

﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using ReadABit.Core.Integrations.Contracts.Conllu;
using Ufal.UDPipe;

namespace ReadABit.Core.Integrations.Services
{
    public static class UDPipeV1Service
    {
        /// <param name="twoLetterISOLanguageName"><see cref="CultureInfo.TwoLetterISOLanguageName" /></param>
        /// <param name="input">Text content of the input.</param>
        public static Conllu.Document ToConlluDocument(string twoLetterISOLanguageName, string input)
        {
            return ToConllu(twoLetterISOLanguageName, input).ToConlluDocument();
        }

        /// <param name="twoLetterISOLanguageName"><see cref="CultureInfo.TwoLetterISOLanguageName" /></param>
        /// <param name="input">Text content of the input.</param>
        /// <returns>CoNLL-U annotaion of the input. See https://universaldependencies.org/format.html for full spec.</returns>
        public static string ToConllu(string twoLetterISOLanguageName, string input)
        {
            var pipeline = new Pipeline(
                LanguageToModelMapping[twoLetterISOLanguageName].Value,
                "tokenize",
                Pipeline.DEFAULT,
                Pipeline.DEFAULT,
                "conllu"
            );

            var error = new ProcessingError();
            var processed = pipeline.process(input, error);

            if (error.occurred())
            {
                throw new UDPipeException(error.message);
            }

            return processed;
        }

        private static ReadOnlyDictionary<string, Lazy<Model>> LanguageToModelMapping => new(new Dictionary<string, Lazy<Model>>
        {
            { "sv", new(() => LoadModel("swedish-ud-1.2-160523.udpipe")) },
        });

        private static Model LoadModel(string modelName)
        {
            var modelPath = Path.Join(
                Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location),
                "Integrations",
                "Ufal",
                "UDPipe",
                "models",
                modelName
            );

            return Model.load(modelPath);
        }

        public class UDPipeException : Exception
        {
            public UDPipeException(string? message) : base(message)
            {
            }
        }
    }
}

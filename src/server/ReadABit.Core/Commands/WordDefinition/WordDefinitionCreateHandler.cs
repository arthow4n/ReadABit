using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using ReadABit.Core.Commands.Utils;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public class WordDefinitionCreateHandler : CommandHandlerBase, IRequestHandler<WordDefinitionCreate, WordDefinition>
    {
        public WordDefinitionCreateHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<WordDefinition> Handle(WordDefinitionCreate request, CancellationToken cancellationToken)
        {
            new WordDefinitionCreateValidator().ValidateAndThrow(request);

            var wordId = await request.Word.GetIdAndEnsureCreated(DB, Mapper, cancellationToken);

            var wordDefinition = new WordDefinition
            {
                Id = Guid.NewGuid(),
                WordId = wordId,
                UserId = request.UserId,
                Public = request.Public,
                LanguageCode = request.LanguageCode,
                Meaning = request.Meaning.Trim(),
                CreatedAt = Clock.GetCurrentInstant(),
                UpdatedAt = Clock.GetCurrentInstant(),
            };

            await DB.Unsafe.AddAsync(wordDefinition, cancellationToken);

            return wordDefinition;
        }
    }
}

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReadABit.Core.Commands.Utils;
using ReadABit.Core.Utils;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public class WordFamiliarityUpsertHandler : CommandHandlerBase, IRequestHandler<WordFamiliarityUpsert, bool>
    {
        public WordFamiliarityUpsertHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<bool> Handle(WordFamiliarityUpsert request, CancellationToken cancellationToken)
        {
            new WordFamiliarityUpdateValidator().ValidateAndThrow(request);

            var wordId = await request.Word.GetIdAndEnsureCreated(DB, cancellationToken);

            var wordFamiliarity = await DB.WordFamiliaritiesOfUser(request.UserId)
                                          .Where(wf => wf.WordId == wordId)
                                          .SingleOrDefaultAsync(cancellationToken: cancellationToken);

            if (wordFamiliarity is not null)
            {
                wordFamiliarity.Level = request.Level;
                return true;
            }

            await DB.Unsafe.AddAsync(new WordFamiliarity
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Level = request.Level,
                WordId = wordId,
            }, cancellationToken);

            return true;
        }
    }
}

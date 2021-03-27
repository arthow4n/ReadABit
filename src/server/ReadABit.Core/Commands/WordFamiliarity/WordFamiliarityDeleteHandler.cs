using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReadABit.Core.Commands.Utils;
using ReadABit.Core.Utils;

namespace ReadABit.Core.Commands
{
    public class WordFamiliarityDeleteHandler : CommandHandlerBase, IRequestHandler<WordFamiliarityDelete, bool>
    {
        public WordFamiliarityDeleteHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<bool> Handle(WordFamiliarityDelete request, CancellationToken cancellationToken)
        {
            var target =
                await DB.WordFamiliaritiesOfUser(request.UserId)
                         .Where(up => up.Id == request.Id)
                         .SingleOrDefaultAsync(cancellationToken: cancellationToken);

            if (target is null)
            {
                return false;
            }

            DB.Unsafe.Remove(target);
            return true;
        }
    }
}

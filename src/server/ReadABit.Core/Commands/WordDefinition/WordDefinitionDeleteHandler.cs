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
    public class WordDefinitionDeleteHandler : CommandHandlerBase, IRequestHandler<WordDefinitionDelete, bool>
    {
        public WordDefinitionDeleteHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<bool> Handle(WordDefinitionDelete request, CancellationToken cancellationToken)
        {
            var target =
                await DB.WordDefinitionsOfUser(request.UserId)
                         .Where(wd => wd.Id == request.Id)
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

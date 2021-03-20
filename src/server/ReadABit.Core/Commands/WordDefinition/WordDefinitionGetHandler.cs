using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReadABit.Core.Commands.Utils;
using ReadABit.Core.Utils;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public class WordDefinitionGetHandler : CommandHandlerBase, IRequestHandler<WordDefinitionGet, WordDefinition?>
    {
        public WordDefinitionGetHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<WordDefinition?> Handle(WordDefinitionGet request, CancellationToken cancellationToken)
        {
            return await DB.WordDefinitionsOfUserOrPublic(request.UserId)
                            .AsNoTracking()
                            .Where(wd => wd.Id == request.Id)
                            .SingleOrDefaultAsync(cancellationToken: cancellationToken);
        }
    }
}

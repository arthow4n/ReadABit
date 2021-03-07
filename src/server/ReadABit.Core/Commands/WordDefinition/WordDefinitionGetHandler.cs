using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReadABit.Infrastructure.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ReadABit.Core.Utils;

namespace ReadABit.Core.Commands
{
    public class WordDefinitionGetHandler : IRequestHandler<WordDefinitionGet, WordDefinition?>
    {
        private readonly DB _db;

        public WordDefinitionGetHandler(DB db)
        {
            _db = db;
        }

        public async Task<WordDefinition?> Handle(WordDefinitionGet request, CancellationToken cancellationToken)
        {
            return await _db.WordDefinitionsOfUserOrPublic(request.UserId)
                            .AsNoTracking()
                            .Where(wd => wd.Id == request.Id)
                            .SingleOrDefaultAsync(cancellationToken: cancellationToken);
        }
    }
}

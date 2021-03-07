using System.Threading;
using System.Threading.Tasks;
using MediatR;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ReadABit.Core.Utils;

namespace ReadABit.Core.Commands
{
    public class WordDefinitionDeleteHandler : IRequestHandler<WordDefinitionDelete, bool>
    {
        private readonly DB _db;

        public WordDefinitionDeleteHandler(DB db)
        {
            _db = db;
        }

        public async Task<bool> Handle(WordDefinitionDelete request, CancellationToken cancellationToken)
        {
            var target =
                await _db.WordDefinitionsOfUser(request.UserId)
                         .Where(wd => wd.Id == request.Id)
                         .SingleOrDefaultAsync(cancellationToken: cancellationToken);

            if (target is null)
            {
                return false;
            }

            _db.Unsafe.Remove(target);
            return true;
        }
    }
}

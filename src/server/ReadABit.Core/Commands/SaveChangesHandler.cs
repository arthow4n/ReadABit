using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReadABit.Core.Commands;
using ReadABit.Core.Utils;
using ReadABit.Infrastructure;

namespace ReadABit.Core.Database.CommandHandlers
{
    public class SaveChangesHandler : IRequestHandler<SaveChanges, int>
    {
        private readonly DB _db;

        public SaveChangesHandler(DB db)
        {
            _db = db;
        }

        public async Task<int> Handle(SaveChanges request, CancellationToken cancellationToken)
        {
            return await _db.Unsafe.SaveChangesAsync();
        }
    }
}

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReadABit.Core.Commands;
using ReadABit.Infrastructure;

namespace ReadABit.Core.Database.CommandHandlers
{
    public class SaveChangesHandler : IRequestHandler<SaveChanges, int>
    {
        private readonly CoreDbContext db;

        public SaveChangesHandler(CoreDbContext db)
        {
            this.db = db;
        }

        public async Task<int> Handle(SaveChanges request, CancellationToken cancellationToken)
        {
            return await db.SaveChangesAsync();
        }
    }
}

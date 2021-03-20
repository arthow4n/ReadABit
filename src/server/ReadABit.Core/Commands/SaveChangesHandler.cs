using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReadABit.Core.Commands;
using ReadABit.Core.Commands.Utils;
using ReadABit.Core.Utils;

namespace ReadABit.Core.Database.CommandHandlers
{
    public class SaveChangesHandler : CommandHandlerBase, IRequestHandler<SaveChanges, int>
    {
        public SaveChangesHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<int> Handle(SaveChanges request, CancellationToken cancellationToken)
        {
            return await DB.Unsafe.SaveChangesAsync(cancellationToken);
        }
    }
}

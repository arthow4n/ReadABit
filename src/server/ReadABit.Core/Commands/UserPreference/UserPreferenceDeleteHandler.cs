using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReadABit.Infrastructure.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ReadABit.Core.Utils;

namespace ReadABit.Core.Commands
{
    public class UserPreferenceDeleteHandler : IRequestHandler<UserPreferenceDelete, bool>
    {
        private readonly DB _db;

        public UserPreferenceDeleteHandler(DB db)
        {
            _db = db;
        }

        public async Task<bool> Handle(UserPreferenceDelete request, CancellationToken cancellationToken)
        {
            var target =
                await _db.UserPreferencesOfUser(request.UserId)
                         .Where(up => up.Id == request.Id)
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

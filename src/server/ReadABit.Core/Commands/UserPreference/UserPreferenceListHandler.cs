using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReadABit.Infrastructure.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ReadABit.Core.Utils;
using System.Collections.Generic;

namespace ReadABit.Core.Commands
{
    public class UserPreferenceListHandler : IRequestHandler<UserPreferenceList, List<UserPreference>>
    {
        private readonly DB _db;

        public UserPreferenceListHandler(DB db)
        {
            _db = db;
        }

        public async Task<List<UserPreference>> Handle(UserPreferenceList request, CancellationToken cancellationToken)
        {
            return await _db.UserPreferencesOfUser(request.UserId)
                            .AsNoTracking()
                            .ToListAsync(cancellationToken: cancellationToken);
        }
    }
}

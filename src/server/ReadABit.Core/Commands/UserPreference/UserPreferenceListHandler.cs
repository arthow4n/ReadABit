using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReadABit.Core.Commands.Utils;
using ReadABit.Core.Utils;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public class UserPreferenceListHandler : CommandHandlerBase, IRequestHandler<UserPreferenceList, List<UserPreference>>
    {
        public UserPreferenceListHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<List<UserPreference>> Handle(UserPreferenceList request, CancellationToken cancellationToken)
        {
            return await DB.UserPreferencesOfUser(request.UserId)
                            .AsNoTracking()
                            .ToListAsync(cancellationToken: cancellationToken);
        }
    }
}

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReadABit.Core.Commands.Utils;
using ReadABit.Core.Utils;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public class UserPreferenceGetHandler : CommandHandlerBase, IRequestHandler<UserPreferenceGet, UserPreferenceData>
    {
        public UserPreferenceGetHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<UserPreferenceData> Handle(UserPreferenceGet request, CancellationToken cancellationToken)
        {
            return await DB.UserPreferencesOfUser(request.UserId)
                           .Select(up => up.Data)
                           .SingleOrDefaultAsync(cancellationToken) ??
                           new UserPreferenceData();
        }
    }
}

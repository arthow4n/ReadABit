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
    public class UserPreferenceUpsertHandler : CommandHandlerBase, IRequestHandler<UserPreferenceUpsert, bool>
    {
        public UserPreferenceUpsertHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<bool> Handle(UserPreferenceUpsert request, CancellationToken cancellationToken)
        {
            new UserPreferenceUpdateValidator().ValidateAndThrow(request);

            var userPreference = await DB.UserPreferencesOfUser(request.UserId)
                                          .Where(up => up.Type == request.Type)
                                          .SingleOrDefaultAsync(cancellationToken: cancellationToken);

            if (userPreference is not null)
            {
                userPreference.Value = request.Value;
                return true;
            }

            await DB.Unsafe.AddAsync(new UserPreference
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Type = request.Type,
                Value = request.Value,
            }, cancellationToken);

            return true;
        }
    }
}

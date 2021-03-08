using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReadABit.Core.Utils;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public class UserPreferenceUpsertHandler : IRequestHandler<UserPreferenceUpsert, bool>
    {
        private readonly DB _db;

        public UserPreferenceUpsertHandler(DB db)
        {
            _db = db;
        }

        public async Task<bool> Handle(UserPreferenceUpsert request, CancellationToken cancellationToken)
        {
            new UserPreferenceUpdateValidator().ValidateAndThrow(request);

            var userPreference = await _db.UserPreferencesOfUser(request.UserId)
                                          .Where(up => up.Type == request.Type)
                                          .SingleOrDefaultAsync(cancellationToken: cancellationToken);

            if (userPreference is not null)
            {
                userPreference.Value = request.Value;
                return true;
            }

            await _db.Unsafe.AddAsync(new UserPreference
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

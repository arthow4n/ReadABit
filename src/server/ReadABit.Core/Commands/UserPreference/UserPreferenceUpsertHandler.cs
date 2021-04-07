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

            await DB.Unsafe.UserPreferences
                .Upsert(new()
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    Data = request.Data,
                })
                .On(x => x.UserId)
                .WhenMatched((existing, updated) => new()
                {
                    Id = existing.Id,
                    UserId = existing.UserId,
                    Data = updated.Data,
                })
                .RunAsync(cancellationToken);

            return true;
        }
    }
}

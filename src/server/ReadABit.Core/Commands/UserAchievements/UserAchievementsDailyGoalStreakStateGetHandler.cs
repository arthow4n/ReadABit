using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReadABit.Core.Commands.Utils;
using ReadABit.Core.Contracts;

namespace ReadABit.Core.Commands.UserAchievements
{
    public class UserAchievementsDailyGoalStreakStateGetHandler : CommandHandlerBase, IRequestHandler<UserAchievementsDailyGoalStreakGet, UserAchievementsDailyGoalStreakStateViewModel>
    {
        public UserAchievementsDailyGoalStreakStateGetHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<UserAchievementsDailyGoalStreakStateViewModel> Handle(UserAchievementsDailyGoalStreakGet request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

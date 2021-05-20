using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using ReadABit.Core.Commands.Utils;
using ReadABit.Core.Contracts;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands.UserAchievements
{
    public class UserAchievementsDailyGoalStreakStateGetHandler : CommandHandlerBase, IRequestHandler<UserAchievementsDailyGoalStreakGetInternal, UserAchievementsDailyGoalStreakStateViewModel>
    {
        public UserAchievementsDailyGoalStreakStateGetHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<UserAchievementsDailyGoalStreakStateViewModel> Handle(UserAchievementsDailyGoalStreakGetInternal request, CancellationToken cancellationToken)
        {
            var checkResult = request.DailyGoalCheckViewModel;
            var meta = checkResult.Metadata;

            var streakCheckEffectiveDate =
                meta.IsNowEarlierThanTodaysReset && !checkResult.NewlyCreatedReached ?
                    meta.CurrentDailyGoalPeriodStart.LocalDateTime.Date.Minus(Period.FromDays(1)) :
                    meta.CurrentDailyGoalPeriodStart.LocalDateTime.Date;

            var streaks =
                await DB.UserAchievementStreaksOfUser(request.UserId, UserAchievementType.WordFamiliarityDailyGoalReached)
                    .ToListAsync(cancellationToken);

            var currentStreakDays =
                await DB.UserAchievementStreaksOfUser(request.UserId, UserAchievementType.WordFamiliarityDailyGoalReached)
                    .Where(s => s.LastEffectiveDateInStreak == streakCheckEffectiveDate)
                    .Select(s => s.StreakDays)
                    .SingleOrDefaultAsync(cancellationToken);

            return new()
            {
                CurrentStreakDays = currentStreakDays,
                DailyGoalCheckResult = request.DailyGoalCheckViewModel,
            };
        }
    }
}

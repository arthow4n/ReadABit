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
    public class UserAchievementsDailyGoalStreakStateGetHandler : CommandHandlerBase, IRequestHandler<UserAchievementsDailyGoalStreakGet, UserAchievementsDailyGoalStreakStateViewModel>
    {
        public UserAchievementsDailyGoalStreakStateGetHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<UserAchievementsDailyGoalStreakStateViewModel> Handle(UserAchievementsDailyGoalStreakGet request, CancellationToken cancellationToken)
        {
            var checkResult = request.DailyGoalCheckViewModel;
            var meta = checkResult.Metadata;

            // var streakCheckDateInUtc =
            //     meta.IsNowEarlierThanTodaysReset && !checkResult.NewlyCreatedReached ?
            //         meta.NowInRequestedZone.Minus(Duration.FromDays(1)).LocalDateTime.Date :
            //         meta.NowInRequestedZone.LocalDateTime.Date;

            var startOfDailyGoalPeriodInRequestedZone =
                meta.StartOfDailyGoalPeriod
                    .InZone(meta.NowInRequestedZone.Zone);

            if (meta.IsNowEarlierThanTodaysReset && checkResult.NewlyCreatedReached)
            {
                startOfDailyGoalPeriodInRequestedZone += Duration.FromDays(1);
            }

            var streakCheckUtcStartOfDate =
                startOfDailyGoalPeriodInRequestedZone
                    .WithLocalTime("00:00:00".ParseIsoHhmmssToLocalTimeOrThrow())
                    .Plus(Duration.FromTicks(meta.NowInRequestedZone.Offset.Ticks))
                    .ToInstant();

            var streaks =
                await DB.UserAchievementStreaksOfUser(request.UserId, UserAchievementType.WordFamiliarityDailyGoalReached)
                    .ToListAsync(cancellationToken);

            var currentStreakDays =
                await DB.UserAchievementStreaksOfUser(request.UserId, UserAchievementType.WordFamiliarityDailyGoalReached)
                    .Where(s => s.LastUtcStartOfDateInStreak == streakCheckUtcStartOfDate)
                    .Select(s => s.StreakDays)
                    .SingleOrDefaultAsync(cancellationToken);

            return new()
            {
                CurrentStreakDays = currentStreakDays,
            };
        }
    }
}

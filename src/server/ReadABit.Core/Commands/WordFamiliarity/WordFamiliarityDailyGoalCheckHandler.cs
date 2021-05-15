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
using ReadABit.Core.Utils;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public class WordFamiliarityDailyGoalCheckHandler : CommandHandlerBase, IRequestHandler<WordFamiliarityDailyGoalCheck, WordFamiliarityDailyGoalCheckViewModel>
    {
        public WordFamiliarityDailyGoalCheckHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<WordFamiliarityDailyGoalCheckViewModel> Handle(WordFamiliarityDailyGoalCheck request, CancellationToken cancellationToken)
        {
            new WordFamiliarityDailyGoalCheckValidator().ValidateAndThrow(request);

            var nowInRequestedZone = Clock
                .GetCurrentInstant()
                .InZone(request
                    .DailyGoalResetTimeTimeZone
                    .ParseToDateTimeZoneOrThrow()
                );

            var dailyGoalResetTimeInTheSameDate = nowInRequestedZone.SwapLocalTime(
                request
                    .DailyGoalResetTimePartial
                    .ParseIsoHhmmssToLocalTimeOrThrow()
                );

            var isNowEarlierThanTodaysReset = (nowInRequestedZone - dailyGoalResetTimeInTheSameDate) < Duration.Zero;

            var dailyGoalPeriodStart =
                (isNowEarlierThanTodaysReset ?
                    dailyGoalResetTimeInTheSameDate.Minus(Duration.FromDays(1)) :
                    dailyGoalResetTimeInTheSameDate
                )
                    .ToInstant();

            var dailyGoalPeriodEnd =
                (isNowEarlierThanTodaysReset ?
                    dailyGoalResetTimeInTheSameDate :
                    dailyGoalResetTimeInTheSameDate.Plus(Duration.FromDays(1))
                )
                    .ToInstant();

            var newlyCreatedWordFamiliarityDuringPeriod =
                await DB.WordFamiliaritiesOfUser(request.UserId)
                    .Where(wf =>
                        wf.CreatedAt >= dailyGoalPeriodStart &&
                        wf.CreatedAt < dailyGoalPeriodEnd
                    )
                    .CountAsync(wf => wf.Level >= 1, cancellationToken);

            var newlyCreatedReached =
                newlyCreatedWordFamiliarityDuringPeriod >=
                request.DailyGoalNewlyCreatedWordFamiliarityCount;

            if (newlyCreatedReached)
            {
                // It's fine to insert multiple entries in a day,
                // we just want to avoid writing it all the time,
                // therefore it's fine that this check misses when race condition happens.
                var isDailyGoalReachedAchievementCreated =
                    await DB.UserAchievementsOfUser(request.UserId)
                        .Where(ua =>
                            ua.CreatedAt >= dailyGoalPeriodStart &&
                            ua.CreatedAt < dailyGoalPeriodEnd
                        )
                        .AnyAsync(cancellationToken);

                if (!isDailyGoalReachedAchievementCreated)
                {
                    await DB.Unsafe.UserAchievements.AddAsync(new()
                    {
                        Id = Guid.NewGuid(),
                        UserId = request.UserId,
                        Type = UserAchievementType.WordFamiliarityDailyGoalReached,
                        CreatedAt = Clock.GetCurrentInstant(),
                    }, cancellationToken);
                }
            }

            return new WordFamiliarityDailyGoalCheckViewModel
            {
                NewlyCreated = newlyCreatedWordFamiliarityDuringPeriod,
                NewlyCreatedGoal = request.DailyGoalNewlyCreatedWordFamiliarityCount,
                NewlyCreatedReached = newlyCreatedReached,
            };
        }
    }
}

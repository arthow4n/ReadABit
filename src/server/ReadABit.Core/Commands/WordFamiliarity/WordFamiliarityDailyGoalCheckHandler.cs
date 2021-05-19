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
using Z.EntityFramework.Plus;

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

            var requestedResetTimeZone =
                request
                    .DailyGoalResetTimeTimeZone
                    .ParseToDateTimeZoneOrThrow();

            var nowInRequestedResetTimeZone = Clock
                .GetCurrentInstant()
                .InZone(requestedResetTimeZone);

            var dailyGoalResetTimeInTheSameDate = nowInRequestedResetTimeZone.SwapLocalTime(
                request
                    .DailyGoalResetTimePartial
                    .ParseIsoHhmmssToLocalTimeOrThrow()
                );

            var isNowEarlierThanTodaysReset = (nowInRequestedResetTimeZone - dailyGoalResetTimeInTheSameDate) < Duration.Zero;

            var dailyGoalPeriodStart =
               isNowEarlierThanTodaysReset ?
                   dailyGoalResetTimeInTheSameDate.Minus(Duration.FromDays(1)) :
                   dailyGoalResetTimeInTheSameDate;

            var dailyGoalPeriodStartInstant = dailyGoalPeriodStart.ToInstant();

            var dailyGoalPeriodEnd =
                isNowEarlierThanTodaysReset ?
                    dailyGoalResetTimeInTheSameDate :
                    dailyGoalResetTimeInTheSameDate.Plus(Duration.FromDays(1));

            var dailyGoalPeriodEndInstant = dailyGoalPeriodEnd.ToInstant();

            var newlyCreatedWordFamiliarityDuringPeriod =
                await DB.WordFamiliaritiesOfUser(request.UserId)
                    .Where(wf =>
                        wf.CreatedAt >= dailyGoalPeriodStartInstant &&
                        wf.CreatedAt < dailyGoalPeriodEndInstant
                    )
                    .CountAsync(wf => wf.Level >= 1, cancellationToken);

            var newlyCreatedReached =
                newlyCreatedWordFamiliarityDuringPeriod >=
                request.DailyGoalNewlyCreatedWordFamiliarityCount;


            // It's fine to insert multiple entries in a day,
            // we just want to avoid writing it all the time,
            // therefore it's fine that this check misses when race condition happens.
            var isDailyGoalReachedAchievementCreated =
                await DB.UserAchievementsOfUser(request.UserId)
                    .Where(ua =>
                        ua.CreatedAt >= dailyGoalPeriodStartInstant &&
                        ua.CreatedAt < dailyGoalPeriodEndInstant
                    )
                    .AnyAsync(cancellationToken);

            var effectiveDateForAchievement =
                dailyGoalPeriodStartInstant
                    .InZone(requestedResetTimeZone)
                    .Date;

            if (newlyCreatedReached && !isDailyGoalReachedAchievementCreated)
            {
                await DB.Unsafe.UserAchievements.AddAsync(new()
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    Type = UserAchievementType.WordFamiliarityDailyGoalReached,
                    EffectiveDate = effectiveDateForAchievement,
                    CreatedAt = nowInRequestedResetTimeZone.ToInstant(),
                }, cancellationToken);
            }
            else if (!newlyCreatedReached)
            {
                await DB.UserAchievementsOfUser(request.UserId)
                    .Where(ua =>
                        ua.Type == UserAchievementType.WordFamiliarityDailyGoalReached &&
                        ua.EffectiveDate == effectiveDateForAchievement
                    )
                    .DeleteAsync(cancellationToken);
            }

            return new WordFamiliarityDailyGoalCheckViewModel
            {
                NewlyCreated = newlyCreatedWordFamiliarityDuringPeriod,
                NewlyCreatedGoal = request.DailyGoalNewlyCreatedWordFamiliarityCount,
                NewlyCreatedReached = newlyCreatedReached,
                Metadata = new()
                {
                    IsNowEarlierThanTodaysReset = isNowEarlierThanTodaysReset,
                    CurrentDailyGoalPeriodStart = dailyGoalPeriodStart,
                }
            };
        }
    }
}

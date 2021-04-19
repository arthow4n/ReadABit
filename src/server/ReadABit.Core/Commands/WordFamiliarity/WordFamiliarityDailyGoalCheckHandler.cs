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

            return new WordFamiliarityDailyGoalCheckViewModel
            {
                NewlyCreated = newlyCreatedWordFamiliarityDuringPeriod,
                NewlyCreatedGoal = request.DailyGoalNewlyCreatedWordFamiliarityCount,
                NewlyCreatedReached = newlyCreatedWordFamiliarityDuringPeriod >= request.DailyGoalNewlyCreatedWordFamiliarityCount,
            };
        }
    }
}

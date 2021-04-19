using System;
using System.Text.Json.Serialization;
using FluentValidation;
using MediatR;
using NSwag.Annotations;
using ReadABit.Core.Contracts;

namespace ReadABit.Core.Commands
{
    public record WordFamiliarityDailyGoalCheck : IRequest<WordFamiliarityDailyGoalCheckViewModel>
    {
        [OpenApiIgnore, JsonIgnore]
        public Guid UserId { get; init; }
        public string DailyGoalResetTimeTimeZone { get; init; } = "Europe/London";
        public string DailyGoalResetTimePartial { get; init; } = "00:00:00";
        public int DailyGoalNewlyCreatedWordFamiliarityCount { get; set; }
    }

    public class WordFamiliarityDailyGoalCheckValidator : AbstractValidator<WordFamiliarityDailyGoalCheck>
    {
        public WordFamiliarityDailyGoalCheckValidator()
        {
            RuleFor(x => x.DailyGoalResetTimeTimeZone).MustBeSupportedTimeZone();
            RuleFor(x => x.DailyGoalResetTimePartial).MustBeIsoHHmmss();
            RuleFor(x => x.DailyGoalNewlyCreatedWordFamiliarityCount).GreaterThanOrEqualTo(0);
        }
    }
}

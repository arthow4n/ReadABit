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
        [OpenApiIgnore, JsonIgnore]
        public string DailyGoalResetTimeTimeZone { get; init; } = "Europe/London";
        [OpenApiIgnore, JsonIgnore]
        public string DailyGoalResetTimePartial { get; init; } = "00:00:00";
        [OpenApiIgnore, JsonIgnore]
        public int DailyGoalNewlyCreatedWordFamiliarityCount { get; set; } = 25;
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

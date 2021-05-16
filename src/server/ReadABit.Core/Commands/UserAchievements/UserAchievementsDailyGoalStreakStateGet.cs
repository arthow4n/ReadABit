using System;
using System.Text.Json.Serialization;
using MediatR;
using NSwag.Annotations;
using ReadABit.Core.Contracts;

namespace ReadABit.Core.Commands.UserAchievements
{
    public record UserAchievementsDailyGoalStreakGet : IRequest<UserAchievementsDailyGoalStreakStateViewModel>
    {
        [OpenApiIgnore, JsonIgnore]
        public Guid UserId { get; init; }
    }
}

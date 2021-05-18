using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace ReadABit.Infrastructure.Models
{
    [Keyless]
    public class UserAchievementStreak
    {
        public Instant LastUtcStartOfDateInStreak { get; init; }
        public int StreakDays { get; init; }
    }
}

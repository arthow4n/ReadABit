using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NodaTime;
using ReadABit.Infrastructure.Interfaces;

namespace ReadABit.Infrastructure.Models
{
    public class UserAchievement : IEntityWithCreateTimestamp
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
        [Required]
        public UserAchievementType Type { get; set; }
        [Required]
        public Instant CreatedAt { get; set; }
    }

    public enum UserAchievementType
    {
        Invalid = 0,
        // This one is saved here mainly to simplify the calculation for the case
        // where a user might want to set a word's familiarity back to 0
        // and that word familiarity entry was created in the past.
        // We gain more flexibility by decoupling the achievement from actual word familiarity states.
        // This also makes it easier to "fix" daily goal streak to motivate an user who missed a day.
        WordFamiliarityDailyGoalReached = 1,
    }
}

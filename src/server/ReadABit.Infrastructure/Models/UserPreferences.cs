using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadABit.Infrastructure.Models
{
    public class UserPreference
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public ApplicationUser User { get; init; }
        [Column(TypeName = "jsonb")]
        public UserPreferenceData Data { get; set; }
    }
}

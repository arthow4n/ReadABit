using NodaTime;

namespace ReadABit.Infrastructure.Interfaces
{
    public interface IEntityWithCreateUpdateTimestamps : IEntityWithCreateTimestamp
    {
        public Instant UpdatedAt { get; set; }
    }
}

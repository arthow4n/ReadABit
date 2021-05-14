using NodaTime;

namespace ReadABit.Infrastructure.Interfaces
{
    public interface IEntityWithCreateTimestamp
    {
        public Instant CreatedAt { get; set; }
    }
}

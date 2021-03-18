using NodaTime;

namespace ReadABit.Infrastructure.Interfaces
{
    public interface ITimestampedEntity
    {
        public Instant CreatedAt { get; set; }
        public Instant UpdatedAt { get; set; }
    }
}

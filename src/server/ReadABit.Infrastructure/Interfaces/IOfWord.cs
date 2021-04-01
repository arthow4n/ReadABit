using System;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Infrastructure.Interfaces
{
    public interface IOfWord
    {
        public Guid WordId { get; set; }
        public Word Word { get; set; }
    }
}

using MediatR;

namespace ReadABit.Core.Commands
{
    public record SaveChanges : IRequest<int>
    {
    }
}

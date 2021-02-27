using MediatR;

namespace ReadABit.Core.Services.Utils
{
    public abstract class ServiceBase
    {
        public ServiceBase(IMediator mediator)
        {
            this.mediator = mediator;
        }

        protected IMediator mediator;
    }
}

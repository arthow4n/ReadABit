using MediatR;
using ReadABit.Core.Utils;

namespace ReadABit.Core.Services.Utils
{
    public abstract class ServiceBase
    {
        public ServiceBase(IMediator mediator, IRequestContext requestContext)
        {
            this.mediator = mediator;
            this.requestContext = requestContext;
        }

        protected readonly IMediator mediator;
        protected readonly IRequestContext requestContext;
    }
}

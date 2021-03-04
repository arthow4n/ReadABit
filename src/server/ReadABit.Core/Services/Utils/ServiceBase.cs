using MediatR;
using ReadABit.Core.Utils;

namespace ReadABit.Core.Services.Utils
{
    public abstract class ServiceBase
    {
        public ServiceBase(IMediator mediator, IRequestContext requestContext)
        {
            Mediator = mediator;
            RequestContext = requestContext;
        }

        protected readonly IMediator Mediator;
        protected readonly IRequestContext RequestContext;
    }
}

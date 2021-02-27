using System;
using MediatR;

namespace ReadABit.Core.Services.Utils
{
    public abstract class ServiceBase
    {
        public ServiceBase(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private readonly IServiceProvider _serviceProvider;
        private IMediator? _mediator;

        protected IMediator Mediator
        {
            get
            {
                _mediator ??= (IMediator?)_serviceProvider.GetService(typeof(IMediator));
                if (_mediator is null)
                {
                    throw new NullReferenceException("ApiControllerBase: Failed to locate IMediator.");
                }
                return _mediator;
            }
        }
    }
}
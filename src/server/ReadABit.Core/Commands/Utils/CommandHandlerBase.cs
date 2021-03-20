using System;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using ReadABit.Core.Utils;

namespace ReadABit.Core.Commands.Utils
{
    public class CommandHandlerBase
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandHandlerBase(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected DB DB => _serviceProvider.GetRequiredService<DB>();
        protected IMapper Mapper => _serviceProvider.GetRequiredService<IMapper>();
        protected IClock Clock => _serviceProvider.GetRequiredService<IClock>();
    }
}

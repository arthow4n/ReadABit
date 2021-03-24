using System;
using Microsoft.Extensions.DependencyInjection;

namespace ReadABit.Core.Utils
{
    /// <summary>
    /// Helper method(s) for easier creating a service with its dependency injected.
    /// Named as DI just because it's generic dependency injection.
    /// </summary>
    public static class DI
    {
        public static T New<T>(IServiceProvider serviceProvider)
        {
            return ActivatorUtilities.CreateInstance<T>(serviceProvider);
        }
    }
}

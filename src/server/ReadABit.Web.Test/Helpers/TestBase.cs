using System;
using ReadABit.Core.Utils;

namespace ReadABit.Web.Test.Helpers
{
    public class TestBase
    {
        public TestBase(IServiceProvider serviceProvider, IRequestContext requestContext)
        {
            if (requestContext is not RequestContextMock)
            {
                throw new NotSupportedException();
            }
            _requestContext = (RequestContextMock)requestContext;
            _requestContext.SignIn();
            ServiceProvider = serviceProvider;
        }

        private readonly RequestContextMock _requestContext;
        protected readonly IServiceProvider ServiceProvider;

    }

    /// <summary>
    /// Automatically creates an instance of <typeparamref name="T1T"> with all the dependencies injected.
    /// </summary>
    public class TestBase<T1T> : TestBase
    {
        protected readonly T1T T1;
        public TestBase(IServiceProvider serviceProvider, IRequestContext requestContext) : base(serviceProvider, requestContext)
        {
            T1 = DI.New<T1T>(serviceProvider);
        }
    }

    /// <summary>
    /// Similar to <see cref="TestBase<T1>" /> but creates more injected instances for you.
    /// </summary>
    public class TestBase<T1T, T2T> : TestBase<T1T>
    {
        protected readonly T2T T2;
        public TestBase(IServiceProvider serviceProvider, IRequestContext requestContext) : base(serviceProvider, requestContext)
        {
            T2 = DI.New<T2T>(serviceProvider);
        }
    }


    /// <summary>
    /// Similar to <see cref="TestBase<T1>" /> but creates more injected instances for you.
    /// </summary>
    public class TestBase<T1T, T2T, T3T> : TestBase<T1T, T2T>
    {
        protected readonly T3T T3;
        public TestBase(IServiceProvider serviceProvider, IRequestContext requestContext) : base(serviceProvider, requestContext)
        {
            T3 = DI.New<T3T>(serviceProvider);
        }
    }
}

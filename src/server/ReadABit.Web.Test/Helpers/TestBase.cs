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
            this.requestContext = (RequestContextMock)requestContext;
            this.requestContext.SignIn();
            this.serviceProvider = serviceProvider;
        }

        private readonly RequestContextMock requestContext;
        protected readonly IServiceProvider serviceProvider;

    }

    /// <summary>
    /// Automatically creates an instance of <typeparamref name="T1"> with all the dependencies injected.
    /// </summary>
    public class TestBase<T1> : TestBase
    {
        protected readonly T1 t1;
        public TestBase(IServiceProvider serviceProvider, IRequestContext requestContext) : base(serviceProvider, requestContext)
        {
            t1 = DI.New<T1>(serviceProvider);
        }
    }

    /// <summary>
    /// Similar to <see cref="TestBase<T1>" /> but creates more injected instances for you.
    /// </summary>
    public class TestBase<T1, T2> : TestBase<T1>
    {
        protected readonly T2 t2;
        public TestBase(IServiceProvider serviceProvider, IRequestContext requestContext) : base(serviceProvider, requestContext)
        {
            t2 = DI.New<T2>(serviceProvider);
        }
    }


    /// <summary>
    /// Similar to <see cref="TestBase<T1>" /> but creates more injected instances for you.
    /// </summary>
    public class TestBase<T1, T2, T3> : TestBase<T1, T2>
    {
        protected readonly T3 t3;
        public TestBase(IServiceProvider serviceProvider, IRequestContext requestContext) : base(serviceProvider, requestContext)
        {
            t3 = DI.New<T3>(serviceProvider);
        }
    }
}

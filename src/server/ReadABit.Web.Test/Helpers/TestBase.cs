using System;
using System.Transactions;
using Microsoft.Extensions.DependencyInjection;

namespace ReadABit.Web.Test.Helpers
{
    public class TestBase
    {
        public TestBase(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        protected readonly IServiceProvider serviceProvider;
    }

    /// <summary>
    /// Automatically creates an instance of <typeparamref name="T1"> with all the dependencies injected.
    /// </summary>
    public class TestBase<T1> : TestBase
    {
        protected readonly T1 t1;

        public TestBase(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            t1 = (T1)ActivatorUtilities.CreateInstance(serviceProvider, typeof(T1));
        }
    }

    /// <summary>
    /// Similar to <see cref="TestBase<T1>" /> but creates more injected instances for you.
    /// </summary>
    public class TestBase<T1, T2> : TestBase<T1>
    {
        protected readonly T2 t2;

        public TestBase(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            t2 = (T2)ActivatorUtilities.CreateInstance(serviceProvider, typeof(T2));
        }
    }


    /// <summary>
    /// Similar to <see cref="TestBase<T1>" /> but creates more injected instances for you.
    /// </summary>
    public class TestBase<T1, T2, T3> : TestBase<T1>
    {
        protected readonly T3 t3;

        public TestBase(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            t3 = (T3)ActivatorUtilities.CreateInstance(serviceProvider, typeof(T3));
        }
    }
}

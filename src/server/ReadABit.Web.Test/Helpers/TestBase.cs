using System;
using System.Threading.Tasks;
using System.Transactions;
using ReadABit.Core.Utils;

namespace ReadABit.Web.Test.Helpers
{
    public class TestBase : IDisposable
    {
        public TestBase(IServiceProvider serviceProvider, IRequestContext requestContext)
        {
            if (requestContext is not RequestContextMock)
            {
                throw new Exception("Did you forget to setup dependency injection with `services.AddScoped<IRequestContext, RequestContextMock>();`?");
            }
            _ts = new TransactionScope(asyncFlowOption: TransactionScopeAsyncFlowOption.Enabled);

            RequestContext = (RequestContextMock)requestContext;
            RequestContext.SignInWithUser(1).GetAwaiter().GetResult();
            ServiceProvider = serviceProvider;
        }

        protected readonly RequestContextMock RequestContext;
        protected readonly IServiceProvider ServiceProvider;
        protected ScopedAnotherUser User(int userNo)
        {
            if (userNo < 2)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(userNo),
                    "The first user in test environment is 1 instead of 0 to avoid ambiguity when writing/reading tests." +
                    "Besides, you are user 1 by default when you are not in a using (User()) {} block."
                );
            }
            return new ScopedAnotherUser(RequestContext, userNo);
        }

        protected class ScopedAnotherUser : IDisposable
        {
            private bool _disposedValue;
            private readonly RequestContextMock _requestContext;

            public ScopedAnotherUser(RequestContextMock requestContext, int userNo)
            {
                _requestContext = requestContext;
                _requestContext.SignInWithUser(userNo).GetAwaiter().GetResult();
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!_disposedValue)
                {
                    if (disposing)
                    {
                        _requestContext.SignInWithUser(1).GetAwaiter().GetResult();
                    }

                    _disposedValue = true;
                }
            }

            public void Dispose()
            {
                // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }



        private readonly TransactionScope _ts;
        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _ts.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
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

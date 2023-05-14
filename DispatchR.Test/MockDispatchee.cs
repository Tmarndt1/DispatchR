namespace DispatchR.Test
{
    [DispatchOrder(1)]
    public class MockDispatchee1 : Dispatchee
    {
        private readonly Func<CancellationToken, Task> _func;

        public MockDispatchee1(Func<CancellationToken, Task> func, DispatchTime dispatchTime) : base(dispatchTime)
        {
            _func = func;
        }

        public override Task ExecuteAsync(CancellationToken token = default) => _func(token);
    }

    [DispatchOrder(2)]
    public class MockDispatchee2 : Dispatchee
    {
        private readonly Func<CancellationToken, Task> _func;

        public MockDispatchee2(Func<CancellationToken, Task> func, DispatchTime dispatchTime) : base(dispatchTime)
        {
            _func = func;
        }

        public override Task ExecuteAsync(CancellationToken token = default) => _func(token);
    }
}

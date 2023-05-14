namespace DispatchR.Test
{
    [DispatchOrder(1)]
    public class MockDispatchee1 : Dispatchee
    {
        public MockDispatchee1(Func<CancellationToken, Task> func, IDispatchTime dispatchTime) : base(func, dispatchTime)
        {
            
        }
    }

    [DispatchOrder(2)]
    public class MockDispatchee2 : Dispatchee
    {
        public MockDispatchee2(Func<CancellationToken, Task> func, IDispatchTime dispatchTime) : base(func, dispatchTime)
        {

        }
    }
}

# DispatchR
DispatchR is a lightweight and performant C# .NET library that dispatches asynchronous functions in order at a specified frequency or date time.  

main: ![Build Status](https://github.com/Tmarndt1/NetWorkflow/workflows/.NET/badge.svg?branch=main)

## Give a Star! :star:

If you like or are using this project please give it a star. Thanks!

## At Specified Time Example

```csharp

Dispatcher dispatcher = new Dispatcher(new Dispatchee2((token) =>
{
    return MethodAsync(token); // Make some asynchronous call.
}, DispatchTime.AtMinute(1))); // Will execute at the first minute of every hour.

dispatcher.RunAsync(token); // Run the dispatcher until the CancellationTokenSource is canceled.

```

## At Specified Frequency Example

```csharp

Dispatcher dispatcher = new Dispatcher(new Dispatchee2((token) =>
{
    return MethodAsync(token); // Make some asynchronous call.
}, DispatchTime.AtFrequency(TimeSpan.FromSeconds(10)))); // Will execute every 10 seconds.

dispatcher.RunAsync(token); // Run the dispatcher until the CancellationTokenSource is canceled.

```

## Multiple Dispatchees Example

```csharp

Dispatcher dispatcher = new Dispatcher(new Dispatchee[]
{
    new Dispatchee1((token) =>
    {
        return MethodAsync(token); // Make some asynchronous call.
    }, DispatchTime.AtMinute(1)), // Will execute on the first minute of every hour.
    new Dispatchee2((token) =>
    {
        return MethodAsync(token); // Make some asynchronous call
    }, DispatchTime.AtFrequency(TimeSpan.FromSeconds(10))) // Will execute every 10 seconds.
});

dispatcher.RunAsync(token); // Run the dispatcher until the CancellationTokenSource is canceled.

```

## Order Dispatchee Execution Example

```csharp

// Will execute Dispatchee1 first.
[DispatchOrder(1)]
public class Dispatchee1 : Dispatchee
{
    private readonly Func<CancellationToken, Task> _func;

    public MockDispatchee1(Func<CancellationToken, Task> func, DispatchTime dispatchTime) : base(dispatchTime)
    {
        _func = func;
    }

    public override Task ExecuteAsync(CancellationToken token = default) => _func(token);
}

// Will execute Dispatchee2 second.
[DispatchOrder(2)]
public class Dispatchee2 : Dispatchee
{
    private readonly Func<CancellationToken, Task> _func;

    public MockDispatchee2(Func<CancellationToken, Task> func, DispatchTime dispatchTime) : base(dispatchTime)
    {
        _func = func;
    }

    public override Task ExecuteAsync(CancellationToken token = default) => _func(token);
}

```
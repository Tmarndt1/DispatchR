# DispatchR
DispatchR is a lightweight and performant C# .NET library that dispatches asynchronous functions in order at a specified frequency or date time.  

main: ![Build Status](https://github.com/Tmarndt1/NetWorkflow/workflows/.NET/badge.svg?branch=main)

## At Specified Time Example

```csharp

Dispatcher dispatcher = new Dispatcher(new Dispatchee2((token) =>
{
    return MethodAsync(token); // Make some asynchronous call
}, DispatchTime.AtMinute(1))); // Will execute at the first minute of every hour.

dispatcher.RunAsync(token); // Run the dispatcher until the CancellationTokenSource is cancelled.

```

## At Specified Frequency Example

```csharp

Dispatcher dispatcher = new Dispatcher(new Dispatchee2((token) =>
{
    return MethodAsync(token); // Make some asynchronous call
}, DispatchTime.AtFrequency(TimeSpan.FromSeconds(10)))); // Will execute every 10 seconds.

dispatcher.RunAsync(token); // Run the dispatcher until the CancellationTokenSource is cancelled.

```

## Multiple Dispatchees Example

```csharp

Dispatcher dispatcher = new Dispatcher(new Dispatchee[]
{
    new Dispatchee1((token) =>
    {
        return MethodAsync(token); // Make some asynchronous call
    }, DispatchTime.AtMinute(1)), // Will execute on the first minute of every hour.
    new Dispatchee2((token) =>
    {
        return MethodAsync(token); // Make some asynchronous call
    }, DispatchTime.AtFrequency(TimeSpan.FromSeconds(10))) // Will execute every 10 seconds.
});

dispatcher.RunAsync(token); // Run the dispatcher until the CancellationTokenSource is cancelled.

```
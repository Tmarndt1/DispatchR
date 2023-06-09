﻿using BenchmarkDotNet.Attributes;

namespace DispatchR.Benchmark
{
    [MemoryDiagnoser]
    public class Benchmarky
    {
        private readonly IEnumerable<Dispatchee> _dispatchees = new Dispatchee[]
        {
            new FakeDispatchee(token =>
            {
                return Task.CompletedTask;
            }, DispatchTime.AtFrequency(TimeSpan.FromMilliseconds(1))),
            new FakeDispatchee(token =>
            {
                return Task.CompletedTask;
            }, DispatchTime.AtFrequency(TimeSpan.FromMilliseconds(2))),
            new FakeDispatchee(token =>
            {
                return Task.CompletedTask;
            }, DispatchTime.AtFrequency(TimeSpan.FromMilliseconds(3))),
            new FakeDispatchee(token =>
            {
                return Task.CompletedTask;
            }, DispatchTime.AtFrequency(TimeSpan.FromMilliseconds(4))),
            new FakeDispatchee(token =>
            {
                return Task.CompletedTask;
            }, DispatchTime.AtFrequency(TimeSpan.FromMilliseconds(5)))
        };

        private readonly FakeDispatcher _dispatcher;

        public Benchmarky()
        {
            _dispatcher = new FakeDispatcher(_dispatchees);
        }

        public void Run()
        {
            _dispatcher.Dispatch();
        }

        [Benchmark]
        public void RunAsync()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() =>
            {
                Thread.Sleep(10);

                cancellationTokenSource.Cancel();
            });

            // Act
            _dispatcher.RunAsync(cancellationTokenSource.Token).Wait();
        }
    }

    [DispatchOrder(1)]
    public class FakeDispatchee : Dispatchee
    {
        private readonly Func<CancellationToken, Task> _func;

        public FakeDispatchee(Func<CancellationToken, Task> func, DispatchTime dispatchTime) : base(dispatchTime)
        {
            _func = func;
        }

        public override Task ExecuteAsync(CancellationToken token = default) => _func(token);
    }

    public class FakeDispatcher : Dispatcher
    {
        private readonly IEnumerable<Dispatchee> _dispatchees;

        public FakeDispatcher(IEnumerable<Dispatchee> dispatchees) : base(dispatchees)
        {
            _dispatchees = dispatchees;
        }

        public void Dispatch()
        {
            Dispatch(_dispatchees.ToList());
        }
    }
}

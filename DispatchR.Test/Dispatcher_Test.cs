using System.Diagnostics;
using System.Reflection;

namespace DispatchR.Test
{
    public class Dispatcher_Test
    {
        /// <summary>
        /// Verifies the order attribute defines the order of execution successfully.
        /// </summary>
        [Fact]
        public void Order_Success()
        {
            // Arrange
            // Act
            Dispatcher dispatcher = new Dispatcher(new Dispatchee[]
            {
                new MockDispatchee2((token) =>
                {
                    return Task.CompletedTask;
                }, DispatchTime.AtMinute(1)),
                new MockDispatchee1((token) =>
                {
                    return Task.CompletedTask;
                }, DispatchTime.AtMinute(1))
            });

            // Assert
            FieldInfo? field = typeof(Dispatcher).GetField("_dispatchees", BindingFlags.NonPublic | BindingFlags.Instance);

            object? fieldValue = field?.GetValue(dispatcher);

            Assert.NotNull(fieldValue);

            List<Dispatchee>? dispatchees = fieldValue as List<Dispatchee>;

            Assert.Equal(nameof(MockDispatchee1), dispatchees?.First()?.GetType()?.Name);
        }

        [Fact]
        public void Order_After_Run_Success()
        {
            // Arrange
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            Dispatcher dispatcher = new Dispatcher(new Dispatchee[]
            {
                new MockDispatchee2((token) =>
                {
                    return Task.CompletedTask;
                }, DispatchTime.AtFrequency(TimeSpan.FromMilliseconds(10)))
            });

            Task.Run(() =>
            {
                Thread.Sleep(50);

                dispatcher.Add(new MockDispatchee1((token) =>
                {
                    return Task.CompletedTask;
                }, DispatchTime.AtFrequency(TimeSpan.FromMilliseconds(10))));

                Thread.Sleep(500);

                cancellationTokenSource.Cancel();
            });

            // Act
            Task.WaitAll(dispatcher.RunAsync(cancellationTokenSource.Token));

            // Assert
            FieldInfo? field = typeof(Dispatcher).GetField("_dispatchees", BindingFlags.NonPublic | BindingFlags.Instance);

            object? fieldValue = field?.GetValue(dispatcher);

            Assert.NotNull(fieldValue);

            List<Dispatchee>? dispatchees = fieldValue as List<Dispatchee>;

            Assert.Equal(nameof(MockDispatchee1), dispatchees?.First()?.GetType()?.Name);
        }

        [Fact]
        public void At_Day_Success()
        {
            // Arrange
            DateTime dateTime = DateTime.Now;

            bool ran = false;

            MockDispatchee1 dispatchee = new MockDispatchee1(token =>
            {
                ran = true;

                return Task.CompletedTask;
            }, DispatchTime.AtDay(dateTime.Day, dateTime.Hour, dateTime.Minute));

            Dispatcher dispatcher = new Dispatcher(dispatchee);

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() =>
            {
                Thread.Sleep(100);

                cancellationTokenSource.Cancel();
            });

            // Act
            dispatcher.RunAsync(cancellationTokenSource.Token).Wait();

            // Assert
            Assert.True(ran);
        }

        [Fact]
        public void At_Hour_Success()
        {
            // Arrange
            DateTime dateTime = DateTime.Now;

            bool ran = false;

            MockDispatchee1 dispatchee = new MockDispatchee1(token =>
            {
                ran = true;

                return Task.CompletedTask;
            }, DispatchTime.AtMinute(dateTime.Minute));

            Dispatcher dispatcher = new Dispatcher(dispatchee);

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() =>
            {
                Thread.Sleep(100);

                cancellationTokenSource.Cancel();
            });

            // Act
            dispatcher.RunAsync(cancellationTokenSource.Token).Wait();

            // Assert
            Assert.True(ran);
        }

        [Fact]
        public void At_Minute_Success()
        {
            // Arrange
            DateTime dateTime = DateTime.Now;

            bool ran = false;

            MockDispatchee1 dispatchee = new MockDispatchee1(token =>
            {
                ran = true;

                return Task.CompletedTask;
            }, DispatchTime.AtHour(dateTime.Hour, dateTime.Minute));

            Dispatcher dispatcher = new Dispatcher(dispatchee);

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() =>
            {
                Thread.Sleep(10);

                cancellationTokenSource.Cancel();
            });

            // Act
            dispatcher.RunAsync(cancellationTokenSource.Token).Wait();

            // Assert
            Assert.True(ran);
        }

        [Fact]
        public void At_Frequency_Success()
        {
            // Arrange
            short count = 0;

            MockDispatchee1 dispatchee = new MockDispatchee1(token =>
            {
                ++count;

                return Task.CompletedTask;
            }, DispatchTime.AtFrequency(TimeSpan.FromMilliseconds(1)));

            Dispatcher dispatcher = new Dispatcher(dispatchee);

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));

                cancellationTokenSource.Cancel();
            });

            // Act
            dispatcher.RunAsync(cancellationTokenSource.Token).Wait();

            // Assert
            Assert.True(count >= 2);
        }
    }
}
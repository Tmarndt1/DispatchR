using System.Diagnostics;

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
            object lockObj = new object();

            string name = string.Empty;

            Action<string> set = (string className) =>
            {
                lock (lockObj)
                {
                    if (string.IsNullOrEmpty(name)) name = className;
                }
            };

            Dispatcher dispatcher = new Dispatcher(new Dispatchee[]
            {
                new MockDispatchee2((token) =>
                {
                    set.Invoke(nameof(MockDispatchee2));

                    return Task.CompletedTask;
                }, DispatchTimeFactory.AtMinute(1)),
                new MockDispatchee1((token) =>
                {
                    set.Invoke(nameof(MockDispatchee1));

                    return Task.CompletedTask;
                }, DispatchTimeFactory.AtMinute(1))
            });

            // Act
            Task.WaitAll(dispatcher.DispatchAsync());

            // Assert
            Assert.Equal(nameof(MockDispatchee1), name);
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
            }, DispatchTimeFactory.AtDay(dateTime.Day, dateTime.Hour, dateTime.Minute));

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
            }, DispatchTimeFactory.AtMinute(dateTime.Minute));

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
            }, DispatchTimeFactory.AtHour(dateTime.Hour, dateTime.Minute));

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
            DateTime dateTime = DateTime.Now;

            short count = 0;

            MockDispatchee1 dispatchee = new MockDispatchee1(token =>
            {
                ++count;

                return Task.CompletedTask;
            }, DispatchTimeFactory.AtFrequency(TimeSpan.FromMilliseconds(1)));

            Dispatcher dispatcher = new Dispatcher(dispatchee);

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(3));

                cancellationTokenSource.Cancel();
            });

            // Act
            dispatcher.RunAsync(cancellationTokenSource.Token).Wait();

            // Assert
            Assert.True(count >= 2);
        }
    }
}
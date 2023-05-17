using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DispatchR
{
    /// <summary>
    /// The Dispatcher class provides a mechanism for managing a collection of Dispatchee objects that encapsulate business logic 
    /// to be executed at a certain frequency or specific time. 
    /// The class allows adding and removing Dispatchee objects, dispatching tasks asynchronously, and running tasks in a background thread. 
    /// The class is designed to work with a task scheduler or similar mechanism for executing scheduled tasks, 
    /// and provides methods for customizing the order of execution of the dispatchees based on a DispatchOrderAttribute. 
    /// The class is thread-safe, and can be used to manage a collection of dispatchees in a multi-threaded environment.
    /// </summary>
    public class Dispatcher
    {
        private List<Dispatchee> _dispatchees = new List<Dispatchee>();

        private readonly object _instanceLock = new object();
        private static readonly object _multiThreadLock = new object();

        /// <summary>
        /// Constructs a Dispatcher object with the specified Dispatchee.
        /// </summary>
        /// <param name="dispatchee">The Dispatchee object to be added and processed by the Dispatcher.</param>
        public Dispatcher(Dispatchee dispatchee)
        {
            _dispatchees.Add(dispatchee);
        }

        /// <summary>
        /// Constructs a Dispatcher object with the specified collection of Dispatchees.
        /// </summary>
        /// <param name="dispatchee">The Dispatchee collection to be added and processed by the Dispatcher.</param>
        public Dispatcher(IEnumerable<Dispatchee> dispatchees)
        {
            _dispatchees = new List<Dispatchee>(dispatchees);
            _dispatchees.Sort();
        }

        /// <summary>
        /// Adds a Dispatchee to be processed.
        /// </summary>
        /// <typeparam name="T">The derived type of the Dispatchee</typeparam>
        /// <param name="dispatchee">The Dispatchee to be addded.</param>
        public void Add<T>(T dispatchee)
            where T : Dispatchee
        {
            lock (_instanceLock)
            {
                if (_dispatchees.Contains(dispatchee)) throw new InvalidOperationException("Duplicate dispatchee found.");

                _dispatchees.Add(dispatchee);

                if (typeof(T).IsDefined(typeof(DispatchOrderAttribute), true))
                {
                    _dispatchees.Sort();
                }
            }
        }

        /// <summary>
        /// Removes a Dispatchee from the Dispatcher.
        /// </summary>
        /// <param name="dispatchee">The Dispatchee to be removed.</param>
        public void Remove(Dispatchee dispatchee)
        {
            lock (_instanceLock)
            {
                _dispatchees?.Remove(dispatchee);
            }
        }

        /// <summary>
        /// Executes the dispatchees with order priority.
        /// </summary>
        /// <param name="token">The CancellationToken</param>
        /// <returns>A collection of tasks</returns>
        public Task[] DispatchAsync(CancellationToken token = default)
        {
            return _dispatchees
                        .Select(x => x.ExecuteAsync(token))
                            .ToArray();
        }

        /// <summary>
        /// Runs the Dispatcher indefinitely until the CancellationTokenSource is cancelled.
        /// </summary>
        /// <param name="token">The CancellationToken to stop the Dispatcher's execution.</param>
        /// <returns>A long running Task.</returns>
        public Task RunAsync(CancellationToken token = default)
        {
            return Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    Dispatch(_dispatchees, token);
                }
            }, token);
        }

        protected static void Dispatch(List<Dispatchee> dispatchees, CancellationToken token = default)
        {
            Dispatchee[] array;
            lock (_multiThreadLock)
            {
                array = dispatchees.ToArray();
            }
            for (int i = 0; i < array.Length; i++)
            {
                Dispatchee dispatchee = array[i];

                if (dispatchee == null) continue;

                if (!dispatchee.ShouldExecute()) continue;

                _ = dispatchee.InvokeAsync(token);
            }
        }
    }
}

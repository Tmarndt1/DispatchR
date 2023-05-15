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

        private readonly object _lock = new object();

        /// <summary>
        /// Constructs a Dispatcher object with the specified Dispatchee.
        /// </summary>
        /// <param name="dispatchee">The Dispatchee object to be added and processed by the Dispatcher.</param>
        public Dispatcher(Dispatchee dispatchee)
        {
            AddTo(dispatchee);
        }

        /// <summary>
        /// Constructs a Dispatcher object with the specified collection of Dispatchees.
        /// </summary>
        /// <param name="dispatchee">The Dispatchee collection to be added and processed by the Dispatcher.</param>
        public Dispatcher(IEnumerable<Dispatchee> dispatchees)
        {
            foreach (var dispatchee in dispatchees)
            {
                AddTo(dispatchee);
            }

            lock (_lock)
            {
                _dispatchees = _dispatchees.SortByOrderAttr().ToList();
            }
        }

        /// <summary>
        /// Adds a Dispatchee to be processed.
        /// </summary>
        /// <typeparam name="T">The derived type of the Dispatchee</typeparam>
        /// <param name="dispatchee">The Dispatchee to be addded.</param>
        public void Add<T>(T dispatchee)
            where T : Dispatchee
        {
            AddTo(dispatchee);

            if (!typeof(T).IsDefined(typeof(DispatchOrderAttribute), true)) return;

            lock (_lock)
            {
                _dispatchees = _dispatchees.SortByOrderAttr().ToList();
            }
        }

        /// <summary>
        /// Removes a Dispatchee from the Dispatcher.
        /// </summary>
        /// <param name="dispatchee">The Dispatchee to be removed.</param>
        public void Remove(Dispatchee dispatchee)
        {
            lock (_lock)
            {
                _dispatchees?.Remove(dispatchee);
            }
        }

        public Task[] DispatchAsync(CancellationToken token = default)
        {
            return _dispatchees.SortByOrderAttr()
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

        protected void AddTo<T>(T dispatchee)
            where T : Dispatchee
        {
            lock (_lock)
            {
                if (_dispatchees.Contains(dispatchee)) throw new InvalidOperationException("Duplicate dispatchee found.");

                _dispatchees.Add(dispatchee);

                return;
            }

            throw new InvalidOperationException("DispatchTime must be defined");
        }

        protected void Dispatch(List<Dispatchee> dispatchees, CancellationToken token = default)
        {
            lock (_lock)
            {
                for (int i = 0; i < dispatchees.Count; i++)
                {
                    Dispatchee dispatchee = dispatchees[i];

                    if (dispatchee == null) continue;

                    if (!dispatchee.ShouldExecute()) continue;

                    _ = dispatchee.InvokeAsync(token);
                }
            }
        }
    }
}

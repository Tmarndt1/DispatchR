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

        public Dispatcher(Dispatchee dispatchee)
        {
            AddTo(dispatchee);
        }

        public Dispatcher(IEnumerable<Dispatchee> dispatchees)
        {
            foreach (var dispatchee in dispatchees)
            {
                AddTo(dispatchee);
            }
        }

        public void Add<T>(T dispatchee)
            where T : Dispatchee
        {
            AddTo(dispatchee);
        }

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
                if (_dispatchees == null) _dispatchees = new List<Dispatchee>();

                if (_dispatchees.Contains(dispatchee)) throw new InvalidOperationException("Duplicate dispatchee found.");

                if (typeof(T).IsDefined(typeof(DispatchOrderAttribute), true))
                {
                    _dispatchees = _dispatchees.SortByOrderAttr().ToList();
                }

                _dispatchees.Add(dispatchee);

                return;
            }

            throw new InvalidOperationException("DispatchTime must be defined");
        }

        protected void Dispatch(List<Dispatchee> dispatchees, CancellationToken token = default)
        {
            if (dispatchees == null) return;

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

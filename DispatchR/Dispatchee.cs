using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DispatchR
{
    /// <summary>
    /// The Dispatchee abstract class provides a base for defining classes that encapsulate business logic 
    /// to be executed at a certain frequency or specific time. It provides properties and methods for defining 
    /// the dispatch frequency and time, executing the business logic asynchronously, and checking if the task 
    /// should be executed based on the dispatch time. 
    /// The class is intended to be subclassed to define specific business logic, 
    /// and is designed to be used in conjunction with a task scheduler or similar mechanism for executing scheduled tasks.
    /// </summary>
    public abstract class Dispatchee : IComparable<Dispatchee>
    {
        /// <summary>
        /// The DispatchFrequency which designates when the Dispatchee should be executed.
        /// </summary>
        protected readonly DispatchFrequency? _dispatchFrequency;

        /// <summary>
        /// The DispatchDateTime which designates when the Dispatchee should be executed.
        /// </summary>
        private readonly DispatchDateTime? _dispatchDateTime;

        /// <summary>
        /// Defines whether or not the Dispatchee has completed/faulted/cancelled the task.
        /// </summary>
        private bool _isDone = true;

        private readonly object _lock = new object();

        internal bool IsDone
        {
            get
            {
                lock (_lock)
                {
                    return _isDone;
                }
            }
            set
            {
                lock (_lock)
                {
                    _isDone = value;
                }
            }
        }

        /// <summary>
        /// Executes the Dispatchee's business logic.
        /// </summary>
        /// <param name="token">The CancellationToken to cancel the execution.</param>
        /// <returns>A Task.</returns>
        public abstract Task ExecuteAsync(CancellationToken token = default);

        /// <summary>
        /// Constructor that requires the DispatchTime.
        /// </summary>
        /// <param name="dispatchTime">The DispatchTime.</param>
        protected Dispatchee(DispatchTime dispatchTime)
        {
            if (dispatchTime is DispatchDateTime dispatchDateTime) 
            {
                _dispatchDateTime = dispatchDateTime;
            }
            else if (dispatchTime is DispatchFrequency dispatchFrequency)
            {
                _dispatchFrequency = dispatchFrequency;
            }
        }

        internal async Task InvokeAsync(CancellationToken token = default)
        {
            IsDone = false;

            try
            {
                if (_dispatchFrequency != null)
                {
                    await Task.Delay(_dispatchFrequency, token);

                    await ExecuteAsync(token);
                }
                else
                {
                    await ExecuteAsync(token);
                }
            }
            catch
            {
                // Continue
            }

            IsDone = true;
        }

        internal bool ShouldExecute()
        {
            if (!IsDone) return false;

            if (_dispatchDateTime != null)
            {
                DateTime dateTime = DateTime.Now;

                if (dateTime.Minute != _dispatchDateTime.Minute) return false;

                if (_dispatchDateTime.Hour == -1 || _dispatchDateTime.Hour == dateTime.Hour) return true;

                if (_dispatchDateTime.Day == -1 || _dispatchDateTime.Day == dateTime.Day) return true;

                return false;
            }

            return true;
        }

        /// <summary>
        /// Compares one instance to another
        /// </summary>
        /// <param name="other">The other dispatchee</param>
        /// <returns>-1 if this comes before other, 1 if this comes after other, 0 otherwise</returns>
        public int CompareTo(Dispatchee? other)
        {
            if (other == null) { return -1; }
            if (Object.ReferenceEquals(other, this)) { return 0; }
            var otherAttr = this.GetType().GetCustomAttribute<DispatchOrderAttribute>(true);
            var attr = this.GetType().GetCustomAttribute<DispatchOrderAttribute>(true);
            if (attr == null) { return otherAttr == null ? 0 : 1; }
            if (otherAttr == null) { return -1; }
            if (attr.Order < otherAttr.Order) { return -1; }
            if (attr.Order > otherAttr.Order) { return 1; }
            return 0;
        }
    }
}

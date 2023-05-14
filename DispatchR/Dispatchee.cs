using System;
using System.Threading;
using System.Threading.Tasks;

namespace DispatchR
{
    public abstract class Dispatchee
    {
        /// <summary>
        /// The DispatchFrequency which designates when the Dispatchee should be executed.
        /// </summary>
        private readonly DispatchFrequency? _dispatchFrequency;

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
        public Dispatchee(DispatchTime dispatchTime)
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
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;

namespace DispatchR
{
    public abstract class Dispatchee
    {
        /// <summary>
        /// The DispatchTime which designates when the Dispatchee should be executed.
        /// </summary>
        private DispatchFrequency? _dispatchFrequency;

        private DispatchDateTime? _dispatchDateTime;

        /// <summary>
        /// Defines whether or not the Dispatchee has completed/faulted/cancelled the task.
        /// </summary>
        private bool _isDone = true;

        private Func<CancellationToken, Task> _func;

        public Dispatchee(Func<CancellationToken, Task> func, DispatchTime dispatchTime)
        {
            _func = func;

            if (dispatchTime is DispatchDateTime dispatchDateTime) 
            {
                _dispatchDateTime = dispatchDateTime;
            }
            else if (dispatchTime is DispatchFrequency dispatchFrequency)
            {
                _dispatchFrequency = dispatchFrequency;
            }
        }

        internal async Task ExecuteAsync(CancellationToken token = default)
        {
            _isDone = false;

            try
            {
                if (_dispatchFrequency != null)
                {
                    await Task.Delay(_dispatchFrequency, token);

                    await _func.Invoke(token);
                }
                else
                {
                    await _func.Invoke(token);
                }
            }
            catch
            {
                // Continue
            }

            _isDone = true;
        }

        internal bool IsDone() => _isDone;

        internal bool ShouldExecute()
        {
            if (!IsDone()) return false;

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

using System;

namespace DispatchR
{
    public class DispatchFrequency : IDispatchTime
    {
        /// <summary>
        /// Determines the frequency of how often the Dispatchee should be executed.
        /// </summary>
        public readonly TimeSpan Frequency;

        internal DispatchFrequency(TimeSpan frequency)
        {
            Frequency = frequency;
        }

        public static implicit operator TimeSpan(DispatchFrequency d) => d.Frequency;
    }
}

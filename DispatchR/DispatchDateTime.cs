using System;

namespace DispatchR
{
    public class DispatchDateTime : IDispatchTime
    {
        private int _day = -1;

        /// <summary>
        /// The day of the month a Dispatchee should be executed.
        /// </summary>
        public int Day
        {
            get => _day;
            private set
            {
                if (value < 1) throw new InvalidOperationException("A month cannot be less than 1");

                if (value > 31) throw new InvalidOperationException("A month cannot be greater than 12");

                _day = value;
            }
        }

        private int _hour = -1;

        /// <summary>
        /// The hour of the day a Dispatchee should be executed.
        /// </summary>
        public int Hour
        {
            get => _hour;
            private set
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(Hour), "A hour cannot be less than 0");

                if (value > 23) throw new ArgumentOutOfRangeException(nameof(Hour), "A hour cannot be greater than 23");

                _hour = value;
            }
        }

        private int _minute = -1;

        /// <summary>
        /// The minute of the hour a Dispatchee should be executed.
        /// </summary>
        public int Minute
        {
            get => _minute;
            private set
            {
                if (value < 0) throw new InvalidOperationException("A minute cannot be less than 0");

                if (value > 59) throw new InvalidOperationException("A minute cannot be greater than 59");

                _minute = value;
            }
        }

        internal DispatchDateTime(int day, int hour, int minute) : this(hour, minute)
        {
            Day = day;
        }

        internal DispatchDateTime(int hour, int minute) : this(minute)
        {
            Hour = hour;
        }

        internal DispatchDateTime(int minute)
        {
            Minute = minute;
        }
    }
}

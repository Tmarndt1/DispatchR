using System;

namespace DispatchR
{
    /// <summary>
    /// DispatchTime designates when a Dispatchee should be executed
    /// </summary>
    /// <remarks>
    /// Days span from 1 to 31.
    /// Hours are in military time so they span from 0 to 23.
    /// Minutes span from 0 to 59.
    /// </remarks>
    public abstract class DispatchTime
    {
        /// <summary>
        /// Designates the Dispatchee to execute at the given frequency
        /// </summary>
        /// <param name="frequency">The frequency to execute the Workflow</param>
        /// <returns>An instance of DispatchFrequency</returns>
        public static DispatchFrequency AtFrequency(TimeSpan frequency)
        {
            return new DispatchFrequency(frequency);
        }

        /// <summary>
        /// Designates the Dispatchee to execute at the given day, hour, and minute.
        /// </summary>
        /// <param name="day">The day of the month the Workflow should be executed.</param>
        /// <param name="hour">The hour of the day the Workflow should be executed.</param>
        /// <param name="minute">The minute of the hour the Workflow should be executed.</param>
        /// <returns>A new instance of DispatchDateTime.</returns>
        public static DispatchDateTime AtDay(int day, int hour, int minute)
        {
            return new DispatchDateTime(day, hour, minute);
        }

        /// <summary>
        /// Designates the Dispatchee to execute at the given day, hour, and minute 0.
        /// </summary>
        /// <param name="day">The day of the month the Workflow should be executed.</param>
        /// <param name="hour">The hour of the day the Workflow should be executed.</param>
        /// <returns>A new instance of DispatchDateTime.</returns>
        /// <remarks>
        /// Will execute at the beginning of the provided day and hour.
        /// </remarks>
        public static DispatchDateTime AtDay(int day, int hour)
        {
            return new DispatchDateTime(day, hour, 0);
        }

        /// <summary>
        /// Designates the Dispatchee to execute at the given day, hour 0, and minute 0.
        /// </summary>
        /// <param name="day">The day of the month the Workflow should be executed.</param>
        /// <returns>A new instance of DispatchDateTime.</returns>
        /// <remarks>
        /// Will execute at midnight on the given day.
        /// </remarks>
        public static DispatchDateTime AtDay(int day)
        {
            return new DispatchDateTime(day, 0, 0);
        }

        /// Designates a WorkflowScheduler should execute a Workflow at the given
        /// hour and at the given minute. 
        /// </summary>
        /// <param name="hour">The hour of the day the Workflow should be executed.</param>
        /// <param name="minute">The minute of the hour the Workflow should be executed.</param>
        /// <returns>A new instance of WorkflowTime.</returns>
        public static DispatchDateTime AtHour(int hour, int minute)
        {
            return new DispatchDateTime(hour, minute);
        }

        /// Designates the Dispatchee to execute at the given hour and minute 0.
        /// </summary>
        /// <param name="hour">The hour of the day the Workflow should be executed.</param>
        /// <returns>A new instance of DispatchDateTime.</returns>
        public static DispatchDateTime AtHour(int hour)
        {
            return new DispatchDateTime(hour, 0);
        }

        /// <summary>
        /// Designates the Dispatchee to execute at the given minute.
        /// </summary>
        /// <param name="minute">The minute of the hour the Workflow should be executed.</param>
        /// <returns>A new instance of DispatchDateTime.</returns>
        public static DispatchDateTime AtMinute(int minute)
        {
            return new DispatchDateTime(minute);
        }

        /// <summary>
        /// Constructor the restrict inheritance.
        /// </summary>
        internal DispatchTime() { }
    }

    public class DispatchDateTime : DispatchTime
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
                if (value < 1) throw new ArgumentOutOfRangeException("A day cannot be less than 1");

                if (value > 31) throw new ArgumentOutOfRangeException("A day cannot be greater than 31");

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
                if (value < 0) throw new ArgumentOutOfRangeException("A hour cannot be less than 0");

                if (value > 23) throw new ArgumentOutOfRangeException("A hour cannot be greater than 23");

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
                if (value < 0) throw new ArgumentOutOfRangeException("A minute cannot be less than 0");

                if (value > 59) throw new ArgumentOutOfRangeException("A minute cannot be greater than 59");

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

    public class DispatchFrequency : DispatchTime
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

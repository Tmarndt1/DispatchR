using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DispatchR
{
    public static class DispatchTimeFactory
    {

        /// <summary>
        /// Designates the Dispatchee to execute at the given frequency
        /// </summary>
        /// <param name="frequency">The frequency to execute the Workflow</param>
        /// <returns>An instance of WorkflowFrequency</returns>
        public static IDispatchTime AtFrequency(TimeSpan frequency)
        {
            return new DispatchFrequency(frequency);
        }

        /// <summary>
        /// Designates the Dispatchee to execute at the given day, hour, and minute.
        /// </summary>
        /// <param name="day">The day of the month the Workflow should be executed.</param>
        /// <param name="hour">The hour of the day the Workflow should be executed.</param>
        /// <param name="minute">The minute of the hour the Workflow should be executed.</param>
        /// <returns>A new instance of WorkflowTime.</returns>
        public static IDispatchTime AtDay(int day, int hour, int minute)
        {
            return new DispatchDateTime(day, hour, minute);
        }

        /// <summary>
        /// Designates the Dispatchee to execute at the given day, hour, and minute 0.
        /// </summary>
        /// <param name="day">The day of the month the Workflow should be executed.</param>
        /// <param name="hour">The hour of the day the Workflow should be executed.</param>
        /// <returns>A new instance of WorkflowTime.</returns>
        /// <remarks>
        /// Will execute at the beginning of the provided day and hour.
        /// </remarks>
        public static IDispatchTime AtDay(int day, int hour)
        {
            return new DispatchDateTime(day, hour, 0);
        }

        /// <summary>
        /// Designates the Dispatchee to execute at the given day, hour 0, and minute 0.
        /// </summary>
        /// <param name="day">The day of the month the Workflow should be executed.</param>
        /// <returns>A new instance of WorkflowTime.</returns>
        /// <remarks>
        /// Will execute at midnight on the given day.
        /// </remarks>
        public static IDispatchTime AtDay(int day)
        {
            return new DispatchDateTime(day, 0, 0);
        }

        /// <summary>
        /// Designates a WorkflowScheduler should execute a Workflow at the given
        /// hour and at the given minute. 
        /// </summary>
        /// <param name="hour">The hour of the day the Workflow should be executed.</param>
        /// <param name="minute">The minute of the hour the Workflow should be executed.</param>
        /// <returns>A new instance of WorkflowTime.</returns>
        public static IDispatchTime AtHour(int hour, int minute)
        {
            return new DispatchDateTime(hour, minute);
        }

        /// <summary>
        /// Designates the Dispatchee to execute at the given hour and minute 0.
        /// </summary>
        /// <param name="hour">The hour of the day the Workflow should be executed.</param>
        /// <returns>A new instance of WorkflowTime.</returns>
        public static IDispatchTime AtHour(int hour)
        {
            return new DispatchDateTime(hour, 0);
        }

        /// <summary>
        /// Designates the Dispatchee to execute at the given minute.
        /// </summary>
        /// <param name="minute">The minute of the hour the Workflow should be executed.</param>
        /// <returns>A new instance of WorkflowTime.</returns>
        public static IDispatchTime AtMinute(int minute)
        {
            return new DispatchDateTime(minute);
        }
    }
}

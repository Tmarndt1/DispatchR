using System;

namespace DispatchR
{
    /// <summary>
    /// An attribute specifying the order or grouping in which to dispatch. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DispatchOrderAttribute : Attribute
    {
        /// <summary>
        /// The order in which to dispatch the item.
        /// </summary>
        public short Order { get; private set; }

        /// <summary>
        /// Constructor requiring the Order value.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <exception cref="ArgumentOutOfRangeException">If provided an invalid order.</exception>
        public DispatchOrderAttribute(short order)
        {
            if (order < 0) throw new ArgumentOutOfRangeException(nameof(order), "Dispatchee's order cannot be less than 0.");

            Order = order;
        }
    }
}

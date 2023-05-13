using System;

namespace DispatchR
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DispatchOrderAttribute : Attribute
    {
        public short Order { get; private set; }

        public DispatchOrderAttribute(short order)
        {
            if (order < 0) throw new InvalidOperationException("Dispatchee's order cannot be less than zero.");

            Order = order;
        }
    }
}

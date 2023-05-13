using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DispatchR
{
    internal static class Extensions
    {
        public static IEnumerable<Dispatchee> SortByOrderAttr(this IEnumerable<Dispatchee> dispatchees)
        {
            return dispatchees.OrderBy(x =>
            {
                var attr = x.GetType().GetCustomAttribute<DispatchOrderAttribute>(true);

                return attr?.Order ?? -1;
            });
        }
    }
}

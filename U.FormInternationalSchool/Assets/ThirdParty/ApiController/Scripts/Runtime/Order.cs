using System;

namespace API
{
    public enum Order
    {
        Asc,
        Desc
    }

    internal static class OrderExtensions
    {
        internal static string ToFriendlyString(this Order order)
        {
            return order switch
            {
                Order.Asc => "ASC",
                Order.Desc => "DESC",
                _ => throw new ArgumentOutOfRangeException(nameof(order), order, null)
            };
        }
    }
}
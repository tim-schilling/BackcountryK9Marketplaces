using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackcountryK9Marketplaces.Contracts
{
    /// <summary>
    /// Represents the order status.
    /// </summary>
    public enum EOrderStatus
    {
        Unshipped = 0,
        PartiallyShipped = 1,
        Shipped = 2
    }
}

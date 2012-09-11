using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackcountryK9Marketplaces.Contracts
{
    /// <summary>
    /// Class representing the items for a fulfillment order.
    /// </summary>
    public class MarketplaceOrderFulfillmentItem
    {
        /// <summary>
        /// The quantity of the item
        /// </summary>
        public int Quantity;
        /// <summary>
        /// The unique id for the item in an order.
        /// </summary>
        public string MarketplaceOrderItemId;
    }
}

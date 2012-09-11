using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackcountryK9Marketplaces.Contracts
{
    /// <summary>
    /// Represents an item of an order on a marketplace.
    /// </summary>
    public class MarketplaceOrderItem
    {
        /// <summary>
        /// The unique id of the item for the marketplace.
        /// </summary>
        public string MarketplaceItemId;
        /// <summary>
        /// The unique id of the item on the order.
        /// </summary>
        public string MarketplaceOrderItemId;
        /// <summary>
        /// The unique identifier for the item
        /// </summary>
        public string SKU;
        /// <summary>
        /// The quantity of the item that hasn't been shipped yet.
        /// </summary>
        public int QuantityUnshipped;
        /// <summary>
        /// The quantity of the item that has been shipped.
        /// </summary>
        public int QuantityShipped;
        /// <summary>
        /// The price of the item.
        /// </summary>
        public decimal Price;
        /// <summary>
        /// The shipping price of the item.
        /// </summary>
        public decimal ShippingPrice;
        /// <summary>
        /// The tax charged for the item.
        /// </summary>
        public decimal Tax;
        /// <summary>
        /// The shipping tax charged for the item.
        /// </summary>
        public decimal ShippingTax;
    }
}

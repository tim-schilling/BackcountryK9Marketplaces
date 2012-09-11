using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackcountryK9Marketplaces.Contracts
{
    /// <summary>
    /// Class for updating an order on the marketplace with fulfillment data.
    /// </summary>
    public class MarketplaceOrderFulfillment
    {
        /// <summary>
        /// Identifies what marketplace it came from.
        /// </summary>
        public EMarketplace Marketplace;
        /// <summary>
        /// The unique id of the order
        /// </summary>
        public string MarketplaceOrderId;
        /// <summary>
        /// The date the fulfillment was made.
        /// </summary>
        public DateTime FulfillmentDate;
        /// <summary>
        /// The carrier of the order.
        /// </summary>
        public Carrier Carrier;
        /// <summary>
        /// The shipping method of the order.
        /// </summary>
        public string ShippingMethod;
        /// <summary>
        /// The tracking number for the shipper.
        /// </summary>
        public string ShipperTrackingNumber;
        /// <summary>
        /// The items on the order.
        /// </summary>
        public IEnumerable<MarketplaceOrderFulfillmentItem> Items;
    }
}

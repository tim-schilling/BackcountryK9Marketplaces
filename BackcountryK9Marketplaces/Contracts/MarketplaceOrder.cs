using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackcountryK9Marketplaces.Contracts
{
    /// <summary>
    /// Class that represents orders on the marketplace.
    /// </summary>
    public class MarketplaceOrder
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
        /// The order total
        /// </summary>
        public decimal OrderTotal;
        /// <summary>
        /// The name of the buyer
        /// </summary>
        public string BuyerName;
        /// <summary>
        /// The buyer's email
        /// </summary>
        public string BuyerEmail;
        /// <summary>
        /// The buyer's shipping phone
        /// </summary>
        public string BuyerShippingPhone;
        /// <summary>
        /// The Buyer's shipping name
        /// </summary>
        public string BuyerShippingName;
        /// <summary>
        /// The Buyer's Shipping first part of the address
        /// </summary>
        public string BuyerShippingAddress1;
        /// <summary>
        /// The Buyer's Shipping second part of the address
        /// </summary>
        public string BuyerShippingAddress2;
        /// <summary>
        /// The Buyer's Shipping third part of the address
        /// </summary>
        public string BuyerShippingAddress3;
        /// <summary>
        /// The Buyer's Shipping city
        /// </summary>
        public string BuyerShippingCity;
        /// <summary>
        /// The Buyer's Shipping state
        /// </summary>
        public string BuyerShippingState;
        /// <summary>
        /// The Buyer's Shipping zipcode
        /// </summary>
        public string BuyerShippingZip;
        /// <summary>
        /// The the level of service for the shipment (ground, expediated, next day).
        /// </summary>
        public string ShipmentServiceLevel;
        /// <summary>
        /// The items on the order.
        /// </summary>
        public IEnumerable<MarketplaceOrderItem> Items;
        /// <summary>
        /// The status of the order
        /// </summary>
        public EOrderStatus OrderStatus;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackcountryK9Marketplaces.Contracts
{
    /// <summary>
    /// Represents a line on a marketplace order refund.
    /// </summary>
    public class MarketplaceOrderRefundItem
    {
        /// <summary>
        /// The unique id of the item on the order.
        /// </summary>
        public string MarketplaceOrderItemId;
        /// <summary>
        /// The reason for refunding the item.
        /// </summary>
        public ERefundReason RefundReason;
        /// <summary>
        /// The refund price for the item. This is the total for the entire quantity of the item, 
        /// this is not a unit price. Positive refunds the customer, negative bills.
        /// </summary>
        public decimal RefundPrice;
        /// <summary>
        /// The refund price for shipping. This is the total for the entire quantity of the item, 
        /// this is not a unit price. Positive refunds the customer, negative bills.
        /// </summary>
        public decimal RefundShipping;
        /// <summary>
        /// The refund price for taxes. This is the total for the entire quantity of the item, 
        /// this is not a unit price. Positive refunds the customer, negative bills.
        /// </summary>
        public decimal RefundTax;
        /// <summary>
        /// The refund price for shipping tax. This is the total for the entire quantity of the item, 
        /// this is not a unit price. Positive refunds the customer, negative bills.
        /// </summary>
        public decimal RefundShippingTax;
        /// <summary>
        /// The cost for the return shipping. This is the total for the entire quantity of the item, 
        /// this is not a unit price. Positive refunds the customer, negative bills.
        /// </summary>
        public decimal ReturnShipping;
        /// <summary>
        /// The quantity cancelled.
        /// </summary>
        public int QuantityCancelled;
    }
}

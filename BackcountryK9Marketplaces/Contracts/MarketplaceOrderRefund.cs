using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackcountryK9Marketplaces.Contracts
{
    /// <summary>
    /// Represents an order on the marketplace that's to be refunded.
    /// </summary>
    public class MarketplaceOrderRefund
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
        /// The items on the order.
        /// </summary>
        public IEnumerable<MarketplaceOrderRefundItem> Items;

        // This should be used in the future when issuing direct refunds. For now just do refunds for exact items.
        //public IEnumerable<MarketplaceOrderRefundDirectAdjustment> adjustments;
    }
}

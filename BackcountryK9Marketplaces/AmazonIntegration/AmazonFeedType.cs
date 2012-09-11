using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackcountryK9Marketplaces.AmazonIntegration
{
    /// <summary>
    /// This defines the feed types for the amazon integration.
    /// </summary>
    public sealed class AmazonFeedType
    {
        public static readonly string InventoryUpdate = "_POST_INVENTORY_AVAILABILITY_DATA_";
        public static readonly string OrderFulfillment = "_POST_ORDER_FULFILLMENT_DATA_";
        public static readonly string OrderRefund = "_POST_PAYMENT_ADJUSTMENT_DATA_";
    }
}

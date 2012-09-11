using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackcountryK9Marketplaces.Contracts;

namespace BackcountryK9Marketplaces
{
    /// <summary>
    /// Defines the operational contract for the marketplace integration
    /// </summary>
    public interface IMarketplaceIntegration
    {
        /// <summary>
        /// Updates the marketplace's inventory so that items will have the specified quantity available.
        /// </summary>
        /// <param name="items">Collection of objects that represent the item and the quantity the inventory should be updated to.</param>
        void UpdateMarketplaceInventory(IEnumerable<MarketplaceInventoryUpdateItem> items);

        /// <summary>
        /// Fetches the newly created orders from the marketplaces from a given date.
        /// </summary>
        /// <param name="lastUpdated">The starting created order date for when orders are supposed to be pulled.</param>
        /// <returns>A collection of marketplace orders.</returns>
        IEnumerable<Contracts.MarketplaceOrder> GetMarketplaceOrders(DateTime createdAfter);

        /// <summary>
        /// Updates the tracking status of the orders.
        /// </summary>
        /// <param name="orders">A list of orders to be updated for tracking.</param>
        void UpdateMarketplaceTracking(IEnumerable<MarketplaceOrderFulfillment> orders);

        /// <summary>
        /// Refunds the list of orders.
        /// </summary>
        /// <param name="orders">Orders that have refunds to be issued to customers.</param>
        void RefundMarketplaceOrders(IEnumerable<MarketplaceOrderRefund> orders);
    }
}

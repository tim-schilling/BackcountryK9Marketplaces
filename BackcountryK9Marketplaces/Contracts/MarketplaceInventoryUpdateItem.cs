using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackcountryK9Marketplaces.Contracts
{
    /// <summary>
    /// This class represents the data contract for updating the inventory quantities on marketplace(s)
    /// </summary>
    public class MarketplaceInventoryUpdateItem
    {
        /// <summary>
        /// The unique identifier for the item
        /// </summary>
        public string SKU;
        /// <summary>
        /// The quantity of the item
        /// </summary>
        public int Quantity;
    }
}

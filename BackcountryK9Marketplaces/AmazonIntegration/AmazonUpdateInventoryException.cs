using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackcountryK9Marketplaces.AmazonIntegration
{
    /// <summary>
    /// This exception is for when Amazon can't update the inventory levels.
    /// </summary>
    public class AmazonUpdateInventoryException : Exception
    {
        /// <summary>
        /// The collection of results that errored out.
        /// </summary>
        public List<Result> ErrorResults { get; set; }

        public AmazonUpdateInventoryException(string message):base(message)
        {
            ErrorResults = new List<Result>();
        }


        public class Result
        {
            public string Code { get; set; }
            public string Description { get; set; }
            public string SKU { get; set; }
        }
    }
}
